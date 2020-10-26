using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCustomization
{
    //Set to defalt character if not overridden later by (ex CharacterCustomizationMenu)
    public static CharacterCustomization Character { get; set; } = ScenesSharedResources.Instance.DefaultCharacter.CharacterCustomization;

    public static string PlayerName { get; set; }
}
