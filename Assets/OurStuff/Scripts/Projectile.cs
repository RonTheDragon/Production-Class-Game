using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Attack
{
    public float Speed = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    protected virtual void Movement()
    {
        transform.position += transform.forward * Speed * Time.deltaTime;
    }

    private void OnTriggerStay(Collider other)
    {

        if (Attackable == (Attackable | (1 << other.gameObject.layer)))
        {
            Health TargetHp = other.transform.GetComponent<Health>();
            if (TargetHp != null)
            {
                TargetHp.TakeDamage(Damage, Knock, transform.position);
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
