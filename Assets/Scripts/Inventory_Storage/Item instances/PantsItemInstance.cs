using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PantsItemInstance : CosmeticItemInstance
{
    [SerializeField] private CharacterPantsItemInformation pantsItemInformation = null;
    public override InventoryItemInformation ItemInformation => pantsItemInformation;

    [SerializeField] private Color32 color = Color.white;
    public Color32 Color_
    {
        get
        {
            return color;
        }
        set
        {
            color = value;
        }
    }

    public PantsItemInstance(CharacterPantsItemInformation itemInfo, Color32 color) : base(itemInfo)
    {
        pantsItemInformation = itemInfo;
        this.Color_ = color;
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
            return (((PantsItemInstance)obj).ItemInformation == this.ItemInformation &&
                this.Color_.Equals(((PantsItemInstance)obj).Color_));
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
