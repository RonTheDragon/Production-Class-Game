using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour , IpooledObject
{
    [SerializeField] Vector2 SoulValue = new Vector2(5, 10);
    [HideInInspector] public int SoulWorth;
    Vector3 previousPos;
    float OriginalSpeed;
    float OriginalDetectionRange;
    NavMeshAgent NMA;
    GameObject Player;
    Transform PlayerCam;
    GameObject TheWall;
    float dist;
    [SerializeField] float DetectionRange;
    [SerializeField] float alert;
    bool chasingPlayer;
    [SerializeField] float RoamCooldown = 10;
    float roamCooldown;
    [SerializeField] float RoamRadius = 10;
    [SerializeField] float BraveEnoughToFight = 50;
    [SerializeField] float timeToAlert = 0.5f;

    public GameObject CanvasHolder;
    public float CanvasHolderHeight;
    Image hpBar;
    Image staminaBar;
    [HideInInspector] public float ShowingData;
    [SerializeField] Transform EnemyAnimationBody;
    Vector3 startposforFix;

    //[SerializeField] float RandomSoundMaxCooldown = 5;
    float SoundCoolDown;

    EnemyAttackSystem eas;
    EnemyHealth hp;
    SkinnedMeshRenderer MR;
    AudioManager Audio;
    ParticleSystem Particle;
    ParticleSystemRenderer PR;

    // public Image HpBar;
    // public Image StaminaBar;

    Transform TheEnemy;

    float StoppingDistance;
     
    
    public Animator anim;

    void Awake()
    {
        TheEnemy = transform.GetChild(0);
        NMA = TheEnemy.GetComponent<NavMeshAgent>();
        eas = TheEnemy.GetComponent<EnemyAttackSystem>();
        hp = TheEnemy.GetComponent<EnemyHealth>();
        Audio = TheEnemy.GetComponent<AudioManager>();
        Particle = TheEnemy.GetComponent<ParticleSystem>();
        PR = TheEnemy.GetComponent<ParticleSystemRenderer>();
        // MR = anim.transform.GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>();

        previousPos = TheEnemy.transform.position;
    }

    void Start()
    {
        startposforFix = EnemyAnimationBody.position;
        Player =GameManager.instance.Player;
        if (Player != null)
        {
            PlayerCam = GameManager.instance.Player?.GetComponent<ThirdPersonMovement>().cam;
        }
        TheWall = GameManager.instance.Wall;
        OriginalSpeed = NMA.speed;
        OriginalDetectionRange = DetectionRange;
        StoppingDistance = NMA.stoppingDistance;
    }

    void Update()
    {
        if (Player == null)
        {
            Player = GameManager.instance.Player;
            if (Player != null)
            {
                PlayerCam = Player.GetComponent<ThirdPersonMovement>().cam;
            }
        }
        //  HpBar.fillAmount = 0;
        //   StaminaBar.fillAmount = 0;
        PlayRandomSound();
        if (!hp.Frozen)
        {
            EnemyAI();
        }
        WalkingAnimation();
        if (GameManager.instance.Data != GameManager.ShowEnemyData.Never && GameManager.instance.Player!=null)
        {
            ShowData();
        }
    }

    private void WalkingAnimation()
    {
        if (previousPos == TheEnemy.transform.position)
        {
            anim.SetInteger("Walk", 0);
        }
        else
        {
            anim.SetInteger("Walk", 1);
            previousPos = TheEnemy.transform.position;
        }
        if (hp.Frozen)
        {
            anim.SetBool("Frozen", true);
        }
        else
        {
            anim.SetBool("Frozen", false);
        }
    }

    protected void EnemyAI()
    {
        if (Player != null)
        {
            if (canSeePlayer())
            {
                alert += Time.deltaTime;
            }
            else if (alert > 0)
            {
                alert -= Time.deltaTime;
            }

            if (alert > timeToAlert)
                chasingPlayer = true;
            else
                chasingPlayer = false;
        }
        else { chasingPlayer = false; }

            if (chasingPlayer)
            {
                DetectionRange = OriginalDetectionRange * 1.5f;
                if (eas.AttackCooldown <= 0)
                {
                    NMA.speed = GetSpeed() * 1.5f;
                    NMA.stoppingDistance = StoppingDistance;
                }
                else
                {
                    NMA.speed = 0;
                }

                if (CheckBravery(BraveEnoughToFight))
                {
                    ChasePlayer();
                }
                else
                {
                    RunningAway();
                }
            }
        
        else
        {
            DetectionRange = OriginalDetectionRange;
            if (eas.AttackCooldown <= 0)
            {
                NMA.speed = GetSpeed();
                NMA.stoppingDistance = StoppingDistance - 2;
            }
            else
            {
                NMA.speed = 0;
            }

            if (TheWall == null)
            {
                Wander();
            }
            else
            {
                AttackWall();
            }
        }

        if (eas.Stamina < eas.Tired) { NMA.speed = GetSpeed() * 0.5f; }
    }

    void PlayRandomSound()
    {
        if (SoundCoolDown <= 0)
        {
            SoundCoolDown = Random.Range(3, 3 + SoundCoolDown);
          //  Audio.PlaySound(Sound.Activation.Custom, "Boo");
        }
        else { SoundCoolDown -= Time.deltaTime; }
    }

   public bool CheckBravery(float bravery)
    {
        float maxBrave = hp.MaxHp + eas.MaxStamina;
        float currentBravery = hp.Hp + eas.Stamina;
        float BravePercent = currentBravery / maxBrave * 100;
        if (BravePercent >= bravery)
        {
            return true;
        }
        else
        {
            return false;
        }
    } 

    void RunningAway()
    {
        NMA.SetDestination(TheEnemy.position + ((TheEnemy.position - Player.transform.position).normalized * 5));
    }

    void ChasePlayer()
    {      
        NMA.SetDestination(Player.transform.position);
        if (dist <= NMA.stoppingDistance || NMA.speed ==0)
        {          
            RotateTowards(Player.transform.position);              
        }
        eas.Attack(Player.transform.position,true);
    }

    void AttackWall()
    {
        Vector3 MoveTo;

        float WallSmaller = GameManager.instance.WallLength * 0.8f;
        if (GameManager.instance.WallFacingZ)
        {
            float Xpos = Mathf.Clamp(TheEnemy.position.x, TheWall.transform.position.x - (WallSmaller / 2), TheWall.transform.position.x + (WallSmaller / 2));
            MoveTo = new Vector3(Xpos, TheEnemy.position.y, TheWall.transform.position.z);
        }
        else
        {
            float Zpos = Mathf.Clamp(TheEnemy.position.z, TheWall.transform.position.z - (WallSmaller / 2), TheWall.transform.position.z + (WallSmaller / 2));
           // Debug.Log(Zpos);
            MoveTo = new Vector3(TheWall.transform.position.x+ 2, TheEnemy.position.y, Zpos);
        }

        NMA.SetDestination(MoveTo);
        float distance = Vector3.Distance(TheEnemy.transform.position, MoveTo);
        if (distance <= NMA.stoppingDistance + 1 || NMA.speed == 0)
        {
            RotateTowards(MoveTo);
        }
        eas.Attack(MoveTo,false);
    }

    void Wander()
    {
        if (roamCooldown > 0) { roamCooldown -= Time.deltaTime; }
        else
        {
            roamCooldown = Random.Range(0, RoamCooldown);
            float x = Random.Range(-RoamRadius, RoamRadius);
            float z = Random.Range(-RoamRadius + 0.1f, RoamRadius);
            Vector3 MoveTo = new Vector3(TheEnemy.position.x + x, TheEnemy.position.y, TheEnemy.position.z + z);
            NMA.SetDestination(MoveTo);
        }
    }

    bool eyesightCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(TheEnemy.position, (Player.transform.position - TheEnemy.position).normalized, out hit, DetectionRange, GameManager.instance.enemiesCanSee))
        {
            //Debug.Log(hit.collider.gameObject.name);
            if (hit.transform.gameObject == Player)
            {
                return true;
            }
        }
        return false;
    }

    bool canSeePlayer()
    {
        dist = Vector3.Distance(TheEnemy.position, Player.transform.position);
        if (dist <= DetectionRange && alert < 3)
        {
            if (eyesightCheck())
                return true;
            else return false;
        }
        else return false;
    } 

    public void GotHit(bool ByPlayer, float Damage)
    {
        //Audio.PlaySound(Sound.Activation.Custom, "Ah"); <-------RETURN LATER
        if (ByPlayer)
        alert += 2;
        if (!hp.Frozen) 
        Particle.Emit((int)(Damage/7.5f));
        else
        {
            hp.IceParticles.Emit((int)(Damage / 7.5f));
        }
        ShowingData = 5;
    }

    private void RotateTowards(Vector3 target)
    {
        Vector3 targetlocation = new Vector3(target.x, TheEnemy.position.y, target.z);
        Vector3 direction = (targetlocation - TheEnemy.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            TheEnemy.rotation = Quaternion.Slerp(TheEnemy.rotation, lookRotation, Time.deltaTime * NMA.angularSpeed);
        }
    }

    public void OnObjectSpawn()
    {
        EnemyAnimationBody.position = startposforFix;
        TheEnemy.transform.position = transform.position;
        TheEnemy.GetComponent<NavMeshAgent>().enabled = true;
        SoulWorth = (int)Random.Range(SoulValue.x, SoulValue.y);
        hp.OnObjectSpawn();
    }

    void ShowData()
    {

        if (GameManager.instance.Data == GameManager.ShowEnemyData.Always)
        {
            if (CanvasHolder.activeSelf==false)
            CanvasHolder.SetActive(true);
            DataUpdating();
        }
        else if (ShowingData>0)
        {
            ShowingData -= Time.deltaTime;
            if (CanvasHolder.activeSelf == false)
                CanvasHolder.SetActive(true);
            DataUpdating();
        }
        else
        {
            if (CanvasHolder.activeSelf == true)
                CanvasHolder.SetActive(false);
        }
    }

    void DataUpdating()
    {
        if (CanvasHolder != null)
        {
            
            CanvasHolder.transform.position = new Vector3(TheEnemy.transform.position.x, TheEnemy.transform.position.y+ CanvasHolderHeight, TheEnemy.transform.position.z);
            CanvasHolder.transform.GetChild(0).LookAt(PlayerCam.transform.position);
            if (hpBar == null)
            {
                hpBar = CanvasHolder.transform.GetChild(0).GetChild(0).GetComponent<Image>();
                staminaBar = CanvasHolder.transform.GetChild(0).GetChild(1).GetComponent<Image>();
            }
            hpBar.fillAmount = hp.Hp / hp.MaxHp;
            staminaBar.fillAmount = eas.Stamina / eas.MaxStamina;
        }
    }

    float GetSpeed()
    {
        float speed = OriginalSpeed;
        if (hp.Temperature < 0) { speed *= 0.01f * (hp.Temperature+100); }
        if (hp.Frozen) speed = 0;
        return speed;
    }
}