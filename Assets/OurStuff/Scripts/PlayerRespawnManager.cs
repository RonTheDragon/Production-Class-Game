using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRespawnManager : MonoBehaviour
{
    [SerializeField] List<SOPlayerBody> PlayerBodies = new List<SOPlayerBody>();
    List<PlayerParameters> playerParameters;
    [SerializeField] GameObject RespawnMenu;
    [SerializeField] GameObject Content;
    [SerializeField] GameObject ChooseHeroButton;
    [SerializeField] List<string> CharacterNames = new List<string>();
    [SerializeField] Transform PlayerRespawnLocation;

    void Start()
    {
        SpawnFirstCharacter();
    }

    void Update()
    {
        
    }

    public void SpawnFirstCharacter()
    {
        RespawnPlayer(CreateCharacter());
        StartCoroutine(SpawnFirstRock());
    }

    IEnumerator SpawnFirstRock()
    {
        yield return null;
        Instantiate(GameManager.instance.PowerStone, PlayerRespawnLocation.position, PlayerRespawnLocation.rotation);
    }

    public void SetCharacterList(int amount)
    {
        playerParameters = new List<PlayerParameters>();
        for (int i = 0; i < amount; i++)
        {
            PlayerParameters par = CreateCharacter();

            playerParameters.Add(par);
        }
    }

    private PlayerParameters CreateCharacter()
    {
        int playerSelected = Random.Range(0, PlayerBodies.Count);

        float damageMultiplier = Random.Range(PlayerBodies[playerSelected].damageMultiplier.x,
            PlayerBodies[playerSelected].damageMultiplier.y);

        float health = Random.Range(PlayerBodies[playerSelected].health.x, PlayerBodies[playerSelected].health.y);

        string PersonalName = CharacterNames[Random.Range(0, CharacterNames.Count)];

        PlayerParameters par = new PlayerParameters(
            PlayerBodies[playerSelected].Body,
            PlayerBodies[playerSelected].role,
            PlayerBodies[playerSelected].RoleName,
            PersonalName,
            damageMultiplier,
            health);
        return par;
    }

    public void OpenRespawnMenu()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        foreach (Transform c in Content.transform) //Kill All Previous Cards
        {
            Destroy(c.gameObject);
        }

        RespawnMenu.SetActive(true);

        if (playerParameters.Count > 0)
        {

                for (int i = 0; i < playerParameters.Count; i++) //Rebuild Cards
                {
                GameObject Card = Instantiate(ChooseHeroButton, transform.position, transform.rotation, Content.transform);
                Card.GetComponent<ChooseHeroButton>().SetTheCharacter(playerParameters[i], i);
                }
        }
        else
        {
            StartCoroutine(GameLost());
        }
    }

    public IEnumerator GameLost()
    {
        Debug.Log("Game Over");
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RemoveCard(int Remove)
    {
        playerParameters.RemoveAt(Remove);
        RespawnMenu.SetActive(false);
    }

    public void RespawnPlayer(PlayerParameters player)
    {
        GameObject RespawnedPlayer = Instantiate(player.PlayerBody, PlayerRespawnLocation.position, PlayerRespawnLocation.rotation);
        GameManager.instance.Player = RespawnedPlayer;
        PlayerHealth hp =  RespawnedPlayer.GetComponent<PlayerHealth>();
        hp.MaxHp = player.health;
        hp.Hp = player.health;
        PlayerAttackSystem pas = RespawnedPlayer.GetComponent<PlayerAttackSystem>();
        pas.PlayerClass = player.role;
        pas.DamageMultiplier = player.damageMultiplier;
    }
}

public class PlayerParameters
{
    public GameObject PlayerBody;
    public SOclass    role;
    public string     RoleName;
    public string     PersonalName;
    public float      damageMultiplier;
    public float      health;

    public PlayerParameters(GameObject PlayerBody, SOclass role,string RoleName, string PersonalName, float damageMultiplier, float health)
    {
        this.PlayerBody       = PlayerBody;
        this.role             = role;
        this.RoleName         = RoleName;
        this.PersonalName     = PersonalName;
        this.damageMultiplier = damageMultiplier;
        this.health           = health;
    }
}