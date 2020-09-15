using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HatItemInstance: TwoColorsCosmeticInstance
{
    [SerializeField] private CharacterHatItemInformation hatItemInformation = null;
    public override InventoryItemInformation ItemInformation => hatItemInformation;

    [TwoColorsCosmeticColorHide(true, "hatItemInformation"), SerializeField] private Color32 primaryColor = Color.white;
    public override Color32? PrimaryColor
    {
        get
        {
            if (hatItemInformation.HasPrimaryColor)
                return primaryColor;
            else
                return null;
        }
    }

    [TwoColorsCosmeticColorHide(false, "hatItemInformation"), SerializeField] private Color32 secondaryColor = Color.white;
    public override Color32? SecondaryColor
    {
        get
        {
            if (hatItemInformation.HasSecondaryColor)
                return secondaryColor;
            else
                return null;
        }
    }

    public HatItemInstance(CharacterHatItemInformation itemInfo, Color32? primaryColor, Color32? secondaryColor): base(itemInfo, primaryColor, secondaryColor)
    {
        hatItemInformation = itemInfo;
        
        if (primaryColor != null)
        {
            if (hatItemInformation.HasPrimaryColor)
                this.primaryColor = (Color32)primaryColor;
            else
                throw new System.Exception("Too many colors!");
        }
        else
        {
            if (hatItemInformation.HasPrimaryColor)
                throw new System.Exception("Missing color!");
        }

        if (secondaryColor != null)
        {
            if (hatItemInformation.HasSecondaryColor)
                this.secondaryColor = (Color32)secondaryColor;
            else
                throw new System.Exception("Too many colors!");
        }
        else
        {
            if (hatItemInformation.HasSecondaryColor)
                throw new System.Exception("Missing color!");
        }
    }
}
