﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryExternalUI : UIObject
{
    public InventoryExternalUI(GameObject prefab, Transform parent): base(prefab, parent)
    {
        InventoryManager.Instance.ShowInventory();
        InventoryManager.Instance.OnInventoryClosed += Hide;
    }

    public void Hide()
    {
        Show(false);
    }

    public override void Show(bool show)
    {
        base.Show(show);

        if (show)
            InventoryManager.Instance.OnInventoryClosed += Hide;
        else
            InventoryManager.Instance.OnInventoryClosed -= Hide;
    }

    public override void Destroy()
    {
        base.Destroy();
        InventoryManager.Instance.OnInventoryClosed -= Hide;
    }
}
