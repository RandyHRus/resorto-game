using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class FishItemInstance : InventoryItemInstance
{
    public int Millimetres { get; }

    public FishItemInstance(AssetReference itemAsset, int millimetres): base(itemAsset)
    {
        this.Millimetres = millimetres;
    }

    public override void ShowMessage(int count)
    {
        new FishGainMessage(this, count);
    }
}
