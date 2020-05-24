using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotClick : MonoBehaviour, IPointerClickHandler
{
    public int slotIndex = Constants.INVALID_ID;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (slotIndex == Constants.INVALID_ID)
            {
                Debug.Log("Slot index not initialized");
                return;
            }
            else
            {
                InventoryManager.Instance.ClickItemSlot_primary(slotIndex);
                return;
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (slotIndex == Constants.INVALID_ID)
            {
                Debug.Log("Slot index not initialized");
                return;
            }
            else
            {
                InventoryManager.Instance.ClickItemSlot_secondary(slotIndex);
                return;
            }
        }
    }
}
