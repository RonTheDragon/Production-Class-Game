using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : Health
{
    //CharacterController CC;
    Rigidbody RB;
    //PlayerControler PC;
    //Animator Anim;
    //AudioManager audio;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        //CC = GetComponent<CharacterController>();
        RB = GetComponent<Rigidbody>();
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
            //CC.Move((transform.position - TheImpactLocation).normalized * TheKnockback * Time.deltaTime);
            RB.AddForce((transform.position - TheImpactLocation).normalized * TheKnockback * Time.deltaTime);

        }
    }
    protected override void Death()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public override void TakeDamage(float Damage, float Knock, Vector3 ImpactLocation)
    {
        base.TakeDamage(Damage, Knock, ImpactLocation);
        //Anim.SetTrigger("Ouch");
        //audio.PlaySound(Sound.Activation.Custom, "Ouch");
    }
}