using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    Enemy enemy;
    public GameObject DeadGhost;
    EnemyAttackSystem EAS;

    new void Start()
    {
        base.Start();
        enemy = transform.parent.GetComponent<Enemy>();
        EAS = GetComponent<EnemyAttackSystem>();
    }

    new void Update()
    {
        base.Update();
        TakeKnockback();
    }
    public override void TakeDamage(float Damage, float Knock, float minStagger, float maxStagger, Vector3 ImpactLocation)
    {
        base.TakeDamage(Damage, Knock , minStagger, maxStagger, ImpactLocation);
        enemy.GotHit();
    }

    void TakeKnockback()
    {
        if (TheKnockback > 0)
        {
            TheKnockback -= TheKnockback * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, TheImpactLocation, -TheKnockback * Time.deltaTime);
        }
    }

    protected override void Death()
    {
        //Instantiate(DeadGhost, transform.position, transform.rotation).GetComponent<ParticleSystemRenderer>().material = transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().materials[1];
        //GameManager.Player.GetComponent<PlayerControler>().KillAdded();
        Destroy(transform.parent.gameObject);
    }

    protected override void GetStaggered()
    {
        EAS.Stagger();
    }
}