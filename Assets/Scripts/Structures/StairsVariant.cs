using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Structure Variants/Stairs")]
public class StairsVariant : StructureVariantInformation
{
    [EnumNamedArray(typeof(StairsDirection)), SerializeField]
    private Sprite[] stairsSprites = null;
    
    public Sprite GetSprite(StairsDirection dir)
    {
        return stairsSprites[(int)dir];
    }
}

public enum StairsDirection
{
    Front,
    Left,
    Right
}