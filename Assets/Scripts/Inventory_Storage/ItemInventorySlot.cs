using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemInventorySlot: InventorySlot
{
    protected InventoryItemInformation item = null;
    public InventoryItemInformation Item => item;

    protected int count = 0;
    public int Count => count;

    public delegate void SlotChange();
    public event SlotChange SlotChanged;

    public delegate void ItemChange();
    public event ItemChange ItemChanged;

    public void OnItemChanged()
    {
        ItemChanged?.Invoke();
    }

    public void OnSlotChanged()
    {
        SlotChanged?.Invoke();
    }
}
