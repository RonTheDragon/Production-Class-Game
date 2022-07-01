using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollision : MonoBehaviour
{
    public string pName;
    [HideInInspector] public ParticleAttack p;

    //Wall Particle Only
    [HideInInspector] public TheWall w;
    [HideInInspector] public float Damage;
    [HideInInspector] public float Knock;
    [HideInInspector] public Vector2 Stagger;
    [HideInInspector] public float Temperature;

    private void OnParticleCollision(GameObject other)
    {
        if (p != null)
        {
            if (p.cooldown <= 0 && p.Attackable == (p.Attackable | (1 << other.gameObject.layer)))
            {
                Health TargetHp = other.transform.GetComponent<Health>();
                if (TargetHp != null)
                {
                    p.cooldown = p.AttackCooldown;
                    TargetHp.TakeDamage(p.Damage * p.Charge, p.Knock * p.Charge, p.Stagger * p.Charge, p.Temperature * p.Charge, transform.position, p.Attacker);
                }
            }
        }
        else if (w != null)
        {
            if (GameManager.instance.PlayerCanAttack == (GameManager.instance.PlayerCanAttack | (1 << other.gameObject.layer)))
            {
                Health TargetHp = other.transform.GetComponent<Health>();
                if (TargetHp != null)
                {
                    TargetHp.TakeDamage(Damage, Knock, Stagger, Temperature, transform.position, w.gameObject);
                }
            }
        }
    }
}
