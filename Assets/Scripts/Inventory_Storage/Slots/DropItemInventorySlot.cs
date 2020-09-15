using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemInventorySlot : InventorySlot
{
    public override void ClickSlot_primary()
    {
        InventoryManager.Instance.DropDraggingItem();
    }

    public override void ClickSlot_secondary()
    {
        InventoryManager.Instance.DropDraggingItem();
    }
}
