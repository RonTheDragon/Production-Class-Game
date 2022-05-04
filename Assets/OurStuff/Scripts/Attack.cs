using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack : MonoBehaviour
{
    public float Damage;
    public float Knock;
    public float minStagger;
    public float maxStagger;
    public LayerMask Attackable;
}