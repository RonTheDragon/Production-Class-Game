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
    [HideInInspector] public bool WallFacingZ;
    [HideInInspector] public float WallLength;

    private void Awake()
    {
        instance = this;
        WallFacingZ = Wall.GetComponent<TheWall>().WallFacingZ;
        WallLength = Wall.GetComponent<TheWall>().WallLength;
    }
}
