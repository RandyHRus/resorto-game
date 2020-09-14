using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HatItemInstance: CosmeticItemInstance
{
    [SerializeField] private CharacterHatItemInformation hatItemInformation = null;
    public override InventoryItemInformation ItemInformation => hatItemInformation;

    [SerializeField] private Color32 baseColor = Color.white;
    public Color32 BaseColor
    {
        get
        {
            return baseColor;
        }
        set
        {
            baseColor = value;
        }
    }

    [SerializeField] private Color32 colorableColor = Color.white;
    public Color32 ColorableColor
    {
        get
        {
            return colorableColor;
        }
        set
        {
            colorableColor = value;
        }
    }

    public HatItemInstance(CharacterHatItemInformation itemInfo, Color32 baseColor, Color32 colorableColor): base(itemInfo)
    {
        hatItemInformation = itemInfo;
        this.BaseColor = baseColor;
        this.ColorableColor = colorableColor;
    }

    public override bool Equals(object obj)
    {
        //Check for null and compare run-time types.
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            return (((HatItemInstance)obj).ItemInformation == this.ItemInformation &&
                this.BaseColor.Equals(((HatItemInstance)obj).BaseColor) &&
                this.ColorableColor.Equals(((HatItemInstance)obj).ColorableColor));
        }

    }

    public override int GetHashCode()
    {
        /*Since I cant use mutable fields in hash code, this should be fine
         * as long as I dont use it in a hash table
        */
        return ItemInformation.GetHashCode();
    }
}
