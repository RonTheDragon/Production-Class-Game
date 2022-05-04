using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : Attack
{
    public float AttackCooldown;
    float cooldown;
    List<Collider> TriggerList = new List<Collider>();


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
            foreach(Collider c in TriggerList) {
                if (c != null)
                {
                    Health TargetHp = c.transform.GetComponent<Health>();
                    if (TargetHp != null)
                    {
                        cooldown = AttackCooldown;
                        TargetHp.TakeDamage(Damage, Knock, transform.parent.position);
                    }
                }
            }
        }
    }
 
 //called when something enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        if (Attackable == (Attackable | (1 << other.gameObject.layer)))
        {
            //if the object is not already in the list
            if (!TriggerList.Contains(other))
            {
                //add the object to the list
                TriggerList.Add(other);
            }
        }
    }

    //called when something exits the trigger
    private void OnTriggerExit(Collider other)
    {
        if (Attackable == (Attackable | (1 << other.gameObject.layer)))
        {
            //if the object is in the list
            if (TriggerList.Contains(other))
            {
                //remove it from the list
                TriggerList.Remove(other);
            }
        }
    }

}