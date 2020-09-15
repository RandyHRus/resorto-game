using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CosmeticItemInstance : InventoryItemInstance
{
    public abstract Color32? PrimaryColor { get; }
    public abstract Color32? SecondaryColor { get; }

    public CosmeticItemInstance(CharacterCosmeticItemInformation item): base(item)
    {

    }

    public override void ShowMessage(int count)
    {
        new CosmeticGainMessage(this, count);
    }
}
