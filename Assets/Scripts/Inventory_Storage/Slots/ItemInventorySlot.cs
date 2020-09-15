using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemInventorySlot: InventorySlot
{
    protected InventoryItemInstance item = null;
    public InventoryItemInstance Item => item;

    protected int count = 0;
    public virtual int Count => count;

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
