using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SOwall : ScriptableObject
{
    public string Name;
    public float AbilityCooldown = 0;
    public Sprite Image;
    public TheWall.WallAttacks wallAttack = TheWall.WallAttacks.None;
}
