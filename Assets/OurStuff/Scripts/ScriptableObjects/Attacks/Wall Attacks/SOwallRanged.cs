using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WallRangeAttack", menuName = "Combat/Wall/Ranged")]
public class SOwallRanged : SOwallAttack
{
    public string Projectile;
    public float ProjectileSpeed;
    public int AmountOfShoots;
}
