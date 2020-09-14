using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterCustomization
{
    [SerializeField] private string characterName = "";
    public string CharacterName => characterName;

    [SerializeField] private CharacterEyebrows eyebrows = null;
    public CharacterEyebrows Eyebrows => eyebrows;

    [SerializeField] private Color32 eyebrowsColor = Color.white;
    public Color32 EyebrowsColor => eyebrowsColor;

    [SerializeField] private CharacterHair hair = null;
    public CharacterHair Hair => hair;

    [SerializeField] private Color32 hairColor = Color.white;
    public Color32 HairColor => hairColor;

    [SerializeField] private Color32 skinColor = Color.white;
    public Color32 SkinColor => skinColor;

    [SerializeField] private Color32 eyesColor = Color.white;
    public Color32 EyesColor => eyesColor;

    [SerializeField] private CharacterSex sex = 0;
    public CharacterSex Sex => sex;

    [SerializeField] private HatItemInstance hat = null;
    public HatItemInstance Hat
    {
        get
        {
            if (hat?.ItemInformation == null)
                return null;
            else
                return hat;
        }
        private set
        {
            hat = value;
        }
    }

    [SerializeField] private ShirtItemInstance shirt = null;
    public ShirtItemInstance Shirt
    {
        get
        {
            if (shirt?.ItemInformation == null)
                return null;
            else
                return shirt;
        }
        private set
        {
            shirt = value;
        }
    }


    [SerializeField] private PantsItemInstance pants = null;
    public PantsItemInstance Pants
    {
        get
        {
            if (pants?.ItemInformation == null)
                return null;
            else
                return pants;
        }
        private set
        {
            pants = value;
        }
    }


    public CharacterCustomization(string characterName, CharacterEyebrows eyebrows, Color32 eyebrowsColor, CharacterHair hair, Color32 hairColor,
        Color32 skinColor, Color32 eyesColor, CharacterSex sex, HatItemInstance hat, ShirtItemInstance shirt, 
        PantsItemInstance pants)
    {
        this.characterName = characterName;
        this.eyebrows = eyebrows;
        this.eyebrowsColor = eyebrowsColor;
        this.hair = hair;
        this.hairColor = hairColor;
        this.skinColor = skinColor;
        this.eyesColor = eyesColor;
        this.sex = sex;
        this.hat = hat;
        this.shirt = shirt;
        this.pants = pants;
    }
}