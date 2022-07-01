using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenetratingProjectile : Projectile , IpooledObject
{
    List<Collider> TriggerList = new List<Collider>();




    protected override void OnTriggerStay(Collider other)
    {
        if (!other.isTrigger)
        {
            if (Attackable == (Attackable | (1 << other.gameObject.layer)))
            {
                if (!TriggerList.Contains(other))
                {
                    TriggerList.Add(other);
                    Health TargetHp = other.transform.GetComponent<Health>();
                    if (TargetHp != null)
                    {
                        TargetHp.TakeDamage(Damage, Knock, Stagger,Temperature, transform.parent.position, Attacker);
                    }
                }
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }


    public void OnObjectSpawn()
    {
        TriggerList = new List<Collider>();
    }
}
