using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WallShop : Shop
{
    public List<SObuyWallAbility> WallAbilities = new List<SObuyWallAbility>();
    List<WallAbility> _wallAbilities = new List<WallAbility>();

    new void Start()
    {
        base.Start();
        if (OwningShop != null)
        {           
            CreateWallAbility();
        }
    }

    void CreateWallAbility()
    {
        ClearChilds(ShopContext.transform);
        foreach (SObuyWallAbility u in WallAbilities)
        {
            WallAbility Up = new WallAbility(u.Name, u.Explanation, u.ability.Image, u.Price, u.ability);
            _wallAbilities.Add(Up);
            MakeWallAbility(Up);
        }
    }

    void RefreshWallAbility()
    {
        ClearChilds(ShopContext.transform);

        foreach (WallAbility u in _wallAbilities)
        {
            MakeWallAbility(u);
        }
    }

    void MakeWallAbility(WallAbility Up)
    {
        GameObject i = Instantiate(GameManager.instance.WallShopItem, transform.position, ShopContext.transform.rotation, ShopContext.transform);
        i.transform.GetChild(0).GetComponent<TMP_Text>().text = Up.Name;
        i.transform.GetChild(1).GetComponent<TMP_Text>().text = Up.Explanation;
        i.transform.GetChild(2).GetComponent<Image>().sprite = Up.Icon;
        i.transform.GetChild(3).GetComponent<TMP_Text>().text = $"Price: {Up.Price}";
        ShopWallAttack SUP = i.GetComponent<ShopWallAttack>();
        SUP.shop = this;
        SUP.wa = Up;
    }

    void MakeEquipableWallAbility(WallAbility Up)
    {
        GameObject i = Instantiate(GameManager.instance.WallBaughtItem, transform.position, EquipContext.transform.rotation, EquipContext.transform);
        i.GetComponent<Image>().sprite = Up.Icon;
        DDwallAbility dd = i.GetComponent<DDwallAbility>();
        dd.shop = this;
        dd.ability = Up.TheAbility;
    }

    public void BuyWallAbility(WallAbility Up)
    {
        if (GameManager.instance.SoulEnergy >= Up.Price)
        {
            GameManager.instance.SoulEnergy -= Up.Price;
            GameManager.instance.GetComponent<TownManager>().UpdateSoulCount();

            MakeEquipableWallAbility(Up);

            _wallAbilities.Remove(Up);

            RefreshWallAbility();
        }
    }
}
public class WallAbility : ShopBuyable
{
    public SOwall TheAbility;
    public WallAbility(string Name, string Explanation, Sprite Icon, int Price, SOwall TheAbility)
    {
        this.Name = Name;
        this.Explanation = Explanation;
        this.Icon = Icon;
        this.Price = Price;
        this.TheAbility = TheAbility;
    }
}
