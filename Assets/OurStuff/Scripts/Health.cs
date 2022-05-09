using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Health : MonoBehaviour
{
    public float MaxHp = 100;
    public float Hp;
    public float HpRegan;

    // Start is called before the first frame update
    protected void Start()
    {
        Hp = MaxHp;
    }

    protected void Update()
    {
        HealthMechanic();
    }

    protected virtual void HealthMechanic()
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

    public virtual void TakeDamage(float Damage, float knock, Vector2 Stagger, Vector3 ImpactLocation)
    {
            Hp -= Damage;
    }

    protected abstract void Death();
}
