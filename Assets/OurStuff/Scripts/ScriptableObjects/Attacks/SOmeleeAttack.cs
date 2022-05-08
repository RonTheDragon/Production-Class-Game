using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeAttack", menuName = "Attacks/Melee Attack")]
public class SOmeleeAttack : SOattack
{
    public float DamagingCooldown = 1;
    public float AttackMovementForce;
    public Vector2 AttackMovementDirection;
}
