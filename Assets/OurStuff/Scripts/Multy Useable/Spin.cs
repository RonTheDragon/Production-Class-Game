using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    [SerializeField] float direction = 1;
    [SerializeField] float timeToChangeDir;
                     float timer;

    [SerializeField] Vector3 rotation;

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            timer = timeToChangeDir;
            direction = -direction;
        }

        transform.Rotate(rotation * Time.deltaTime);
        transform.position += Vector3.up * Time.deltaTime * direction;
    }
}
