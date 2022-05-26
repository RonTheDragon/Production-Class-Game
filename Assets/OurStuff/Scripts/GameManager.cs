using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public static GameManager instance;
    public        GameObject  Player;
    public        GameObject  Wall;
    public        GameObject  PowerStone;
    public        LayerMask   enemiesCanSee;
    public        LayerMask   enemiesCanAttack;
    public        LayerMask   PlayerCanAttack;

    private void Awake()
    {
        instance = this;
    }
}
