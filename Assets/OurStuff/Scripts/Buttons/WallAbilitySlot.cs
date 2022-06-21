using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WallAbilitySlot : MonoBehaviour , IDropHandler
{
    public int SlotNum;
    public RectTransform HeldItem;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {

            if (HeldItem != eventData.pointerDrag.GetComponent<RectTransform>()) //if held item isnt what already held
            {
                DDwallAbility DD = eventData.pointerDrag.GetComponent<DDwallAbility>(); //gets Item
                if (DD.InSlot != null)
                {
                    if (HeldItem == null)
                    {
                        DD.InSlot.GetComponent<WallAbilitySlot>().HeldItem = null; //makes Item Original slot empty
                        GameManager.instance.Wall.GetComponent<TheWall>().TheWallAttacks[DD.InSlot.GetComponent<WallAbilitySlot>().SlotNum] = null;
                    }
                    else
                    {
                        DD.InSlot.GetComponent<WallAbilitySlot>().HeldItem = HeldItem; //makes the Item Original slot be filled with my old item
                        HeldItem.GetComponent<DDwallAbility>().InSlot = DD.InSlot.GetComponent<RectTransform>(); // makes my old item replace owner 
                        HeldItem.SetParent(DD.InSlot.transform);
                        HeldItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(50, -50);
                        GameManager.instance.Wall.GetComponent<TheWall>().TheWallAttacks[DD.InSlot.GetComponent<WallAbilitySlot>().SlotNum] = HeldItem.GetComponent<DDwallAbility>().ability;
                    }
                }
                else
                {
                    if (HeldItem != null)
                    {
                        HeldItem.GetComponent<DDwallAbility>().InSlot = null; // makes my old item replace owner 
                        HeldItem.SetParent(DD.transform.parent);
                    }
                }
                DD.InSlot = GetComponent<RectTransform>(); //Telling The Item I hold it
                DD.Lost = false;
                DD.transform.SetParent(transform);
                DD.PreviousLocation = new Vector2(50,-50);

                HeldItem = eventData.pointerDrag.GetComponent<RectTransform>(); //Telling myself what I hold
                GameManager.instance.Wall.GetComponent<TheWall>().TheWallAttacks[SlotNum] = DD.ability;
            }
        }
    }
}
