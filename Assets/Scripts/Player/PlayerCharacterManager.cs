using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterManager : MonoBehaviour
{
    private void Start()
    {
        if (CharacterCustomizationMenu.playerCharacter != null)
        {
            GetComponent<CharacterCustomizationLoader>().LoadCharacter(CharacterCustomizationMenu.playerCharacter);
        }
        else
        {
            Debug.Log("No player characer set. Not applying character information.");
        }
    }
}
