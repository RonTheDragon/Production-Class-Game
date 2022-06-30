using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soul : MonoBehaviour , IpooledObject
{
    [HideInInspector] public int SoulEnergy;
    [SerializeField] float speed = 10;
    [SerializeField] float rotationSpeed = 10;
    bool SoulAlreadyCollected;
    ParticleSystem particle;
    float startingSpeed;

    void Start()
    {
        startingSpeed = speed;
        particle = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.SoulSucker != null)
        {
            if (GameManager.instance.SoulSucker.gameObject != null && !SoulAlreadyCollected)
            {
                speed += Time.deltaTime*2;
                float s = 1 + (0.1f * SoulEnergy);
                transform.localScale = new Vector3(s, s, s);
                float dist = Vector3.Distance(transform.position, GameManager.instance.SoulSucker.position);
                transform.position += transform.forward * (speed) * Time.deltaTime;
                Vector3 newDirection = Vector3.RotateTowards(transform.forward, GameManager.instance.SoulSucker.position - transform.position, rotationSpeed * Time.deltaTime, 0.0f);
                transform.rotation = Quaternion.LookRotation(newDirection);
                if (dist < 0.1f && !SoulAlreadyCollected)
                {
                    SoulCollected();
                }
            }
        }
    }

    void SoulCollected()
    {
        SoulAlreadyCollected = true;
        GameManager.instance.SoulEnergy += SoulEnergy;
        StartCoroutine(SoulDisappear());
    }

    public void OnObjectSpawn()
    {
        speed = startingSpeed;
        SoulAlreadyCollected = false;
    }

    IEnumerator SoulDisappear()
    {
        particle.Stop();
        yield return new WaitForSeconds(1);    
        gameObject.SetActive(false);
    }
}
