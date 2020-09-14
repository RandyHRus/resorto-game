using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage
{
    public int SlotCountX { get; private set; }
    public int SlotCountY { get; private set; }

    public int SlotCount { get; private set; }
    private StorageItemInventorySlot[] slots;

    public Storage(int slotCountX, int slotCountY)
    {
        this.SlotCountX = slotCountX;
        this.SlotCountY = slotCountY;

        SlotCount = slotCountX * slotCountY;

        slots = new StorageItemInventorySlot[SlotCount];
        for (int i = 0; i < SlotCount; i++)
        {
            slots[i] = new StorageItemInventorySlot();
        }
    }

    public StorageItemInventorySlot GetStorageSlotInformation(int index)
    {
        if (index < 0 || index >= slots.Length)
            return null;

        return slots[index];
    }
}
