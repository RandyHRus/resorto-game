using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlotInformation
{
    public int Capacity { get; private set; } = 0;
    public InventoryItemInformation Item { get; private set; } = null;
    public int Count { get; private set; } = 0;

    public delegate void OnSlotChange();
    public event OnSlotChange OnSlotChanged;

    public delegate void OnItemChange();
    public event OnItemChange OnItemChanged;

    public ItemSlotInformation(int capacity)
    {
        this.Capacity = capacity;
    }

    public void SetSlot(InventoryItemInformation item, int count)
    {
        if (count > Capacity)
            Debug.LogError("Too many items in slot!");

        InventoryItemInformation oldItem = this.Item;

        this.Item = item;
        this.Count = count;

        if (count <= 0)
        {
            this.Item = null;
            this.Count = 0;
        }

        OnSlotChanged?.Invoke();

        if (oldItem != this.Item)
        {
            OnItemChanged?.Invoke();
        }
    }
}
