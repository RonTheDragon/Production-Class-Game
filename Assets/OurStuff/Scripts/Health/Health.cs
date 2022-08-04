using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Health : MonoBehaviour
{
                      public float MaxHp = 100;
                      public float Hp;
                      public float HpRegen;
    [HideInInspector] public bool  AlreadyDead;

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
            if (HpRegen > 0)
                Hp += Time.deltaTime * HpRegen;
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

    public virtual void TakeDamage(float Damage, float knock, Vector2 Stagger, float Temperature, Vector3 ImpactLocation, GameObject Attacker)
    {
        Hp -= Damage;
        WallSound();
    }

    protected virtual void WallSound()
    {

    }

    protected abstract void Death();
}
