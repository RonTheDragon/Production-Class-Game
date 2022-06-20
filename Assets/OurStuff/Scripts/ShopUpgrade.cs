using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUpgrade : MonoBehaviour
{
    public Shop shop;
    public Upgrade upgrade;
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }
    public void OnClick()
    {
        shop.UseUpgrade(upgrade);
    }
}
