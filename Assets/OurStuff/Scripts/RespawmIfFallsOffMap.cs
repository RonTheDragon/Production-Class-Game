using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawmIfFallsOffMap : MonoBehaviour
{
    Vector3 startPos;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -10)
        {
            CharacterController CC = transform.GetComponent<CharacterController>();
            Rigidbody rb = transform.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
            }
            if (CC != null)
            {
                CC.enabled = false;
                transform.position = startPos;
                CC.enabled = true;
            }
            else transform.position = startPos;

        }
    }
}