using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotClick : MonoBehaviour, IPointerClickHandler
{
    public ItemSlotInformation slot = null;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (slot == null)
            {
                Debug.Log("Slot index not initialized");
                return;
            }
            else
            {
                InventoryManager.Instance.ClickItemSlot_primary(slot);
                return;
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (slot == null)
            {
                Debug.Log("Slot index not initialized");
                return;
            }
            else
            {
                InventoryManager.Instance.ClickItemSlot_secondary(slot);
                return;
            }
        }
    }
}
