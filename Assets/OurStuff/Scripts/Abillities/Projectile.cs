using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Attack
{
    private AudioManager audioManager;

    public float Speed = 10;
    public float Gravity = 0;

    private void Start()
    {
        audioManager = GetComponent<AudioManager>();
    }

    protected void Update()
    {
        
    }

    protected void FixedUpdate()
    {
        Movement();
    }

    protected virtual void Movement()
    {
        transform.position += transform.forward * Speed * Time.deltaTime;
        if (transform.rotation.x < 90)
        {
            transform.Rotate(Gravity * Time.deltaTime, 0, 0);
        }
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (!other.isTrigger)
        {
            if (Attackable == (Attackable | (1 << other.gameObject.layer)))
            {
                Health TargetHp = other.transform.GetComponent<Health>();
                if (TargetHp != null)
                {
                    TargetHp.TakeDamage(Damage, Knock, Stagger,Temperature, transform.position, Attacker);
                    gameObject.SetActive(false);
                    //Destroy(gameObject);
                }
            }
            else
            {
                gameObject.SetActive(false);
                //Destroy(gameObject);
            }
        }
    }
}
