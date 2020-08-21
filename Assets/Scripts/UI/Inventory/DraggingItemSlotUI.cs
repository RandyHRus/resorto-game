using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DraggingItemSlotUI : ItemSlotUI
{
    public DraggingItemSlotUI(DraggingItemInventorySlot slotInfo, Transform parent): base (slotInfo, parent)
    {
        ObjectInScene.GetComponent<Image>().raycastTarget = false;
    }

    public override void StartEnlarge()
    {
        throw new System.Exception("Can't enlarge!");
    }

    public override void StartShrink()
    {
        throw new System.Exception("Can't shrink!");
    }
}
