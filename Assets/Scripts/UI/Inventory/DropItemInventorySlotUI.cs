using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemInventorySlotUI : InventorySlotUI
{
    public DropItemInventorySlotUI(DropItemInventorySlot slot, Transform parent): base (slot, ResourceManager.Instance.DropItemInventorySlot, parent)
    {

    }
}
