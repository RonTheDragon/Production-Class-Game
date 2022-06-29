using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Round", menuName = "Waves/Level")]
public class SOlevel : ScriptableObject
{
    public List<SOwave> Waves = new List<SOwave>();
    public int PlayerCharacterAmount = 3;
    public float DifficultyMultiplier = 1;
}
