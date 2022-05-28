using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : Projectile, IpooledObject
{
    public float ExplosionRadius = 10;
    Rigidbody RB;
    // Start is called before the first frame update
    void Awake()
    {
        RB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
                        TargetHp.TakeDamage(Damage* Effect, Knock* Effect, Stagger*Effect, transform.position,Attacker);
                    }
                }
            }
            gameObject.SetActive(false);
        }
        
    }
}