using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryExternalUI : UIObject
{
    public InventoryExternalUI(GameObject prefab, Transform parent): base(prefab, parent)
    {
        InventoryManager.Instance.ShowInventory();
        InventoryManager.OnInventoryClosed += Hide;

        OnDestroy += UnSub;
    }

    public void Hide()
    {
        Show(false);
    }

    public override void Show(bool show)
    {
        base.Show(show);

        if (show)
            InventoryManager.OnInventoryClosed += Hide;
        else
            InventoryManager.OnInventoryClosed -= Hide;
    }

    public void UnSub()
    {
        InventoryManager.OnInventoryClosed -= Hide;
        OnDestroy -= UnSub;
    }
}
