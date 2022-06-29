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
        //SpawnFirstCharacter();
        RespawnMenu.SetActive(false);
    }

    void Update()
    {
        
    }

    public void SpawnFirstCharacter()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (GameManager.instance.Player == null) // if no player
        {
            RespawnPlayer(CreateCharacter());
            StartCoroutine(SpawnFirstRock());
        }
        else                                                // if there is a player
        {
            if (GameManager.instance.Player.GetComponent<PlayerHealth>().AlreadyDead == true)
            {
                Destroy(GameManager.instance.Player);
                if (GameManager.instance.SoulSucker!=null)
                Destroy(GameManager.instance.SoulSucker.gameObject);
                RespawnPlayer(CreateCharacter());
                StartCoroutine(SpawnFirstRock());
                return;
            }

            GameObject Player = GameManager.instance.Player;
            Player.transform.position = PlayerRespawnLocation.position;
            Player.SetActive(true);
            Health hp = Player.GetComponent<Health>();
            hp.Hp = hp.MaxHp;
            if (GameManager.instance.SoulSucker != null)
            {
                if (GameManager.instance.SoulSucker.gameObject != Player)
                {
                    Destroy(GameManager.instance.SoulSucker.gameObject);
                    StartCoroutine(SpawnFirstRock());
                }
            }else StartCoroutine(SpawnFirstRock());
        }
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

        float dm = 1;
        float hm = 1;
        switch (PlayerBodies[playerSelected].classRole)
        {
            case PlayerParameters.ClassRole.Warrior:
                dm = GameManager.instance.WarriorDamageMultiplier;
                hm = GameManager.instance.WarriorHealthMultiplier;
                break;
            case PlayerParameters.ClassRole.Rogue:
                dm = GameManager.instance.RogueDamageMultiplier;
                hm = GameManager.instance.RogueHealthMultiplier;
                break;
            case PlayerParameters.ClassRole.Mage:
                dm = GameManager.instance.MageDamageMultiplier;
                hm = GameManager.instance.MageHealthMultiplier;
                break;
        }

        PlayerParameters par = new PlayerParameters(
            PlayerBodies[playerSelected].Body,
            PlayerBodies[playerSelected].role,
            PlayerBodies[playerSelected].RoleName,
            PersonalName,
            damageMultiplier * dm,
            health * hm);
        return par;
    }

    public void OpenRespawnMenu()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (Content.transform.childCount > 0)
        {

            foreach(Transform c in Content.transform)
            {
                Destroy(c.gameObject);
            }
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
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        if (GameManager.instance.Player != null)
        {
            GameManager.instance.Player.SetActive(false);
        }

        RespawnMenu.SetActive(false);
        if (GameManager.instance.SoulSucker != null)
        {
            Destroy(GameManager.instance.SoulSucker.gameObject);
        }
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        GameManager.instance.GetComponent<MonsterSpawner>().WaveLost();
        GameManager.instance.GetComponent<TownManager>().TownCamera.gameObject.SetActive(true);
        GameManager.instance.GetComponent<TownManager>().UpdateSoulCount();
        GameManager.instance.CantUseTown = false;
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
        GameManager.instance.Wall.GetComponent<TheWall>().SetUpAllCooldowns(false);
    }
}

public class PlayerParameters
{
    public enum ClassRole { Warrior, Rogue, Mage};
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