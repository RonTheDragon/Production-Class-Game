using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ShopProduct : MonoBehaviour
{
    public Shop shop;
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }
    public abstract void OnClick();

}
