using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WallRangeAttack", menuName = "Combat/Attacks/Wall/Ranged")]
public class SOwallRanged : SOwall
{
    public string Projectile;
    public float ProjectileSpeed;
    public int AmountOfShoots;
}
