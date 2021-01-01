using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class NPCInformation
{
    [SerializeField] private string npcName = "";
    public string NpcName => npcName;

    [SerializeField] public CharacterCustomization characterCustomization = null;
    public CharacterCustomization CharacterCustomization => characterCustomization;

    public NPCInformation(string name, CharacterCustomization customization)
    {
        this.npcName = name;
        this.characterCustomization = customization;
    }
}
