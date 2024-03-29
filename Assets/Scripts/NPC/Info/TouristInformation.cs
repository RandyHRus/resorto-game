﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TouristInformation : NPCInformation
{
    [SerializeField] TouristPersonality personality = 0;
    public TouristPersonality Personality => personality;

    public TouristInformation(TouristPersonality personality, string name, CharacterCustomization customization): base(name, customization)
    {
        this.personality = personality;
    }

    public static TouristInformation CreateRandomTouristInformation()
    {
        string randomName = RandomNPCName.GetRandomNPCName();
        CharacterCustomization randomCustomization = CharacterCustomization.RandomCharacterCustomization((CharacterSex)Random.Range(0, System.Enum.GetNames(typeof(CharacterSex)).Length));
        TouristPersonality randomPersonality = (TouristPersonality)Random.Range(0, System.Enum.GetNames(typeof(TouristPersonality)).Length);

        return new TouristInformation(randomPersonality, randomName, randomCustomization);
    }
}
