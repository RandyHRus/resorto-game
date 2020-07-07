using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage
{
    private int capacity;
    private ItemSlot[] itemSlots;

    public Storage(int capacity)
    {
        itemSlots = new ItemSlot[capacity];
        for (int i = 0; i < capacity; i++)
        {
            itemSlots[i] = new ItemSlot();
        }
    }

    public ItemSlot GetItemSlot(int index)
    {
        return itemSlots[index];
    }
}
