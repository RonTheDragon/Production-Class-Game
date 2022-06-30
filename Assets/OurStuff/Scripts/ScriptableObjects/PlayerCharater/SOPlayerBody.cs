using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "PlayerBody", menuName = "PlayerBody")]
public class SOPlayerBody : ScriptableObject
{
    public string     RoleName;
    public Sprite      Image; 
    public GameObject Body;
    public SOclass    role;
    public Vector2    damageMultiplier;
    public Vector2    health;
}
