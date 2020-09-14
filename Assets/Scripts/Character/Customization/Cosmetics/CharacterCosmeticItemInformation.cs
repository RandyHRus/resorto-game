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
        PlayerStateMachine.Instance.TrySwitchState<DefaultState>();
    }
}

[System.Serializable]
public class CosmeticSpritePair
{
    [SerializeField] private Sprite spriteFront = null;
    public Sprite SpriteFront => spriteFront;

    [SerializeField] private Sprite spriteBack = null;
    public Sprite SpriteBack => spriteBack;

}