using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class SOattack : SOability
{
    public float Damage = 10;
    public float Knockback = 5;
    public float minStagger = 0;
    public float maxStagger = 100;
}