using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : Projectile, IpooledObject
{
    Rigidbody RB;
    [SerializeField] string LeavesBehind; 
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
            
                
                    Health TargetHp = other.transform.GetComponent<Health>();
                    if (TargetHp != null)
                    {
                       
                        TargetHp.TakeDamage(Damage, Knock, Stagger,Temperature, transform.position, Attacker);
                    }
                
            
            GameObject Boom = ObjectPooler.Instance.SpawnFromPool(LeavesBehind, transform.position, transform.rotation);
            ParticleSystem P = Boom.GetComponent<ParticleSystem>();
            if (P != null) P.Play();
            gameObject.SetActive(false);
        }

    }
}
