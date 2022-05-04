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
            hp?.CollectPowerStone();
            Destroy(this.gameObject);
        }
    }
}
