using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerStone : MonoBehaviour
{
    void Update()
    {
        if (GameManager.instance.Player != null)
        {
            transform.position = Vector3.MoveTowards(transform.position,
            GameManager.instance.Player.transform.position, GameManager.instance.RemnantMagnet * Time.deltaTime);
        }
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
