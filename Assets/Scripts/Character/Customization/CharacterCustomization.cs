using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterCustomization
{
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

    private static List<Color32> commonSkinColors = new List<Color32>()
    {
        new Color32(141, 85, 36, 255),
        new Color32(198, 134, 66, 255),
        new Color32(224, 172, 105, 255),
        new Color32(241, 194, 125, 255),
        new Color32(255, 219, 172, 255)
    };

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


    public CharacterCustomization(CharacterEyebrows eyebrows, Color32 eyebrowsColor, CharacterHair hair, Color32 hairColor,
        Color32 skinColor, Color32 eyesColor, CharacterSex sex, HatItemInstance hat, ShirtItemInstance shirt, 
        PantsItemInstance pants)
    {
        SetParts(eyebrows, eyebrowsColor, hair, hairColor, skinColor, eyesColor, sex, hat, shirt, pants);
    }

    public static CharacterCustomization RandomCharacterCustomization()
    {
        Color32 GetRandomColor()
        {
            return new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
        }

        CharacterEyebrows eyebrows = ScenesSharedResources.Instance.EyebrowsOptions[Random.Range(0, ScenesSharedResources.Instance.EyebrowsOptions.Length)];
        Color32 eyebrowsColor = GetRandomColor();

        CharacterHair hair = ScenesSharedResources.Instance.HairOptions[Random.Range(0, ScenesSharedResources.Instance.HairOptions.Length)];
        Color32 hairColor = GetRandomColor();

        Color32 skinColor;
        //Most times, pick a common skin color
        if (Random.Range(0, 10) < 8)
        {
            skinColor = commonSkinColors[Random.Range(0, commonSkinColors.Count)];
        }
        else
        {
            skinColor = GetRandomColor();
        }

        Color32 eyesColor = GetRandomColor();

        PantsItemInstance pantsInstance = new PantsItemInstance(ScenesSharedResources.Instance.Pants, GetRandomColor());

        CharacterShirtItemInformation randomShirt = ScenesSharedResources.Instance.ShirtOptions[Random.Range(0, ScenesSharedResources.Instance.ShirtOptions.Length)];
        ShirtItemInstance shirtItem = new ShirtItemInstance(randomShirt, 
            randomShirt.HasPrimaryColor ? (Color32?)GetRandomColor() : null,
            randomShirt.HasSecondaryColor ? (Color32?)GetRandomColor() : null);

        CharacterSex randomSex = (CharacterSex)Random.Range(0, System.Enum.GetNames(typeof(CharacterSex)).Length);

        return new CharacterCustomization(eyebrows, eyebrowsColor, hair, hairColor, skinColor, eyesColor, randomSex, null, shirtItem, pantsInstance);
    }

    public void SetParts(CharacterEyebrows eyebrows, Color32 eyebrowsColor, CharacterHair hair, Color32 hairColor,
        Color32 skinColor, Color32 eyesColor, CharacterSex sex, HatItemInstance hat, ShirtItemInstance shirt,
        PantsItemInstance pants)
    {
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