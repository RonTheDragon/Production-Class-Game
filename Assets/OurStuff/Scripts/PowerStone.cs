using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerStone : MonoBehaviour
{
    

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerHealth hp = other.GetComponent<PlayerHealth>();
            if (hp != null)
            {
                if (!hp.isDead())
                {
                    hp?.CollectPowerStone();
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
