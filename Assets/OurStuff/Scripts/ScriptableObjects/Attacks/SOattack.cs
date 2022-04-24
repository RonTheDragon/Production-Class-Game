using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Attack",menuName ="Attacks/Attack")]
public class SOattack : ScriptableObject
{
    public string Name;
    public string AnimationTrigger;
    public float Damage = 10;
    public float Knockback = 10000;
    public float StaminaCost = 10;
    public float AttackCooldown = 1;
    public float MinRange = 0;
    public float MaxRange = 3;
    public float Chance = 100;
    public UnityEvent AttackMethod;
}