using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : Attack
{
    public float AttackCooldown;
    float cooldown;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (cooldown > 0) { cooldown -= Time.deltaTime; }
    }


    private void OnTriggerStay(Collider other)
    {

        if (cooldown <= 0 && Attackable == (Attackable | (1 << other.gameObject.layer)))
        {
            Health TargetHp = other.transform.GetComponent<Health>();
            if (TargetHp != null)
            {
                cooldown = AttackCooldown;
                TargetHp.TakeDamage(Damage, Knock, transform.parent.position);
            }
        }
    }

}