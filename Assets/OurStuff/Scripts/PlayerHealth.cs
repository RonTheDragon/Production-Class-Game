using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : CharacterHealth
{
    CharacterController CC;
    Rigidbody RB;
    PlayerAttackSystem PAS;
    //PlayerControler PC;
    //Animator Anim;
    //AudioManager audio;

    bool iGotThePower;
    bool CanKillSelf;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        CC = GetComponent<CharacterController>();
        RB = GetComponent<Rigidbody>();
        PAS = GetComponent<PlayerAttackSystem>();
        CanKillSelf = GameManager.instance.PlayerKillingShortCut;
        //Anim = transform.GetChild(0).GetComponent<Animator>();
        //audio = GetComponent<AudioManager>();
        //PC = transform.parent.GetComponent<PlayerControler>();
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
        }
    }
    void TakeKnockback()
    {
        if (TheKnockback > 0)
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
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        if (iGotThePower)
        {
            LoseStone();
            Instantiate(GameManager.instance.PowerStone, transform.position, transform.rotation);
        }
        //GameManager.instance.Player = Instantiate(GameManager.instance.Player, GetComponent<RespawmIfFallsOffMap>().startPos, transform.rotation/* FIX LATER*/);
        GameManager.instance.GetComponent<PlayerRespawnManager>().OpenRespawnMenu();
        Destroy(gameObject);
    }

    protected override void GetStaggered()
    {
        PAS.Stagger();
    }

    public override void TakeDamage(float Damage, float Knock, Vector2 Stagger, Vector3 ImpactLocation, GameObject Attacker)
    {
        base.TakeDamage(Damage, Knock, Stagger, ImpactLocation, Attacker);
        //Anim.SetTrigger("Ouch");
        //audio.PlaySound(Sound.Activation.Custom, "Ouch");
    }

    public void CollectPowerStone()
    {
        iGotThePower = true;
        PAS.DamageMultiplier *= 2;
        MaxHp *= 2;
        Hp = MaxHp;
        transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
    }
    public void LoseStone()
    {
        iGotThePower = false;
        PAS.DamageMultiplier /= 2;
        MaxHp /= 2;
        Hp = MaxHp;
        transform.localScale = new Vector3(1f, 1f, 1f);
    }
}