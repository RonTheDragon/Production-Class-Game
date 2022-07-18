using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AllyHealth : CharacterHealth
{
    Animator anim;
    new void Start()
    {
        base.Start();     
        anim = GetComponent<CharacterAI>().anim;
    }

    new void Update()
    {
        base.Update();  
        TakeKnockback();
        
    }
    protected override void Death()
    {
        if (!AlreadyDead)
        {
            AlreadyDead = true;
                StopIce();
            if (anim != null)
            {
                anim.SetBool("Frozen", false);
                anim.SetBool("Death", true);
            }
            CharacterAI c = GetComponent<CharacterAI>();
            c.ShowingData = 0;
            c.enabled = false;
            GetComponent<NavMeshAgent>().SetDestination(transform.position);
            StartCoroutine(DisposeOfBody());

        }
    }
    void TakeKnockback()
    {
        if (!Frozen)
        {
            if (TheKnockback > 0 && !AlreadyDead)
            {
                TheKnockback -= TheKnockback * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, TheImpactLocation, -TheKnockback * Time.deltaTime);
            }
        }
        else
        {
            TheKnockback = 0;
        }
    }
    protected override IEnumerator DisposeOfBody()
    {
        yield return new WaitForSeconds(3);
        ThirdPersonMovement t = GetComponent<ThirdPersonMovement>();
        if (t != null)
        {
            Destroy(gameObject);
        }
        else
        {
            if (transform.parent!=null)
            transform.parent.gameObject.SetActive(false);
        }
    }

    protected override void GetStaggered()
    {
        AIAttackSystem aas = GetComponent<AIAttackSystem>();
        if (aas != null)
        {
            aas.Stagger();
        }
    }
}
