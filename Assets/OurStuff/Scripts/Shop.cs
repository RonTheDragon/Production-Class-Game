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

    public List<SOupgrade> Upgrades = new List<SOupgrade>();
    List<Upgrade> _upgrades = new List<Upgrade>();

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
        }
    }

    void Update()
    {
        
    }

    void CreateUpgrades()
    {
        foreach(Transform t in UpgradeContext.transform)
        {
            Destroy(t.gameObject);
        }
        foreach(SOupgrade u in Upgrades)
        {
            Upgrade Up = new Upgrade(u.name,u.Explanation,u.Icon,u.Prices,u.Upgrading);
            _upgrades.Add(Up);
            MakeUpgrade(Up);
        }
    }

    void RefreshUprades()
    {
        foreach (Transform t in UpgradeContext.transform)
        {
            Destroy(t.gameObject);
        }
        foreach (Upgrade u in _upgrades)
        {
            MakeUpgrade(u);
        }
    }

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
}



public class Upgrade
{
    public string Name;
    public string Explanation;
    public Sprite Icon;
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