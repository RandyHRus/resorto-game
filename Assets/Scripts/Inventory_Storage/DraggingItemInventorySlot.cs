using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggingItemInventorySlot : StorageItemInventorySlot
{
    public override void ClickSlot_primary()
    {
        throw new System.Exception("Not clickable!");
    }

    public override void ClickSlot_secondary()
    {
        throw new System.Exception("Not clickable!");
    }

}
