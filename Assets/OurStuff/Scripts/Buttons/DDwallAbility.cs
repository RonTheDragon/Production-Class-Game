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
            transform.SetParent(shop.EquipContext.transform);
            slot.HeldItem = null;
            InSlot = null;
        }
    }

    void Start()
    {
        canvas = transform.parent.parent.parent.parent.parent.parent.GetComponent<Canvas>();
    }
}
