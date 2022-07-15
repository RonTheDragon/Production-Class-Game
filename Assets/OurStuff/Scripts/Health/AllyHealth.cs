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
            anim.SetBool("Death", true);
            GetComponent<CharacterAI>().enabled = false;
            GetComponent<NavMeshAgent>().SetDestination(transform.position);
            StartCoroutine(DisposeOfBody());
        }
    }
    protected override IEnumerator DisposeOfBody()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }

    protected override void GetStaggered()
    {
        GetComponent<AIAttackSystem>().Stagger();
    }
}
