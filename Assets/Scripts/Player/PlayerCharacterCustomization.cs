using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterCustomization
{
    //Set to defalt character if not overridden later by (ex CharacterCustomizationMenu)
    public static CharacterCustomization Customization { get; set; } = ScenesSharedResources.Instance.DefaultCharacter.CharacterCustomization;
}
