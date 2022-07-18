using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantExplosion : Attack , IpooledObject
{
    public float ExplosionRadius;
    ParticleSystem p;

    public void Awake()
    {
        p = GetComponent<ParticleSystem>();
    }

    public void OnObjectSpawn()
    {
        StartCoroutine(Explode());     
    }
    
    IEnumerator Explode()
    {
        yield return null;
        Collider[] colliders = Physics.OverlapSphere(transform.position, ExplosionRadius);
        foreach (Collider c in colliders)
        {
            if (Attackable == (Attackable | (1 << c.gameObject.layer)))
            {
                Health TargetHp = c.transform.GetComponent<Health>();
                if (TargetHp != null)
                {
                    float distance = Vector3.Distance(transform.position, c.ClosestPoint(transform.position));
                    float Effect = (ExplosionRadius - distance) / ExplosionRadius;
                    if (Effect < 0) { Effect = 0; }
                    TargetHp.TakeDamage(Damage * Effect, Knock * Effect, Stagger * Effect, Temperature * Effect, transform.position, Attacker);
                }
            }
        }
        if (p != null)
        {
           p.Play();
        }
        LastEffect();

    }

    public virtual void LastEffect()
    {
        //Empty for Overriding.
    }
}
