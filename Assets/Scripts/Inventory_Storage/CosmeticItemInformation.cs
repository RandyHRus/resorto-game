using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CosmeticItemInformation : InventoryItemInformation
{
    [SerializeField] private Sprite sprite = null;
    public Sprite Sprite => sprite;

    public enum CosmeticType
    {
        Hat,
        Shirt,
        Pants,
        Shoes
    }   
}