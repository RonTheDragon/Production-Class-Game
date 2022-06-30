using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopWallAttack : ShopProduct
{
    [HideInInspector] public WallAbility wa;
    public override void OnClick()
    {
        if (shop is WallShop)
        {
            WallShop WS = (WallShop)shop;
            WS.BuyWallAbility(wa);
        }
    }
}
