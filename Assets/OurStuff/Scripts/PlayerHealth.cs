using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : Health
{
    CharacterController CC;
    Rigidbody RB;
    PlayerAttackSystem PAS;
    //PlayerControler PC;
    //Animator Anim;
    //AudioManager audio;

    bool iGotThePower;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        CC = GetComponent<CharacterController>();
        RB = GetComponent<Rigidbody>();
        PAS = GetComponent<PlayerAttackSystem>();
        //Anim = transform.GetChild(0).GetComponent<Animator>();
        //audio = GetComponent<AudioManager>();
        //PC = transform.parent.GetComponent<PlayerControler>();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        TakeKnockback();
    }
    void TakeKnockback()
    {
        if (TheKnockback > 0)
        {
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
            Instantiate(GameManager.instance.PowerStone, transform.position, transform.rotation);
        }
        GameManager.instance.Player = Instantiate(GameManager.instance.Player, GetComponent<RespawmIfFallsOffMap>().startPos, transform.rotation/* FIX LATER*/);
        Destroy(gameObject);
    }

    protected override void GetStaggered()
    {
        PAS.Stagger();
    }

    public override void TakeDamage(float Damage, float Knock, float minStagger, float maxStagger, Vector3 ImpactLocation)
    {
        base.TakeDamage(Damage, Knock, minStagger, maxStagger, ImpactLocation);
        //Anim.SetTrigger("Ouch");
        //audio.PlaySound(Sound.Activation.Custom, "Ouch");
    }

    public void CollectPowerStone()
    {
        iGotThePower = true;
        PAS.DamageMultiplier = 2;
        MaxHp *= 2;
        Hp = MaxHp;
    }
}