using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CosmeticItemInstance : InventoryItemInstance
{
    public abstract Color32? ColorPrimary { get; }
    public abstract Color32? ColorSecondary { get; }

    public CosmeticItemInstance(CharacterCosmeticItemInformation item): base(item)
    {

    }
}
