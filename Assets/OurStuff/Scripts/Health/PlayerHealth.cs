using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : CharacterHealth
{
    CharacterController CC;
    Rigidbody           RB;
    PlayerAttackSystem  PAS;
    Animator            anim;
    //PlayerControler PC;
    //Animator Anim;
    //AudioManager audio;

    bool iGotThePower;
    bool CanKillSelf;
    [SerializeField] GameObject MashToBreakFree;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        CC          = GetComponent<CharacterController>();
        RB          = GetComponent<Rigidbody>();
        PAS         = GetComponent<PlayerAttackSystem>();
        anim        = GetComponent<ThirdPersonMovement>().animator;
        CanKillSelf = GameManager.instance.PlayerKillingShortCut;
        //Anim      = transform.GetChild(0).GetComponent<Animator>();
        //audio     = GetComponent<AudioManager>();
        //PC        = transform.parent.GetComponent<PlayerControler>();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        TakeKnockback();
        if (CanKillSelf)
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                Death();
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                GameManager.instance.Wall.GetComponent<WallHealth>().Hp = -1;
            }
        }
        if (Frozen)
        {
            if (!MashToBreakFree.activeSelf)
            MashToBreakFree.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Temperature += 5;
                IceParticles.Emit(10);
                if (Audio != null)
                    Audio.PlaySound(Sound.Activation.Custom, "Frozen Ah");
            }
        }
        else
        {
            if (MashToBreakFree.activeSelf)
                MashToBreakFree.SetActive(false);
        }
    }
    void TakeKnockback()
    {
        if (TheKnockback > 0 && !AlreadyDead)
        {
            if (PAS.AttackMovementForce > 0) { TheKnockback = 0; }
            TheKnockback -= TheKnockback * Time.deltaTime * 2;
            Vector3 Knock = (transform.position - TheImpactLocation).normalized * TheKnockback * Time.deltaTime;
            Knock.y = 0;
            //RB.AddForce(Knock);
            CC.Move(Knock);

        }
    }

    protected override void Death()
    {
        if (!AlreadyDead)
        {
            AlreadyDead = true;
            if (!GameManager.instance.AlreadyWon)
            {
                //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                if (iGotThePower)
                {
                    LoseStone();
                    GameManager.instance.SoulSucker = Instantiate(GameManager.instance.PowerStone, transform.position, transform.rotation).transform;
                }
                //GameManager.instance.Player = Instantiate(GameManager.instance.Player, GetComponent<RespawmIfFallsOffMap>().startPos, transform.rotation/* FIX LATER*/);
                StopIce();
                anim.SetBool("Frozen", false);
                anim.SetBool("Death", true);
                CC.enabled = false;
                PAS.enabled = false;
                StartCoroutine(DisposeOfBody());

            }
        }
    }

    

    protected override void GetStaggered()
    {
        PAS.Stagger();
    }

    public override void TakeDamage(float Damage, float Knock, Vector2 Stagger,  float Temperature , Vector3 ImpactLocation, GameObject Attacker)
    {
        
            if (!AlreadyDead)
            {
                base.TakeDamage(Damage, Knock, Stagger, Temperature, ImpactLocation, Attacker);
            }
        
        //Anim.SetTrigger("Ouch");
        //audio.PlaySound(Sound.Activation.Custom, "Ouch");
    }

    public void CollectPowerStone()
    {
        iGotThePower = true;
        PAS.DamageMultiplier *= 2;
        MaxHp *= 2;
        Hp = MaxHp;
        transform.localScale += new Vector3(0.3f, 0.3f, 0.3f);
        GameManager.instance.SoulSucker = transform;
    }
    public void LoseStone()
    {
        iGotThePower = false;
        PAS.DamageMultiplier /= 2;
        MaxHp /= 2;
        Hp = MaxHp;
        transform.localScale -= new Vector3(0.3f, 0.3f, 0.3f);
        if (GameManager.instance.RemnantBlastDamage != 0)
        {
            GameObject Boom = ObjectPooler.Instance.SpawnFromPool("RemnantExplosion", transform.position, transform.rotation);
            InstantExplosion i = Boom.GetComponent<InstantExplosion>();
            if (i != null)
            {
                i.Damage = GameManager.instance.RemnantBlastDamage;
                i.ExplosionRadius = GameManager.instance.RemnantBlastRadius;
                i.Stagger = new Vector2(100,200);
                i.Attackable = GameManager.instance.PlayerCanAttack;
                i.Attacker = gameObject;
            }
        }
    }

    protected override IEnumerator DisposeOfBody()
    {
        yield return new WaitForSeconds(3);
        if (GameManager.instance.AlreadyWon == false)
        {
            GameManager.instance.GetComponent<PlayerRespawnManager>().OpenRespawnMenu();
            Destroy(gameObject);
        }
    }

    public void GainSoul()
    {
        if (iGotThePower && !AlreadyDead)
        {
            PAS.Stamina += GameManager.instance.SoulEnvigoration;
            Hp          += GameManager.instance.SoulHeal;
        }
    }
}