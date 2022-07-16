using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ClassShop : Shop
{
    public List<SObuyClass> Classes = new List<SObuyClass>();
    List<BuyableClass> _classes = new List<BuyableClass>();

    new void Start()
    {
        base.Start();
        if (OwningShop != null)
        {
            CreateClass();
        }
    }

    void CreateClass()
    {
        ClearChilds(ShopContext.transform);
        foreach (SObuyClass u in Classes)
        {
            BuyableClass Up = new BuyableClass(u.Name, u.Explanation, u.Body.Image, u.Price, u.Body);
            _classes.Add(Up);
            MakeClass(Up);
        }
    }

    void RefreshClass()
    {
        ClearChilds(ShopContext.transform);

        foreach (BuyableClass u in _classes)
        {
            MakeClass(u);
        }
    }

    void MakeClass(BuyableClass Up)
    {
        GameObject i = Instantiate(GameManager.instance.ClassShopItem, transform.position, ShopContext.transform.rotation, ShopContext.transform);
        i.transform.GetChild(0).GetComponent<TMP_Text>().text = Up.Name;
        i.transform.GetChild(1).GetComponent<TMP_Text>().text = Up.Explanation;
        i.transform.GetChild(2).GetComponent<Image>().sprite = Up.Icon;
        i.transform.GetChild(3).GetComponent<TMP_Text>().text = $"Price: {Up.Price}";
        ShopClass SUP = i.GetComponent<ShopClass>();
        SUP.shop = this;
        SUP.Bc = Up;
        if (Up.Price == 0)
        {
            SUP.OnClick();
        }
    }

    void MakeEquipableClass(BuyableClass Up)
    {
        GameObject i = Instantiate(GameManager.instance.ClassBaughtItem, transform.position, EquipContext.transform.rotation, EquipContext.transform);
        i.GetComponent<Image>().sprite = Up.Icon;
        ClassEnabler CE = i.GetComponent<ClassEnabler>();
        CE.shop = this;
        CE.Body = Up.TheBody;
        CE.OnClick();
    }
    public void BuyClass(BuyableClass Up)
    {
        if (GameManager.instance.SoulEnergy >= Up.Price)
        {
            GameManager.instance.SoulEnergy -= Up.Price;
            GameManager.instance.GetComponent<TownManager>().UpdateSoulCount();

            MakeEquipableClass(Up);

            _classes.Remove(Up);

            RefreshClass();
        }
    }
}
public class BuyableClass : ShopBuyable
{
    public SOPlayerBody TheBody;
    public BuyableClass(string Name, string Explanation, Sprite Icon, int Price, SOPlayerBody TheBody)
    {
        this.Name = Name;
        this.Explanation = Explanation;
        this.Icon = Icon;
        this.Price = Price;
        this.TheBody = TheBody;
    }
}