using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : ExplosiveProjectile, IpooledObject
{
    Rigidbody RB;
    // Start is called before the first frame update
    void Awake()
    {
        RB = GetComponent<Rigidbody>();
    }

    protected override void Movement()
    {
        RB.AddForce(transform.forward * Speed * Time.deltaTime);
    }

    IEnumerator StartMove()
    {
        yield return null;
        Movement();
    }

    public void OnObjectSpawn()
    {
        StartCoroutine(StartMove());
    }

    protected override void OnTriggerStay(Collider other)
    {

        if (Attackable == (Attackable | (1 << other.gameObject.layer)))
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, ExplosionRadius);
            foreach (Collider c in colliders)
            {
                if (Attackable == (Attackable | (1 << c.gameObject.layer)))
                {
                    Health TargetHp = c.transform.GetComponent<Health>();
                    if (TargetHp != null)
                    {
                        float distance = Vector3.Distance(transform.position, c.transform.position);
                        float Effect = (ExplosionRadius - distance)/ ExplosionRadius;
                        TargetHp.TakeDamage(Damage* Effect, Knock* Effect, Stagger*Effect, Temperature*Effect , transform.position,Attacker);
                    }
                }
            }
            GameObject Boom = ObjectPooler.Instance.SpawnFromPool(Explosion, transform.position, transform.rotation);
            Boom.GetComponent<ParticleSystem>().Play();
            gameObject.SetActive(false);
        }
        
    }
}
