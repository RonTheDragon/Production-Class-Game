using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SOwallAttack : SOwall
{
    public float Damage = 10;
    public float Knockback = 5;
    public Vector2 Stagger = new Vector2(0, 100);
    public float Temperature = 0;
}
