using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuyableClass", menuName = "Shop/buyableClass")]
public class SObuyClass : ScriptableObject
{
    public string Name;
    public string Explanation;
    public string LongExplanation;
    public int Price;
    public SOPlayerBody Body;
}
