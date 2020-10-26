using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShirtItemInstance : TwoColorsCosmeticInstance
{
    [SerializeField] private CharacterShirtItemInformation shirtItemInformation = null;
    public override InventoryItemInformation ItemInformation => shirtItemInformation;

    [TwoColorsCosmeticColorHide(true, "shirtItemInformation"), SerializeField] private Color32 primaryColor = Color.white;
    public override Color32? PrimaryColor
    {
        get
        {
            if (shirtItemInformation.HasPrimaryColor)
                return primaryColor;
            else
                return null;
        } 
    }

    [TwoColorsCosmeticColorHide(false, "shirtItemInformation"), SerializeField] private Color32 secondaryColor = Color.white;
    public override Color32? SecondaryColor
    {
        get
        {
            if (shirtItemInformation.HasSecondaryColor)
                return secondaryColor;
            else
                return null;
        }
    }

    public ShirtItemInstance(CharacterShirtItemInformation itemInfo, Color32? primaryColor, Color32? secondaryColor) : base(itemInfo, primaryColor, secondaryColor)
    {
        shirtItemInformation = itemInfo;

        if (primaryColor != null)
        {
            if (shirtItemInformation.HasPrimaryColor)
                this.primaryColor = (Color32)primaryColor;
            else
                throw new System.Exception("Too many colors!");
        }
        else
        {
            if (shirtItemInformation.HasPrimaryColor)
                throw new System.Exception("Missing color!");
        }

        if (secondaryColor != null)
        {
            if (shirtItemInformation.HasSecondaryColor)
                this.secondaryColor = (Color32)secondaryColor;
            else
                throw new System.Exception("Too many colors!");
        }
        else
        {
            if (shirtItemInformation.HasSecondaryColor)
                throw new System.Exception("Missing color!");
        }
    }
}