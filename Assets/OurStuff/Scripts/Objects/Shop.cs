using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public abstract class Shop : MonoBehaviour, ItownClickable
{
    [HideInInspector] public enum UpgradesList { Select_Upgrade_Here , WallHp , WarriorHp , WarriorDamage , RogueHp , RogueDamage , MageHp, MageDamage}
    public GameObject OwningShop;
    protected GameObject UpgradeContext;

    public List<SOupgrade> Upgrades = new List<SOupgrade>();
    protected List<Upgrade> _upgrades = new List<Upgrade>();


    [SerializeField] GameObject ShowWhenUsed;
    [SerializeField] string TextToShow;
    float hovered;



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

    protected void Start()
    {
        if (OwningShop != null)
        {
            UpgradeContext = OwningShop.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject;
            CreateUpgrades();
        }
    }

    void Update()
    {
        if (hovered > 0)
            hovered -= Time.deltaTime;
        else
            ShowWhenUsed.SetActive(false);
    }

    protected void ClearChilds(Transform T)
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

    //Refreshes 
    void RefreshUprades()
    {
        ClearChilds(UpgradeContext.transform);

        foreach (Upgrade u in _upgrades)
        {
            MakeUpgrade(u);
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



    //Uses
    public void UseUpgrade(Upgrade Up)
    {
        if (Up.Prices.Length > Up.lvl-1 && GameManager.instance.SoulEnergy>=Up.Prices[Up.lvl - 1])
        {    
            GameManager.instance.SoulEnergy -= Up.Prices[Up.lvl - 1];
            Up.lvl++;
            RefreshUprades();
            GameManager.instance.GetComponent<TownManager>().UpdateSoulCount();
            float mult = 0.3f;
            switch (Up.Upgrading)
            {
                case UpgradesList.WallHp:
                    GameManager.instance.Wall.GetComponent<WallHealth>().MaxHp += 100;
                    break;
                case UpgradesList.WarriorHp:
                    GameManager.instance.WarriorHealthMultiplier += mult;
                    break;
                case UpgradesList.WarriorDamage:
                    GameManager.instance.WarriorDamageMultiplier += mult;
                    break;
                case UpgradesList.RogueHp:
                    GameManager.instance.RogueHealthMultiplier += mult;
                    break;
                case UpgradesList.RogueDamage:
                    GameManager.instance.RogueDamageMultiplier += mult;
                    break;
                case UpgradesList.MageHp:
                    GameManager.instance.MageHealthMultiplier += mult;
                    break;
                case UpgradesList.MageDamage:
                    GameManager.instance.MageDamageMultiplier += mult;
                    break;
            }
        }
    }

    public string OnHover()
    {
        ShowWhenUsed.SetActive(true);
        hovered = 0.01f;
        return TextToShow;
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
