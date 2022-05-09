using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : CharacterHealth , IpooledObject
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
    public void OnObjectSpawn()
    {
        Start();
    }

    new void Update()
    {
        base.Update();
        TakeKnockback();
    }
    public override void TakeDamage(float Damage, float Knock, Vector2 Stagger, Vector3 ImpactLocation)
    {
        base.TakeDamage(Damage, Knock , Stagger, ImpactLocation);
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
        transform.parent.gameObject.SetActive(false);
    }

    protected override void GetStaggered()
    {
        EAS.Stagger();
    }

}