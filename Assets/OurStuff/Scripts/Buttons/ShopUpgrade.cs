using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUpgrade : ShopProduct
{
    public Upgrade upgrade;
    public override void OnClick()
    {
        shop.UseUpgrade(upgrade);
    }
}
