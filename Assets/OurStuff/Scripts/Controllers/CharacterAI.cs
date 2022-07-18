using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public abstract class CharacterAI : MonoBehaviour , IpooledObject
{
    public Vector2 SoulValue = new Vector2(5, 10);
    [HideInInspector] public int SoulWorth;
    protected Vector3 previousPos;
    protected float OriginalSpeed;
    protected float OriginalDetectionRange;
    protected NavMeshAgent NMA;
    protected GameObject Player;
    public GameObject Target;
    protected Health TargetHealth;
    protected Transform PlayerCam;
    protected GameObject TheWall;

    protected float SearchCoolDown = 1;
    protected float searchCool;

    protected float dist;
    public float DetectionRange;
    protected float alert;
    protected bool chasingTarget;
    [SerializeField] float RoamCooldown = 10;
    protected float roamCooldown;
    [SerializeField] float RoamRadius = 10;
    public float BraveEnoughToFight = 50;
    public float timeToAlert = 0.5f;

    public GameObject CanvasHolder;
    public float CanvasHolderHeight;
    protected Image hpBar;
    protected Image staminaBar;
    [HideInInspector] public float ShowingData;
    public  Transform CharacterAnimationBody;
    protected Vector3 startposforFix;

    //[SerializeField] float RandomSoundMaxCooldown = 5;
    protected float SoundCoolDown;

    protected AIAttackSystem eas;
    protected CharacterHealth hp;
    protected SkinnedMeshRenderer MR;
    protected AudioManager Audio;
    protected ParticleSystem Particle;
    protected ParticleSystemRenderer PR;

    // public Image HpBar;
    // public Image StaminaBar;

    protected Transform TheBody;

    protected float StoppingDistance;

    protected LayerMask Targetable;
    protected LayerMask CanSee;

    public Animator anim;

    protected void Awake()
    {
        //TheBody = transform.GetChild(0);
        NMA = TheBody.GetComponent<NavMeshAgent>();
        eas = TheBody.GetComponent<AIAttackSystem>();
        hp = TheBody.GetComponent<CharacterHealth>();
        Audio = TheBody.GetComponent<AudioManager>();
        Particle = TheBody.GetComponent<ParticleSystem>();
        PR = TheBody.GetComponent<ParticleSystemRenderer>();
        // MR = anim.transform.GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>();

        previousPos = TheBody.transform.position;
    }

    protected void Start()
    {
        TheWall = GameManager.instance.Wall;
        if (CharacterAnimationBody != null)
        {
            startposforFix = CharacterAnimationBody.localPosition;
        }
        Player =GameManager.instance.Player;
        if (Player != null)
        {
            PlayerCam = GameManager.instance.Player?.GetComponent<ThirdPersonMovement>().cam;
        }
        OriginalSpeed = NMA.speed;
        OriginalDetectionRange = DetectionRange;
        StoppingDistance = NMA.stoppingDistance;
    }

    protected void Update()
    {
        if (hp == null)
        {
            hp = TheBody.GetComponent<CharacterHealth>();
        }  
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
            Behaviour();
        }
        if (anim != null)
        {
            WalkingAnimation();
        }
        if (GameManager.instance.Data != GameManager.ShowEnemyData.Never && GameManager.instance.Player!=null && CanvasHolder!=null)
        {
            ShowData();
        }
    }

    private void WalkingAnimation()
    {
        if (previousPos == TheBody.transform.position)
        {
            anim.SetInteger("Walk", 0);
        }
        else
        {
            anim.SetInteger("Walk", 1);
            previousPos = TheBody.transform.position;
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

    protected abstract void Behaviour();
    
    public void ForgetTarget()
    {
        chasingTarget = false;
        Target = null;
        eas.CancelAttack();
    }
    protected void PlayRandomSound()
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

    protected void RunningAway()
    {
        if (Target != null)
        {
            NMA.SetDestination(TheBody.position + ((TheBody.position - Target.transform.position).normalized * 5));
        }
    }

    protected void ChaseTarget()
    {
        if (Target != null)
        {
            if (eas.AttackCooldown <= 0)
            {
                NMA.SetDestination(Target.transform.position);
            }
            if (dist <= NMA.stoppingDistance || NMA.speed == 0)
            {
                RotateTowards(Target.transform.position);
            }
            eas.Attack(Target.transform.position, true);
        }
    }

    protected void AttackWall()
    {
        if (!hp.Frozen)
        {
            Vector3 MoveTo;

            float WallSmaller = GameManager.instance.WallLength * 0.8f;
            if (GameManager.instance.WallFacingZ)
            {
                float Xpos = Mathf.Clamp(TheBody.position.x, TheWall.transform.position.x - (WallSmaller / 2), TheWall.transform.position.x + (WallSmaller / 2));
                MoveTo = new Vector3(Xpos, TheBody.position.y, TheWall.transform.position.z);
            }
            else
            {
                float Zpos = Mathf.Clamp(TheBody.position.z, TheWall.transform.position.z - (WallSmaller / 2), TheWall.transform.position.z + (WallSmaller / 2));
                // Debug.Log(Zpos);
                MoveTo = new Vector3(TheWall.transform.position.x + 2, TheBody.position.y, Zpos);
            }

            NMA.SetDestination(MoveTo);
            float distance = Vector3.Distance(TheBody.position, MoveTo);
            if (distance <= NMA.stoppingDistance + 1 || NMA.speed == 0)
            {
                RotateTowards(MoveTo);
            }
            eas.Attack(MoveTo, false);
        }else NMA.SetDestination(transform.position);
    }

    protected void Wander()
    {
        eas.CancelAttack();
        if (roamCooldown > 0) { roamCooldown -= Time.deltaTime; }
        else
        {
            roamCooldown = Random.Range(0, RoamCooldown);
            float x = Random.Range(-RoamRadius, RoamRadius);
            float z = Random.Range(-RoamRadius + 0.1f, RoamRadius);
            Vector3 MoveTo = new Vector3(TheBody.position.x + x, TheBody.position.y, TheBody.position.z + z);
            NMA.SetDestination(MoveTo);
        }
    }

    protected void SearchForTarget()
    {
        if (searchCool > 0)
        {
            searchCool -= Time.deltaTime;
        }
        else
        {
            searchCool = SearchCoolDown;

            Collider[] colliders = Physics.OverlapSphere(TheBody.transform.position, DetectionRange);
            GameObject ClosestTarget = null;
            float ClosestDist = Mathf.Infinity;
            foreach (Collider c in colliders)
            {
                if (Targetable == (Targetable | (1 << c.gameObject.layer)) && c.gameObject != GameManager.instance.Wall)
                {
                    float dist = Vector3.Distance(TheBody.transform.position, c.transform.position);
                    if (dist < ClosestDist)
                    {
                        ClosestDist = dist;
                        ClosestTarget = c.gameObject;
                    }
                }
            }
            if (ClosestTarget != null)
            {
                        Target = ClosestTarget;
                        TargetHealth = Target.GetComponent<Health>();
                
                        alert = 0.5f;
                
            }
            
        } 
    }


    protected bool eyesightCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(TheBody.position, (Target.transform.position - TheBody.position).normalized, out hit, DetectionRange, CanSee))
        {
            //Debug.Log(hit.collider.gameObject.name);
            if (hit.transform.gameObject == Target)
            {
                return true;
            }
        }
        return false;
    }

    protected bool canSeeTarget()
    {
        dist = Vector3.Distance(TheBody.position, Target.transform.position);
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
        if (ByPlayer)
        {
            alert += 2;
            Target = Player;
        }
        if (!hp.Frozen)
        {
            Particle.Emit((int)(Damage/7.5f));
            Audio.PlaySound(Sound.Activation.Custom, "Ah");
        } 
       
    }

    protected void RotateTowards(Vector3 target)
    {
        Vector3 targetlocation = new Vector3(target.x, TheBody.position.y, target.z);
        Vector3 direction = (targetlocation - TheBody.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            TheBody.rotation = Quaternion.Slerp(TheBody.rotation, lookRotation, Time.deltaTime * NMA.angularSpeed);
        }
    }

    public void OnObjectSpawn()
    {
        CharacterAnimationBody.localPosition = startposforFix;
        TheBody.transform.position = transform.parent.position;
        TheBody.GetComponent<NavMeshAgent>().enabled = true;
        SoulWorth = (int)Random.Range(SoulValue.x, SoulValue.y);
        if (hp is EnemyHealth)
        {
            EnemyHealth h = (EnemyHealth)hp;
            h.OnObjectSpawn();
        }
    }

    protected void ShowData()
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
            if (CanvasHolder != null)
            {
                if (CanvasHolder.activeSelf == true)
                    CanvasHolder.SetActive(false);
            }
        }
    }

    protected void DataUpdating()
    {
        if (CanvasHolder != null)
        {
            
            CanvasHolder.transform.position = new Vector3(TheBody.transform.position.x, TheBody.transform.position.y+ CanvasHolderHeight, TheBody.transform.position.z);
            CanvasHolder.transform.GetChild(0).LookAt(PlayerCam.transform.position);
            if (hpBar == null)
            {
                hpBar = CanvasHolder.transform.GetChild(0).GetChild(0).GetComponent<Image>();
                staminaBar = CanvasHolder.transform.GetChild(0).GetChild(1).GetComponent<Image>();
            }
            hpBar.fillAmount = hp.Hp / hp.MaxHp;
            if (eas != null)
            {
                staminaBar.fillAmount = eas.Stamina / eas.MaxStamina;
            }
            else
            {
                staminaBar.fillAmount = 0;
            }
        }
    }

    protected float GetSpeed()
    {
        float speed = OriginalSpeed;
        if (hp.Temperature < 0) { speed *= 0.01f * (hp.Temperature+100); }
        if (hp.Frozen) { speed = 0; NMA.SetDestination(transform.position); }
        return speed;
    }
}