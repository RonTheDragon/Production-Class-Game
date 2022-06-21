using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : CharacterHealth , IpooledObject
{
    Enemy enemy;
    public GameObject DeadGhost;
    EnemyAttackSystem EAS;
    NavMeshAgent nav;
    Collider col;

    new void Start()
    {
        base.Start();
        enemy = transform.parent.GetComponent<Enemy>();
        EAS = GetComponent<EnemyAttackSystem>();
        nav = GetComponent<NavMeshAgent>();
        col = GetComponent<Collider>();
        
    }
    public void OnObjectSpawn()
    {
        Start();
        enemy.enabled = true;
        enemy.anim.SetBool("Death", false);
        AlreadyDead = false;
        col.enabled = true;
    }

    new void Update()
    {
        base.Update();
        TakeKnockback();
    }
    public override void TakeDamage(float Damage, float Knock, Vector2 Stagger, Vector3 ImpactLocation, GameObject Attacker)
    {
        if (!AlreadyDead)
        {
            base.TakeDamage(Damage, Knock, Stagger, ImpactLocation, Attacker);
            enemy.GotHit(Attacker == GameManager.instance.Player);
        }
    }

    void TakeKnockback()
    {
        if (TheKnockback > 0 && !AlreadyDead)
        {
            TheKnockback -= TheKnockback * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, TheImpactLocation, -TheKnockback * Time.deltaTime);
        }
    }

    protected override void Death()
    {
        if (!AlreadyDead)
        {
            AlreadyDead = true;
            //Instantiate(DeadGhost, transform.position, transform.rotation).GetComponent<ParticleSystemRenderer>().material = transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().materials[1];
            //GameManager.Player.GetComponent<PlayerControler>().KillAdded();
            enemy.anim.SetBool("Death",true);
            enemy.CanvasHolder.SetActive(false);
            enemy.enabled = false;
            nav.SetDestination(transform.position);
            GameObject soul =ObjectPooler.Instance.SpawnFromPool("Soul", transform.position+Vector3.up, Random.rotation);
            soul.GetComponent<Soul>().SoulEnergy = enemy.SoulWorth;
            col.enabled = false;
            StartCoroutine(DisposeOfBody());
        }
    }

    protected override void GetStaggered()
    {
        EAS.Stagger();
    }

    protected override IEnumerator DisposeOfBody()
    {
        yield return new WaitForSeconds(5);
        transform.parent.gameObject.SetActive(false);
    }
}