using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotInteraction : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public InventorySlotUI slotUI = null;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (InventoryManager.Instance.IsInventoryOpen)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                slotUI.Slot.ClickSlot_primary();
                return;
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                slotUI.Slot.ClickSlot_secondary();
                return;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (InventoryManager.Instance.IsInventoryOpen)
            slotUI.StartEnlarge();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (InventoryManager.Instance.IsInventoryOpen)
            slotUI.StartShrink();
    }
}
