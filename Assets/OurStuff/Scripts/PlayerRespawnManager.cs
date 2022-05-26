using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawnManager : MonoBehaviour
{
    [SerializeField] List<SOPlayerBody> PlayerBodies = new List<SOPlayerBody>();
    List<PlayerParameters> playerParameters;


    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void SetCharacterList(int amount)
    {
        playerParameters = new List<PlayerParameters>();
        for (int i = 0; i < amount; i++)
        {
            int   playerSelected   = Random.Range(0, PlayerBodies.Count);
            float damageMultiplier = Random.Range(PlayerBodies[playerSelected].damageMultiplier.x,
                PlayerBodies[playerSelected].damageMultiplier.y);
            float health           = Random.Range(PlayerBodies[playerSelected].health.x, PlayerBodies[playerSelected].health.y);
            playerParameters.Add(new PlayerParameters(PlayerBodies[playerSelected].Body, PlayerBodies[playerSelected].role,
                damageMultiplier, health));
        }
    }

    public void ShowOptions()
    {

    }
}

public class PlayerParameters
{
    public GameObject PlayerBody;
    public SOclass    role;
    public float      damageMultiplier;
    public float      health;

    public PlayerParameters(GameObject PlayerBody, SOclass role, float damageMultiplier, float health)
    {
        this.PlayerBody       = PlayerBody;
        this.role             = role;
        this.damageMultiplier = damageMultiplier;
        this.health           = health;
    }
}