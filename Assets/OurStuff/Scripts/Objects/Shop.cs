using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Shop : MonoBehaviour, ItownClickable
{
    [HideInInspector] public enum UpgradesList { Select_Upgrade_Here , WallHp }
    public GameObject OwningShop;
    GameObject UpgradeContext;
    GameObject ShopContext;
    [HideInInspector] public GameObject EquipContext;

    public List<SOupgrade> Upgrades = new List<SOupgrade>();
    List<Upgrade> _upgrades = new List<Upgrade>();

    public List<SObuyWallAbility> WallAbilities = new List<SObuyWallAbility>();
    List<WallAbility> _wallAbilities = new List<WallAbility>();

    

    //public List<>

    public void OnClicked()
    {
        if (OwningShop != null)
        {
            OwningShop.SetActive(true);
            GameManager.instance.Shopping(true);
        }
        else { Debug.Log("Missing a Shop"); }
    }

    void Start()
    {
        if (OwningShop != null)
        {
            UpgradeContext = OwningShop.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject;
            CreateUpgrades();
            ShopContext = OwningShop.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).gameObject;
            CreateWallAbility();
            EquipContext = OwningShop.transform.GetChild(0).GetChild(2).GetChild(0).GetChild(0).gameObject;

        }
    }

    void Update()
    {
        
    }

    void ClearChilds(Transform T)
    {
        foreach (Transform t in T)
        {
            Destroy(t.gameObject);
        }
    }

    //Creates 
    void CreateUpgrades()
    {
        ClearChilds(UpgradeContext.transform);
        foreach (SOupgrade u in Upgrades)
        {
            Upgrade Up = new Upgrade(u.name,u.Explanation,u.Icon,u.Prices,u.Upgrading);
            _upgrades.Add(Up);
            MakeUpgrade(Up);
        }
    }
    void CreateWallAbility()
    {
        ClearChilds(ShopContext.transform);
        foreach (SObuyWallAbility u in WallAbilities)
        {
            WallAbility Up = new WallAbility(u.name, u.Explanation, u.ability.Image, u.Price, u.ability);
            _wallAbilities.Add(Up);
            MakeWallAbility(Up);
        }
    }

    //Refreshes 
    void RefreshUprades()
    {
        ClearChilds(UpgradeContext.transform);

        foreach (Upgrade u in _upgrades)
        {
            MakeUpgrade(u);
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
    
    //Makes
    void MakeUpgrade(Upgrade Up)
    {
        GameObject i = Instantiate(GameManager.instance.Upgrade, transform.position, UpgradeContext.transform.rotation, UpgradeContext.transform);
        i.transform.GetChild(0).GetComponent<TMP_Text>().text = Up.Name;
        i.transform.GetChild(1).GetComponent<TMP_Text>().text = Up.Explanation;
        i.transform.GetChild(2).GetComponent<Image>().sprite = Up.Icon;
        if (Up.Prices.Length > Up.lvl - 1)
        i.transform.GetChild(3).GetComponent<TMP_Text>().text = $"Price: {Up.Prices[Up.lvl - 1]}";
        else
        i.transform.GetChild(3).GetComponent<TMP_Text>().text = $"MAXED";
        i.transform.GetChild(4).GetComponent<TMP_Text>().text = $"LVL: {Up.lvl}";
        ShopUpgrade SUP = i.GetComponent<ShopUpgrade>();
        SUP.shop = this;
        SUP.upgrade = Up;
    }

    void MakeWallAbility(WallAbility Up)
    {
        GameObject i = Instantiate(GameManager.instance.ShopItem, transform.position, ShopContext.transform.rotation, ShopContext.transform);
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
        GameObject i = Instantiate(GameManager.instance.BaughtItem, transform.position, EquipContext.transform.rotation, EquipContext.transform);
        i.GetComponent<Image>().sprite = Up.Icon;
        DDwallAbility dd = i.GetComponent<DDwallAbility>();
        dd.shop = this;
        dd.ability = Up.TheAbility;
    }

    //Uses
    public void UseUpgrade(Upgrade Up)
    {
        if (Up.Prices.Length > Up.lvl-1 && GameManager.instance.SoulEnergy>=Up.Prices[Up.lvl - 1])
        {    
            GameManager.instance.SoulEnergy -= Up.Prices[Up.lvl - 1];
            Up.lvl++;
            RefreshUprades();
            GameManager.instance.GetComponent<TownManager>().UpdateSoulCount();
            switch (Up.Upgrading)
            {
                case UpgradesList.WallHp:
                    GameManager.instance.Wall.GetComponent<WallHealth>().MaxHp += 100;
                    break;
            }
        }
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


public abstract class ShopItem
{
    public string Name;
    public string Explanation;
    public Sprite Icon;
}

public class Upgrade : ShopItem
{
    public int[] Prices;
    public int lvl;
    public Shop.UpgradesList Upgrading;
    public Upgrade(string Name, string Explanation, Sprite Icon, int[] Prices, Shop.UpgradesList Upgrading)
    {
        this.Name = Name;
        this.Explanation = Explanation;
        this.Icon = Icon;
        this.Prices = Prices;
        this.lvl = 1;
        this.Upgrading = Upgrading;
    }
}

public abstract class ShopBuyable : ShopItem
{
    public int Price;
}

public class WallAbility : ShopBuyable
{
    public SOwall TheAbility;
    public WallAbility(string Name, string Explanation, Sprite Icon,int Price,SOwall TheAbility)
    {
        this.Name = Name;
        this.Explanation = Explanation;
        this.Icon = Icon;
        this.Price = Price;
        this.TheAbility = TheAbility;
    }
}
