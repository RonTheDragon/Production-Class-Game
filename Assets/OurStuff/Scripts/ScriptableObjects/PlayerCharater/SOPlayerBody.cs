using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerBody", menuName = "PlayerBody")]
public class SOPlayerBody : ScriptableObject
{
    public GameObject Body;
    public SOclass    role;
    public Vector2    damageMultiplier;
    public Vector2    health;
}
