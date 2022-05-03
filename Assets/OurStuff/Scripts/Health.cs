using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Health : MonoBehaviour
{
    public float MaxHp = 100;
    public float Hp;
    [SerializeField] float HpRegan;
    protected float TheKnockback;
    protected Vector3 TheImpactLocation;

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
    }

    public virtual void TakeDamage(float Damage, float knock, Vector3 ImpactLocation)
    {
        Hp -= Damage;
        TheKnockback = knock;
        TheImpactLocation = ImpactLocation;
    }

    protected virtual void Death()
    {
        gameObject.SetActive(false);
    }

}