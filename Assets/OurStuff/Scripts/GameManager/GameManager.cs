using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private AudioListener listener;
    public enum ShowEnemyData {Never, Always, AfterBeingDamaged}
    [HideInInspector] public static GameManager   instance;
    [HideInInspector] public        GameObject    Player;
                      public        GameObject    Wall;
                      public        GameObject    PowerStone;
                      public        float         EnemeiesTryToAttackEvery = 0.1f;
                      public        ShowEnemyData Data                     = ShowEnemyData.Never;
                      public        bool          PlayerKillingShortCut    = false;
                      public        LayerMask     enemiesCanSee;
                      public        LayerMask     AlliesCanSee;
                      public        LayerMask     enemiesCanAttack;
                      public        LayerMask     PlayerCanAttack;
                      public        LayerMask     LayerMaskOfEverything;
    [HideInInspector] public        bool          WallFacingZ;
    [HideInInspector] public        float         WallLength;
                      public        GameObject    CooldownCircleObject;
    [HideInInspector] public        bool          Shopping;
                      public        GameObject    Upgrade;
                      public        GameObject    WallShopItem;
                      public        GameObject    WallBaughtItem;
                      public        GameObject    ClassShopItem;
                      public        GameObject    ClassBaughtItem;
    [HideInInspector] public        Transform     SoulSucker;
                      public        int           SoulEnergy;
    [HideInInspector] public        bool          AlreadyWon;


    [HideInInspector] public        float         RogueDamageMultiplier    = 1;
    [HideInInspector] public        float         MageDamageMultiplier     = 1;
    [HideInInspector] public        float         WarriorDamageMultiplier  = 1;

    [HideInInspector] public        float         RogueHealthMultiplier    = 1;
    [HideInInspector] public        float         MageHealthMultiplier     = 1;
    [HideInInspector] public        float         WarriorHealthMultiplier  = 1;

    [HideInInspector] public        float         RogueStaminaMultiplier   = 1;
    [HideInInspector] public        float         MageStaminaMultiplier    = 1;
    [HideInInspector] public        float         WarriorStaminaMultiplier = 1;

    [HideInInspector] public        float         RogueSpeedMultiplier     = 1;
    [HideInInspector] public        float         MageSpeedMultiplier      = 1;
    [HideInInspector] public        float         WarriorSpeedMultiplier   = 1;

    [HideInInspector] public        float         RogueRegenMultiplier     = 1;
    [HideInInspector] public        float         MageRegenMultiplier      = 1;
    [HideInInspector] public        float         WarriorRegenMultiplier   = 1;

    [HideInInspector] public        float         ElementRecovery          = 1;
    [HideInInspector] public        float         ElementEfficiency        = 1;
    [HideInInspector] public        float         RemnantBlastDamage       = 0;
    [HideInInspector] public        float         RemnantBlastRadius       = 0;
    [HideInInspector] public        float         RemnantMagnet            = 0;
    [HideInInspector] public        float         SoulEnvigoration         = 0;
    [HideInInspector] public        float         SoulHeal                 = 0;
    [HideInInspector] public        float         SoulWorth                = 1;
    [HideInInspector] public        float         TwinSouls                = 0;

    private void Awake()
    {
        instance = this;
        WallFacingZ = Wall.GetComponent<TheWall>().WallFacingZ;
        WallLength = Wall.GetComponent<TheWall>().WallLength;
        listener = GetComponent<AudioListener>();
    }

    public void Update()
    {

        if (Player == null)
        {
          //  if (listener.enabled == false)
            {
                listener.enabled = true;
            }
        }
        else if (!Player.activeSelf)
        {
            //  if (listener.enabled == false)
            {
                listener.enabled = true;
            }
        }
        else
        {
         //   if (listener.enabled == true)
            {
                listener.enabled = false;
            }
        }
    }
    public void QuitGame()
    {
        // save any game data here
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        //UnityEditor.EditorApplication.isPaused = true;
        UnityEditor.EditorApplication.isPlaying = false;
#else
             Application.Quit();
#endif
    }

    public void TurnSOclassToAttackList(List<SOability> attacklist, SOclass role)
    {
        foreach (SOability s in role.DownLeftClickAttacks)
        {
            attacklist.Add(s);
        }
        foreach (SOability s in role.UpLeftClickAttacks)
        {
            attacklist.Add(s);
        }
        foreach (SOability s in role.DownRightClickAttacks)
        {
            attacklist.Add(s);
        }
        foreach (SOability s in role.UpRightClickAttacks)
        {
            attacklist.Add(s);
        }
        foreach (SOability s in role.LeftClickAttacks)
        {
            attacklist.Add(s);
        }
        foreach (SOability s in role.RightClickAttacks)
        {
            attacklist.Add(s);
        }
        foreach (SOability s in role.SpaceAbility)
        {
            attacklist.Add(s);
        }
        foreach (SOability s in role.F_Ability)
        {
            attacklist.Add(s);
        }
    }
}
