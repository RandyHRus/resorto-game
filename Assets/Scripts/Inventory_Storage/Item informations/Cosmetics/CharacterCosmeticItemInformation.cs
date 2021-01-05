using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterCosmeticItemInformation : InventoryItemInformation
{
    public override ItemTag Tag { get { return ItemTag.Cosmetic; } }

    public override bool Stackable { get { return false; } }

    public override void ItemSelected()
    {
        PlayerStateMachineManager.Instance.SwitchState<DefaultState>();
    }
}