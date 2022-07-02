using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : Attack
{
    public bool AnimationTrigger;
    public float AttackCooldown;
    public float AttackMovementForce;
    public Vector2 AttackMovementDirection;
    float cooldown;
    List<Collider> TriggerList = new List<Collider>();
    Collider TheTrigger;
    [HideInInspector] public AttackSystem attackSystem;
    bool _alreadyON;


    // Start is called before the first frame update
    void Start()
    {
        TheTrigger = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cooldown > 0) { cooldown -= Time.deltaTime; }
        if (!TheTrigger.enabled && TriggerList.Count>0) { TriggerList = new List<Collider>(); }

        if (!_alreadyON && AnimationTrigger)
        {
            _alreadyON = true;
            attackSystem.AttackMovementForce = AttackMovementForce*Charge;
            attackSystem.AttackMovementDirection = AttackMovementDirection;
            attackSystem.AttackMovement();
        }
        else if (_alreadyON && !AnimationTrigger)
        {
            _alreadyON = false;
        }

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
                        TargetHp.TakeDamage(Damage*Charge, Knock*Charge, Stagger*Charge, Temperature*Charge, transform.parent.position,Attacker);
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