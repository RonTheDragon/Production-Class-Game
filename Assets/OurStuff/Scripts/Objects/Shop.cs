using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public abstract class Shop : MonoBehaviour, ItownClickable
{
    [HideInInspector] public enum UpgradesList { SelectUpgradeHere , WallHp , WallDamage, WallHealing, WallCooldowns,
        WarriorHp , WarriorDamage , WarriorStamina , WarriorSpeed , WarriorRegan,
        RogueHp , RogueDamage , RogueStamina , RogueSpeed, RogueRegan,
        MageHp, MageDamage, MageStamina , MageSpeed, MageRegan}
    public GameObject OwningShop;
    protected GameObject UpgradeContext;
    protected GameObject ShopContext;
    [HideInInspector] public GameObject EquipContext;

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
            ShopContext = OwningShop.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).gameObject;
            EquipContext = OwningShop.transform.GetChild(0).GetChild(2).GetChild(0).GetChild(0).gameObject;
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
                case UpgradesList.WallDamage:
                    GameManager.instance.Wall.GetComponent<TheWall>().DamageMultiplier += mult;
                    break;
                case UpgradesList.WallHealing:
                    GameManager.instance.Wall.GetComponent<TheWall>().HealingMultiplier += mult;
                    break;
                case UpgradesList.WallCooldowns:
                    GameManager.instance.Wall.GetComponent<TheWall>().CooldownMultiplier -= 0.15f;
                    break;
                case UpgradesList.WarriorHp:
                    GameManager.instance.WarriorHealthMultiplier += mult;
                    break;
                case UpgradesList.WarriorDamage:
                    GameManager.instance.WarriorDamageMultiplier += mult;
                    break;
                case UpgradesList.WarriorStamina:
                    GameManager.instance.WarriorStaminaMultiplier += mult;
                    break;
                case UpgradesList.WarriorSpeed:
                    GameManager.instance.WarriorSpeedMultiplier += mult/2;
                    break;
                case UpgradesList.WarriorRegan:
                    GameManager.instance.WarriorReganMultiplier += mult;
                    break;
                case UpgradesList.RogueHp:
                    GameManager.instance.RogueHealthMultiplier += mult;
                    break;
                case UpgradesList.RogueDamage:
                    GameManager.instance.RogueDamageMultiplier += mult;
                    break;
                case UpgradesList.RogueStamina:
                    GameManager.instance.RogueStaminaMultiplier += mult;
                    break;
                case UpgradesList.RogueSpeed:
                    GameManager.instance.RogueSpeedMultiplier += mult / 2;
                    break;
                case UpgradesList.RogueRegan:
                    GameManager.instance.RogueReganMultiplier += mult;
                    break;
                case UpgradesList.MageHp:
                    GameManager.instance.MageHealthMultiplier += mult;
                    break;
                case UpgradesList.MageDamage:
                    GameManager.instance.MageDamageMultiplier += mult;
                    break;
                case UpgradesList.MageStamina:
                    GameManager.instance.MageStaminaMultiplier += mult;
                    break;
                case UpgradesList.MageSpeed:
                    GameManager.instance.MageSpeedMultiplier += mult / 2;
                    break;
                case UpgradesList.MageRegan:
                    GameManager.instance.MageReganMultiplier += mult;
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


