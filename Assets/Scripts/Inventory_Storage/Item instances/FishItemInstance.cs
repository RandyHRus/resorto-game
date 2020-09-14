using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishItemInstance : InventoryItemInstance
{
    public int Millimetres { get; }

    public FishItemInstance(FishItemInformation itemInfo, int millimetres): base(itemInfo)
    {
        this.Millimetres = millimetres;
    }

    public override void ShowMessage(int count)
    {
        new FishGainMessage(this, count);
    }
}
