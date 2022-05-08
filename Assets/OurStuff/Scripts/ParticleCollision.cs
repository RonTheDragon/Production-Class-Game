using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollision : MonoBehaviour
{
    public string pName;
    public ParticleAttack p;
    
    private void OnParticleCollision(GameObject other)
    {
        if (p.cooldown <= 0 && p.Attackable == (p.Attackable | (1 << other.gameObject.layer)))
        {
            Health TargetHp = other.transform.GetComponent<Health>();
            if (TargetHp != null)
            {
                p.cooldown = p.AttackCooldown;
                TargetHp.TakeDamage(p.Damage*p.Charge, p.Knock*p.Charge, p.minStagger, p.maxStagger ,transform.position);
            }
        }
    }
}
