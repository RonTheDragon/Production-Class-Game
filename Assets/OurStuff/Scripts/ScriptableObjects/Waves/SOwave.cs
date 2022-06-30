using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "Waves/Monster Wave")]
public class SOwave : ScriptableObject
{
    public List<MonsterWave> Monsters;
    public Vector2 TimeBetweenSpawns = new Vector2(1, 3);
    public Vector2 TimeTillNextWave = new Vector2(30,60);
    public Vector2 TooManyMonstersToStart = new Vector2(5, 10);
    public Vector2 HealthMultiplier = new Vector2(1, 1);
    public Vector2 DamageMultiplier = new Vector2(1, 1);
}
[System.Serializable]
public class MonsterWave
{
    public string Name;
    public Vector2 Amount;
}
