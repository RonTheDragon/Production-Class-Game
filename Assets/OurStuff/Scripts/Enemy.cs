using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    Vector3 previousPos;
    float OriginalSpeed;
    float OriginalDetectionRange;
    NavMeshAgent NMA;
    GameObject Player;
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

    //[SerializeField] float RandomSoundMaxCooldown = 5;
    float SoundCoolDown;

    EnemyAttackSystem eas;
    EnemyHealth hp;
    Animator anim;
    SkinnedMeshRenderer MR;
    AudioManager Audio;
    ParticleSystem Particle;
    ParticleSystemRenderer PR;

    // public Image HpBar;
    // public Image StaminaBar;

    Transform TheEnemy;

    float StoppingDistance;
     
    void Awake()
    {
        TheEnemy = transform.GetChild(0);
        NMA = TheEnemy.GetComponent<NavMeshAgent>();
        eas = TheEnemy.GetComponent<EnemyAttackSystem>();
        hp = TheEnemy.GetComponent<EnemyHealth>();
        Audio = TheEnemy.GetComponent<AudioManager>();
        Particle = TheEnemy.GetComponent<ParticleSystem>();
        PR = TheEnemy.GetComponent<ParticleSystemRenderer>();
        anim = TheEnemy.GetChild(0).GetComponent<Animator>();
        // MR = anim.transform.GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>();

        previousPos = TheEnemy.transform.position;
    }

    void Start()
    {
        Player=GameManager.instance.Player;
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
        }
        //  HpBar.fillAmount = 0;
        //   StaminaBar.fillAmount = 0;
        EnemyAI();
        PlayRandomSound();
        WalkingAnimation();
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
    }

    protected void EnemyAI()
    {
        if (canSeePlayer())
        {
            alert += Time.deltaTime;
        }
        else if (alert > 0)
        {
            alert -= Time.deltaTime;
        }
            if(alert > timeToAlert)
            chasingPlayer = true;
            else
            chasingPlayer = false;

        if (chasingPlayer)
        {
            DetectionRange = OriginalDetectionRange * 1.5f;
            if (eas.AttackCooldown <= 0)
            {
                NMA.speed = OriginalSpeed * 1.5f;
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
                NMA.speed = OriginalSpeed;
                NMA.stoppingDistance = StoppingDistance-2;
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

        if (eas.Stamina < eas.Tired) { NMA.speed = OriginalSpeed * 0.5f; }
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
        if (BravePercent >= 100 - bravery)
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
        Vector3 MoveTo = new Vector3(TheEnemy.position.x, TheEnemy.position.y, TheWall.transform.position.z);
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

    public void GotHit()
    {
        //Particle.Emit(5); <-------RETURN LATER
        //Audio.PlaySound(Sound.Activation.Custom, "Ah"); <-------RETURN LATER
        //anim.SetTrigger("Ouch"); <-------RETURN LATER
        alert += 2;
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
}