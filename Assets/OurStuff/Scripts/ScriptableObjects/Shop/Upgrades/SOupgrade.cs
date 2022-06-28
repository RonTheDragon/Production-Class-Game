using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "upgrade", menuName = "Shop/Upgrade")]
public class SOupgrade : ScriptableObject
{
    public string Name;
    public string Explanation;
    public Sprite Icon;
    public int[] Prices;
    public Shop.UpgradesList Upgrading;

}
