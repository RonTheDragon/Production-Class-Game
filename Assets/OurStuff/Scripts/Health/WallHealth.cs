using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WallHealth : Health
{

    [SerializeField] AudioSource audioSource;

    protected override void Death()
    {
        if (!AlreadyDead)
        {
            AlreadyDead = true;
            StartCoroutine(GameManager.instance.GetComponent<PlayerRespawnManager>().GameLost());       
        }
    }

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    protected override void WallSound()
    {
        audioSource.Play();
    }

}
