using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : CharacterHealth , IpooledObject
{
    [HideInInspector] public float StartMaxHp;
    Enemy             enemy;
    AIAttackSystem EAS;
    NavMeshAgent      nav;
    Collider          col;

    void Awake()
    {
        StartMaxHp = MaxHp;
    }

    new void Start()
    {
        base.Start();
        enemy = GetComponent<Enemy>();
        EAS = GetComponent<AIAttackSystem>();
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
        Temperature = 0;
    }

    new void Update()
    {
        base.Update();
        TakeKnockback();
    }
    public override void TakeDamage(float Damage, float Knock, Vector2 Stagger, float Temperature,
        Vector3 ImpactLocation, GameObject Attacker)
    {
        if (!AlreadyDead)
        {
            base.TakeDamage(Damage, Knock, Stagger, Temperature, ImpactLocation, Attacker);
            enemy.GotHit(Attacker == GameManager.instance.Player,Damage);
        }
    }

    void TakeKnockback()
    {
        if (!Frozen)
        {
            if (TheKnockback > 0)
            {
                TheKnockback -= TheKnockback * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, TheImpactLocation, - TheKnockback * Time.deltaTime);
            }
        }
        else
        {
            TheKnockback = 0;
        }
    }

    protected override void Death()
    {
        if (!AlreadyDead)
        {
            AlreadyDead = true;
            StopIce();
            enemy.anim.SetBool("Frozen", false);
            enemy.anim.SetBool("Death",true);
            TheKnockback = 10;
            enemy.CanvasHolder.SetActive(false);
            enemy.enabled   = false;
            nav.isStopped = true;
            nav.speed = 0;
            GameObject soul = ObjectPooler.Instance.SpawnFromPool("Soul", transform.position+Vector3.up, transform.rotation);
            soul.GetComponent<Soul>().SoulEnergy = enemy.SoulWorth;
            int r = Random.Range(0, 100);
            if (r < GameManager.instance.TwinSouls)
            {
                soul = ObjectPooler.Instance.SpawnFromPool("Soul", transform.position + Vector3.up, transform.rotation);
                soul.GetComponent<Soul>().SoulEnergy = enemy.SoulWorth;
            }
            col.enabled     = false;
            StartCoroutine(DisposeOfBody());
        }
    }

    protected override void GetStaggered()
    {
        enemy.anim.Rebind();
        EAS.Stagger();
    }

    protected override IEnumerator DisposeOfBody()
    {
        yield return new WaitForSeconds(5);
        enemy.anim.Rebind();
        enemy.ForgetTarget();
        transform.parent.gameObject.SetActive(false);
    }
}