using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CosmeticInventorySlot : ItemInventorySlot
{
    public override void ClickSlot_primary()
    {
        SlotInteraction();
    }

    public override void ClickSlot_secondary()
    {
        SlotInteraction();
    }

    private void SlotInteraction()
    {
        StorageItemInventorySlot mouseDraggingSlotInformation = InventoryManager.Instance.MouseDraggingSlot;

        InventoryItemInformation currentItem = Item;
        int currentCount = Count;


    }
}
