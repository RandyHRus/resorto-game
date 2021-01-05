using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public abstract class CosmeticItemInstance : InventoryItemInstance
{
    public abstract Color32? PrimaryColor { get; }
    public abstract Color32? SecondaryColor { get; }


    public CosmeticItemInstance(): base()
    {

    }

    public CosmeticItemInstance(AssetReference item): base(item)
    {

    }

    public override void ShowMessage(int count)
    {
        new CosmeticGainMessage(this, count);
    }
}
