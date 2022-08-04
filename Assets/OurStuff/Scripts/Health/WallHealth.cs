using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WallHealth : Health
{

    AudioManager audioManager;

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
        audioManager = GetComponent<AudioManager>();
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    protected override void WallSound()
    {
        audioManager.PlaySound(Sound.Activation.Custom,"WallDamage");
    }

}
