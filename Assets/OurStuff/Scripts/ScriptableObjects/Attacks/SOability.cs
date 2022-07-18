using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SOability : ScriptableObject
{
    public string Name;
    public string AnimationTrigger;
    public string PreviousAttack;
    public float StaminaCost     = 10;
    public float AttackCooldown  = 1;
    public float AbilityCooldown = 0;
    public bool AlwaysShow;
    public bool UnStopable;
    public float MinRange        = 0;
    public float MaxRange        = 3;
    public float Chance          = 100;
    public enum AttackType { Normal, NeedsRelease, CanRelease , Combo, StartCombo}
    public      AttackType attackType = AttackType.Normal;
    public bool aiming;
    public Sprite Image;
    public string sound;
}
