using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class DDwallAbility : DragAndDropItem 
{
    public Shop shop;
    public SOwall ability;

    public override void LostGrip()
    {
        if (InSlot!=null)
        {
            WallAbilitySlot slot = InSlot.GetComponent<WallAbilitySlot>();
            GameManager.instance.Wall.GetComponent<TheWall>().TheWallAttacks[slot.SlotNum] = null;
            if (shop is WallShop)
            {
                WallShop WS = (WallShop)shop;
                transform.SetParent(WS.EquipContext.transform);
            }
            slot.HeldItem = null;
            InSlot = null;
        }
    }
    public void Baught()
    {
        canvas = transform.parent.parent.parent.parent.parent.parent.GetComponent<Canvas>();
        Transform ItemSlots = transform.parent.parent.parent.GetChild(1);
        int n = 0;
        foreach(SOwall s in GameManager.instance.Wall.GetComponent<TheWall>().TheWallAttacks)
        {
            if (s == null)
            {
                WallAbilitySlot w = ItemSlots.GetChild(n).GetComponent<WallAbilitySlot>();
                w.AutoSet(gameObject);
                break;
            }
            n++;
        }

    }
}
