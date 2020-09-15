using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TwoColorsCosmeticInstance : CosmeticItemInstance
{
    public Color32? BaseColor {
        get
        {
            if (((CharacterTwoColorsCosmeticInformation)ItemInformation).BaseColorable)
                return PrimaryColor;
            else
                return null;
        }
    }

    public Color32? ColorableColor
    {
        get
        {
            CharacterTwoColorsCosmeticInformation cosInfo = (CharacterTwoColorsCosmeticInformation)ItemInformation;

            if (cosInfo.HasColorable)
            {
                if (cosInfo.BaseColorable)
                    return SecondaryColor;
                else
                    return PrimaryColor;
            }
            else
                return null;
        }
    }

    public TwoColorsCosmeticInstance(CharacterCosmeticItemInformation itemInfo, Color32? primaryColor, Color32? secondaryColor): base(itemInfo)
    {

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
            TwoColorsCosmeticInstance compare = (TwoColorsCosmeticInstance)obj;

            return (compare.ItemInformation == this.ItemInformation &&
                (this.BaseColor == null ? (compare == null) : this.BaseColor.Equals(compare.BaseColor)) &&
                (this.ColorableColor == null ? (compare == null) : this.ColorableColor.Equals(compare.ColorableColor)));
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
