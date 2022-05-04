using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Health : MonoBehaviour
{
    public float MaxHp = 100;
    public float Hp;
    [SerializeField] float StaggerResistance;
    [SerializeField] float HpRegan;
    protected float TheKnockback;
    protected Vector3 TheImpactLocation;

    float TempHpProtection = 1;
    float TempKnockProtection = 1;
    float TempTimeLeft;

    protected void Start()
    {
        Hp = MaxHp;
    }


    protected void Update()
    {
        HealthMechanic();
    }

    protected void HealthMechanic()
    {

        if (Hp < MaxHp)
        {
            if (HpRegan > 0)
                Hp += Time.deltaTime * HpRegan;
        }
        else
        {
            Hp = MaxHp;
        }

        if (Hp <= 0)
        {
            Death();
        }

        if (TempTimeLeft > 0)
        {
            TempTimeLeft -= Time.deltaTime;
        }
    }

    public virtual void TakeDamage(float Damage, float knock, float minStagger ,float maxStagger, Vector3 ImpactLocation)
    {
        if (TempTimeLeft > 0)
        {
            Hp -= Damage * TempHpProtection;
            TheKnockback = knock * TempKnockProtection;
        }
        else
        {
            Hp -= Damage;
            TheKnockback = knock;
            TryStagger(minStagger, maxStagger);
            
        }
        TheImpactLocation = ImpactLocation;
    }

    public void TryStagger(float minStagger, float maxStagger)
    {
        float S = Random.Range(minStagger, maxStagger);
        if (S > StaggerResistance)
        {
            GetStaggered();
        }
    }

    protected abstract void GetStaggered();


    public void GainTempProtection(float hp, float knock, float time)
    {
        TempHpProtection = hp;
        TempKnockProtection = knock;
        TempTimeLeft = time;
    }

    protected virtual void Death()
    {
        gameObject.SetActive(false);
    }

}