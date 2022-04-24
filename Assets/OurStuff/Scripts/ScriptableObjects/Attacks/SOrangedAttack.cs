using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RangeAttack", menuName = "Attacks/Ranged Attack")]
public class SOrangedAttack : SOattack
{
    public string Projectile;
    public float ProjectileSpeed;
}

