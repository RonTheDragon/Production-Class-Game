using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopWallAttack : ShopProduct
{
    public WallAbility wa;
    public override void OnClick()
    {
        shop.BuyWallAbility(wa);
    }
}
