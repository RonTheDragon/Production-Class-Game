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
    }
    protected override void Death()
    {
        if (!AlreadyDead)
        {
            AlreadyDead = true;
            if (anim!=null)
            anim.SetBool("Death", true);
            CharacterAI c = GetComponent<CharacterAI>();
            c.ShowingData = 0;
            c.enabled = false;
            GetComponent<NavMeshAgent>().SetDestination(transform.position);
            StartCoroutine(DisposeOfBody());

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
