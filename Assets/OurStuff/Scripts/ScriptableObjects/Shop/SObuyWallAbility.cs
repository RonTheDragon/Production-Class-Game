using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuyableWallAbility", menuName = "Shop/WallAbility")]
public class SObuyWallAbility : ScriptableObject
{
    public string Name;
    public string Explanation;
    public int Price;
    public SOwall ability;
}
