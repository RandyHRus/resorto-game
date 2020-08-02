using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage
{
    public int SlotCountX { get; private set; }
    public int SlotCountY { get; private set; }

    private ItemSlotInformation[] slots;

    public Storage(int slotCountX, int slotCountY, int individualSlotCapacity)
    {
        this.SlotCountX = slotCountX;
        this.SlotCountY = slotCountY;

        int totalSlotCount = slotCountX * slotCountY;

        slots = new ItemSlotInformation[totalSlotCount];
        for (int i = 0; i < totalSlotCount; i++)
        {
            slots[i] = new ItemSlotInformation(individualSlotCapacity);
        }
    }

    public ItemSlotInformation GetSlotInformation(int index)
    {
        return slots[index];
    }
}
