using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Pants")]
public class CharacterPantsItemInformation : CharacterCosmeticItemInformation
{
    public override bool HasPrimaryColor
    {
        get
        {
            return true;
        }
    }

    public override bool HasSecondaryColor
    {
        get
        {
            return false;
        }
    }
}
