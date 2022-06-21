using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class DragAndDropItem : MonoBehaviour , IBeginDragHandler, IEndDragHandler, IDragHandler
{
    //InventorySystem IS;
    protected Canvas canvas;
    [HideInInspector]
    public Image image;
    protected RectTransform rectTransform;
    public RectTransform InSlot;
    CanvasGroup canvasGroup;
    [HideInInspector]
    public Vector2 PreviousLocation;
    [HideInInspector]
    public string Name;
    public bool Lost;

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = .6f;
        PreviousLocation = rectTransform.anchoredPosition;
        Lost = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1;
        rectTransform.anchoredPosition = PreviousLocation;
        if (Lost) { LostGrip(); }
    }

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        image = GetComponent<Image>();
    }
  

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            if (eventData.pointerDrag != null)
            {
                if (InSlot != null)
                {
                    InSlot.GetComponent<WallAbilitySlot>().OnDrop(eventData);
                }

            }
        }
    }

    public abstract void LostGrip();

 
}
