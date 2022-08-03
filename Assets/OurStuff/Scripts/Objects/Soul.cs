using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soul : MonoBehaviour , IpooledObject
{
    [HideInInspector] public int            SoulEnergy;
    [SerializeField]         float          speed = 10;
    [SerializeField]         float          rotationSpeed = 10;
                             bool           SoulAlreadyCollected;
                             ParticleSystem particle;
                             float          startingSpeed;
                             float          startingRotation;

    void Awake()
    {
        startingSpeed = speed;
        startingRotation = rotationSpeed;
        particle      = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.SoulSucker != null)
        {
            if (GameManager.instance.SoulSucker.gameObject != null && !SoulAlreadyCollected)
            {
                if (speed<30)
                speed += Time.deltaTime*2;
                rotationSpeed += Time.deltaTime*5;
                float s = 1 + (0.1f * SoulEnergy);
                transform.localScale = new Vector3(s, s, s);
                float dist = Vector3.Distance(transform.position, GameManager.instance.SoulSucker.position);
                transform.position += transform.forward * (speed) * Time.deltaTime;
                Vector3 newDirection = Vector3.RotateTowards(transform.forward, GameManager.instance.SoulSucker.position - transform.position, rotationSpeed * Time.deltaTime, 0.0f);
                if (dist < 1 && !SoulAlreadyCollected)
                {
                    transform.LookAt(GameManager.instance.SoulSucker.position);
                    rotationSpeed = 0;
                }
                else
                {
                    transform.rotation = Quaternion.LookRotation(newDirection);
                }
                if (dist < 0.2f && !SoulAlreadyCollected)
                {
                    SoulCollected();
                }
            }
        }
    }

    void SoulCollected()
    {
        SoulAlreadyCollected = true;
        GameManager.instance.SoulEnergy += (int)(SoulEnergy * GameManager.instance.SoulWorth);
        if (GameManager.instance.Player != null)
        {
            PlayerHealth hp = GameManager.instance.Player.GetComponent<PlayerHealth>();
            if (hp != null)
            {
                hp.GainSoul();
            }
        }
        StartCoroutine(SoulDisappear());
    }

    public void OnObjectSpawn()
    {
        speed = startingSpeed;
        rotationSpeed = startingRotation;
        SoulAlreadyCollected = false;
        RotateToRandom();
    }

    IEnumerator SoulDisappear()
    {
        particle.Stop();
        yield return new WaitForSeconds(1);    
        gameObject.SetActive(false);
    }

    void RotateToRandom()
    {
        transform.rotation = Quaternion.Euler(-Random.Range(0,180), Random.rotation.y, 0);
    }
}
