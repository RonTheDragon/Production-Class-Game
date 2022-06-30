using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopClass : ShopProduct
{
    [HideInInspector] public BuyableClass Bc;
    public override void OnClick()
    {
        if (shop is ClassShop)
        {
            ClassShop CS = (ClassShop)shop;
            CS.BuyClass(Bc);
        }
    }
}
