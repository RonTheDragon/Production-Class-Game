using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack : MonoBehaviour
{
    public float Damage;
    public float Knock;
    public Vector2 Stagger;
    [HideInInspector] public LayerMask Attackable;
    [HideInInspector] public float Charge;
}