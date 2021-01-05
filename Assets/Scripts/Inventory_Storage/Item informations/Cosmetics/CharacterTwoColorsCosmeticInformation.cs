using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterTwoColorsCosmeticInformation : CharacterCosmeticItemInformation
{
    [SerializeField] private CosmeticSpritePair baseSpritePair = null;
    public CosmeticSpritePair BaseSpritePair => baseSpritePair;

    [SerializeField] private CosmeticSpritePair topSpritePair = null;
    public CosmeticSpritePair TopSpritePair => topSpritePair;
}

[System.Serializable]
public class CosmeticSpritePair
{
    [SerializeField] private Sprite spriteFront = null;
    public Sprite SpriteFront => spriteFront;

    [SerializeField] private Sprite spriteBack = null;
    public Sprite SpriteBack => spriteBack;

}