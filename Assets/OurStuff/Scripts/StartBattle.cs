using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBattle : MonoBehaviour , ItownClickable
{
    public void OnClicked()
    {
        GameManager.instance.GetComponent<PlayerRespawnManager>().SpawnFirstCharacter();
        GameManager.instance.GetComponent<MonsterSpawner>().StartWave();
        GameManager.instance.Wall.GetComponent<TheWall>().SetUpAllCooldowns(true);
        WallHealth hp = GameManager.instance.Wall.GetComponent<WallHealth>();
        hp.AlreadyDead = false;
        hp.Hp = hp.MaxHp;
        GameManager.instance.GetComponent<TownManager>().TownCamera.gameObject.SetActive(false);
        GameManager.instance.AlreadyWon = false;
    }

}
