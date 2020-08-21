using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageItemInventorySlot: ItemInventorySlot
{
    private int capacity = 99;

    public void SetSlot(InventoryItemInformation item, int count)
    {
        if (count > capacity)
            Debug.LogError("Too many items in slot!");

        InventoryItemInformation oldItem = this.Item;

        this.item = item;
        this.count = count;

        if (count <= 0)
        {
            this.item = null;
            this.count = 0;
        }

        OnSlotChanged();

        if (oldItem != this.Item)
        {
            OnItemChanged();
        }
    }

    public void RemoveItem(int count)
    {
        if (this.Count - count < 0)
            Debug.LogError("Tried to remove too many items");
        SetSlot(Item, this.Count - count);
    }

    public void FillSlotToCapacity(InventoryItemInformation item, int toAdd, out int remains)
    {
        if (Count + toAdd <= capacity)
        {
            SetSlot(item, Count + toAdd);
            remains = 0;
            return;
        }
        else if (Count < capacity)
        {
            toAdd -= capacity - Count;
            SetSlot(item, capacity);
            remains = toAdd;
            return;
        }

        remains = toAdd;
    }

    public override void ClickSlot_primary()
    {
        StorageItemInventorySlot mouseDraggingSlotInformation = InventoryManager.Instance.MouseDraggingSlot;

        InventoryItemInformation currentItem = Item;
        int currentCount = Count;

        //Just put mouse item in inventory slot
        if (currentItem == null)
        {
            SetSlot(mouseDraggingSlotInformation.Item, mouseDraggingSlotInformation.Count);
            mouseDraggingSlotInformation.SetSlot(null, 0);
        }
        //Fill slot with dragging item
        else if (currentItem.Equals(mouseDraggingSlotInformation.Item))
        {
            FillSlotToCapacity(currentItem, mouseDraggingSlotInformation.Count, out int remains);
            mouseDraggingSlotInformation.SetSlot(currentItem, remains);
        }
        //Switch slots items
        else
        {
            SetSlot(mouseDraggingSlotInformation.Item, mouseDraggingSlotInformation.Count);
            mouseDraggingSlotInformation.SetSlot(currentItem, currentCount);
        }
    }

    public override void ClickSlot_secondary()
    {
        StorageItemInventorySlot mouseDraggingSlotInformation = InventoryManager.Instance.MouseDraggingSlot;

        InventoryItemInformation currentItem = Item;
        int currentCount = Count;

        if (currentItem == null || currentItem == mouseDraggingSlotInformation.Item)
        {
            if (mouseDraggingSlotInformation.Item != null)
            {
                FillSlotToCapacity(mouseDraggingSlotInformation.Item, 1, out int remains);
                if (remains == 0)
                    mouseDraggingSlotInformation.SetSlot(mouseDraggingSlotInformation.Item, mouseDraggingSlotInformation.Count - 1);
            }
        }
        else if (mouseDraggingSlotInformation.Item == null)
        {
            int half = currentCount / 2;
            int otherHalf = currentCount - half;

            SetSlot(currentItem, half);
            mouseDraggingSlotInformation.SetSlot(currentItem, otherHalf);
        }
    }
}
