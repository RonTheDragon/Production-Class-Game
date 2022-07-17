using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public abstract class Shop : MonoBehaviour, ItownClickable
{
    [HideInInspector] public enum UpgradesList { SelectUpgradeHere, WallHp, WallDamage, WallHealing, WallCooldowns, //Wall
        WarriorHp, WarriorDamage, WarriorStamina, WarriorSpeed, WarriorRegen,                                         //Warrior
        RogueHp, RogueDamage, RogueStamina, RogueSpeed, RogueRegen,                                                   //Rogue
        MageHp, MageDamage, MageStamina, MageSpeed, MageRegen,                                                        //Mage
        RemnantMagnet, RemnantBlast, SoulWorth, TwinSouls, SoulHeal, SoulEnvigoration, ElementEfficiency              //Remnant
    }
    public GameObject OwningShop;
    protected GameObject UpgradeContext;
    protected GameObject ShopContext;
    [HideInInspector] public GameObject EquipContext;

    public List<SOupgrade> Upgrades = new List<SOupgrade>();
    protected List<Upgrade> _upgrades = new List<Upgrade>();


    [SerializeField] GameObject ShowWhenUsed;
    [SerializeField] string TextToShow;
    float hovered;

    [SerializeField] float mult = 0.3f;


    //public List<>

    public void OnClicked()
    {
        if (OwningShop != null)
        {
            OwningShop.SetActive(true);
            GameManager.instance.Shopping=true;
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
            Upgrade Up = new Upgrade(u.Name,u.Explanation,u.Icon,u.Prices,u.Upgrading);
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
                    GameManager.instance.WarriorHealthMultiplier  += mult;
                    break;
                case UpgradesList.WarriorDamage:
                    GameManager.instance.WarriorDamageMultiplier  += mult;
                    break;
                case UpgradesList.WarriorStamina:
                    GameManager.instance.WarriorStaminaMultiplier += mult;
                    break;
                case UpgradesList.WarriorSpeed:
                    GameManager.instance.WarriorSpeedMultiplier   += mult/2;
                    break;
                case UpgradesList.WarriorRegen:
                    GameManager.instance.WarriorRegenMultiplier   += mult;
                    break;
                case UpgradesList.RogueHp:
                    GameManager.instance.RogueHealthMultiplier    += mult;
                    break;
                case UpgradesList.RogueDamage:
                    GameManager.instance.RogueDamageMultiplier    += mult;
                    break;
                case UpgradesList.RogueStamina:
                    GameManager.instance.RogueStaminaMultiplier   += mult;
                    break;
                case UpgradesList.RogueSpeed:
                    GameManager.instance.RogueSpeedMultiplier     += mult / 2;
                    break;
                case UpgradesList.RogueRegen:
                    GameManager.instance.RogueRegenMultiplier     += mult;
                    break;
                case UpgradesList.MageHp:
                    GameManager.instance.MageHealthMultiplier     += mult;
                    break;
                case UpgradesList.MageDamage:
                    GameManager.instance.MageDamageMultiplier     += mult;
                    break;
                case UpgradesList.MageStamina:
                    GameManager.instance.MageStaminaMultiplier    += mult;
                    break;
                case UpgradesList.MageSpeed:
                    GameManager.instance.MageSpeedMultiplier      += mult / 2;
                    break;
                case UpgradesList.MageRegen:
                    GameManager.instance.MageRegenMultiplier      += mult;
                    break;
                case UpgradesList.ElementEfficiency:
                    GameManager.instance.ElementEfficiency        += mult;
                    GameManager.instance.ElementRecovery          -= mult / 2;
                    break;
                case UpgradesList.RemnantBlast:
                    GameManager.instance.RemnantBlastRadius       += 2;
                    GameManager.instance.RemnantBlastDamage       += mult * 150;
                    break;
                case UpgradesList.RemnantMagnet:
                    GameManager.instance.RemnantMagnet            += mult * 2;
                    break;
                case UpgradesList.SoulEnvigoration:
                    GameManager.instance.SoulEnvigoration         += 5;
                    break;
                case UpgradesList.SoulHeal:
                    GameManager.instance.SoulHeal                 += 3;
                    break;
                case UpgradesList.SoulWorth:
                    GameManager.instance.SoulWorth                += mult;
                    break;
                case UpgradesList.TwinSouls:
                    GameManager.instance.TwinSouls                += 8;
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


