using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlooringVariantBase : StructureVariantInformation
{
    [EnumNamedArray(typeof(FlooringVariantCodes)), SerializeField]
    private Sprite[] flooringSprites = null;

    [EnumNamedArray(typeof(FlooringRotation)), SerializeField]
    private Sprite[] indicatorSprites = new Sprite[Enum.GetNames(typeof(FlooringRotation)).Length];
    public Sprite[] IndicatorSprites => indicatorSprites;

    public Dictionary<int, Sprite> CodeToSprite { get; private set; }
    //public Dictionary<Sprite, int> SpriteToCode { get; private set; }

    private void OnEnable()
    {
        CodeToSprite = new Dictionary<int, Sprite>();
        //SpriteToCode = new Dictionary<Sprite, int>();
        Debug.Log("Creating dictionary");
        for (int i = 0; i < flooringSprites.Length; i++)
        {
            CodeToSprite.Add(i, flooringSprites[i]);
            //SpriteToCode.Add(flooringSprites[i], i);
        }
    }
}

//This is messy cause I need to use EnumNamedArray
//TODO: Create ListValueArray or something, not sure
public enum FlooringVariantCodes
{
    _00000 = 0b00000,
    _00001 = 0b00001,
    _00010 = 0b00010,
    _00011 = 0b00011,
    _00100 = 0b00100,
    _00101 = 0b00101,
    _00110 = 0b00110,
    _00111 = 0b00111,
    _01000 = 0b01000,
    _01001 = 0b01001,
    _01010 = 0b01010,
    _01011 = 0b01011,
    _01100 = 0b01100,
    _01101 = 0b01101,
    _01110 = 0b01110,
    _01111 = 0b01111,
    _10000 = 0b10000,
    _10001 = 0b10001,
    _10010 = 0b10010,
    _10011 = 0b10011,
    _10100 = 0b10100,
    _10101 = 0b10101,
    _10110 = 0b10110,
    _10111 = 0b10111,
    _11000 = 0b11000,
    _11001 = 0b11001,
    _11010 = 0b11010,
    _11011 = 0b11011,
    _11100 = 0b11100,
    _11101 = 0b11101,
    _11110 = 0b11110,
    _11111 = 0b11111,

}