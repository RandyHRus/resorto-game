using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character/Character base")]
public class CharacterScriptableObject : ScriptableObject
{
    [SerializeField] private CharacterCustomization characterCustomization = null;
    public CharacterCustomization CharacterCustomization => characterCustomization;
}
