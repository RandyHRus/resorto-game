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
        PlayerStateMachine.Instance.SwitchState<DefaultState>();
    }

    public abstract bool HasPrimaryColor { get; }

    public abstract bool HasSecondaryColor { get; }
    
    public enum NumberOfColors
    {
        None,
        One,
        Two
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