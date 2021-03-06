using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack : Ability
{
    public float Damage;
    public float Knock;
    public Vector2 Stagger;
    public float Temperature;
    [HideInInspector] public LayerMask Attackable;
    [HideInInspector] public float Charge;
    [HideInInspector] public GameObject Attacker;
}