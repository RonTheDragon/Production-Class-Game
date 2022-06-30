using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum ShowEnemyData {Never, Always, AfterBeingDamaged}
    [HideInInspector]
    public static GameManager   instance;
    [HideInInspector] public        GameObject    Player;
    public        GameObject    Wall;
    public        GameObject    PowerStone;
    public        float         EnemeiesTryToAttackEvery = 0.1f;
    public        ShowEnemyData Data                     = ShowEnemyData.Never;
    public        bool          PlayerKillingShortCut    = false;
    public        LayerMask     enemiesCanSee;
    public        LayerMask     enemiesCanAttack;
    public        LayerMask     PlayerCanAttack;
    public        LayerMask     LayerMaskOfEverything;
    [HideInInspector] public bool WallFacingZ;
    [HideInInspector] public float WallLength;
    public        GameObject    CooldownCircleObject;
    [HideInInspector] public bool CantUseTown;
    public GameObject Upgrade;
    public GameObject WallShopItem;
    public GameObject WallBaughtItem;
    public GameObject ClassShopItem;
    public GameObject ClassBaughtItem;
    [HideInInspector] public Transform SoulSucker;
    public int SoulEnergy;
    [HideInInspector] public bool AlreadyWon;

    [HideInInspector] public float RogueDamageMultiplier = 1;
    [HideInInspector] public float MageDamageMultiplier = 1;
    [HideInInspector] public float WarriorDamageMultiplier = 1;

    [HideInInspector] public float RogueHealthMultiplier = 1;
    [HideInInspector] public float MageHealthMultiplier = 1;
    [HideInInspector] public float WarriorHealthMultiplier = 1;

    [HideInInspector] public float RogueStaminaMultiplier = 1;
    [HideInInspector] public float MageStaminaMultiplier = 1;
    [HideInInspector] public float WarriorStaminaMultiplier = 1;

    [HideInInspector] public float RogueSpeedMultiplier = 1;
    [HideInInspector] public float MageSpeedMultiplier = 1;
    [HideInInspector] public float WarriorSpeedMultiplier = 1;

    [HideInInspector] public float RogueReganMultiplier = 1;
    [HideInInspector] public float MageReganMultiplier = 1;
    [HideInInspector] public float WarriorReganMultiplier = 1;

    private void Awake()
    {
        instance = this;
        WallFacingZ = Wall.GetComponent<TheWall>().WallFacingZ;
        WallLength = Wall.GetComponent<TheWall>().WallLength;
    }

    public void Shopping(bool b)
    {
        CantUseTown = b;
    }
    public void Update()
    {
       if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }
    public void QuitGame()
    {
        // save any game data here
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPaused = true;
#else
             Application.Quit();
#endif
    }
}
