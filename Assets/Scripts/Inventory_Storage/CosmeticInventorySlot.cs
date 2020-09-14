using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CosmeticInventorySlot<T> : ItemInventorySlot where T: CharacterCosmeticItemInformation
{
    public override void ClickSlot_primary()
    {
        SlotInteraction();
    }

    public override void ClickSlot_secondary()
    {
        SlotInteraction();
    }

    public void SetSlot(InventoryItemInstance item)
    {
        if (item != null && item.ItemInformation.GetType() != typeof(T))
        {
            throw new System.Exception("Invalid item in cosmetic slot");
        }

        //Count can only be 1 or 0
        count = (item == null) ? 0 : 1;

        InventoryItemInstance oldItem = Item;
        this.item = item;

        if (oldItem == null)
        {
            if (this.Item != null)
            {
                OnItemChanged();
            }
        }
        else if (!oldItem.Equals(this.Item))
        {
            OnItemChanged();
        }

        OnSlotChanged();
    }

    private void SlotInteraction()
    {
        StorageItemInventorySlot mouseDraggingSlotInformation = InventoryManager.Instance.MouseDraggingSlot;

        InventoryItemInstance currentItem = Item;

        if (mouseDraggingSlotInformation.Item?.ItemInformation is T cosmetic)
        {
            SetSlot(mouseDraggingSlotInformation.Item);
            mouseDraggingSlotInformation.SetSlot(currentItem, currentItem == null ? 0 : 1);
        }
        else if (Item != null && mouseDraggingSlotInformation.Item == null)
        {
            SetSlot(null);
            mouseDraggingSlotInformation.SetSlot(currentItem, 1);
        }
    }
}
