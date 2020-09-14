using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Hat")]
public class CharacterHatItemInformation : CharacterCosmeticItemInformation
{
    [SerializeField] private CosmeticSpritePair baseSpritePair = null;
    public CosmeticSpritePair BaseSpritePair => baseSpritePair;

    [SerializeField] private bool baseColorable = false;
    public bool BaseColorable => baseColorable;

    [SerializeField] private bool hasColorable = false;
    public bool HasColorable => hasColorable;

    [ConditionalHide("hasColorable", false, false), SerializeField] private CosmeticSpritePair colorableSpritePair = null;
    public CosmeticSpritePair ColorableSpritePair => colorableSpritePair;
}
