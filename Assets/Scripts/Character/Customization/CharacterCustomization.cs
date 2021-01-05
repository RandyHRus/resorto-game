using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Newtonsoft.Json;

[System.Serializable]
public class CharacterCustomization
{
    private static List<Color32> commonSkinColors = new List<Color32>()
    {
        new Color32(141, 85, 36, 255),
        new Color32(198, 134, 66, 255),
        new Color32(224, 172, 105, 255),
        new Color32(241, 194, 125, 255),
        new Color32(255, 219, 172, 255)
    };

    public CustomEventManager<string> CustomizationPartChangedEventManager { get; } = new CustomEventManager<string>();

    [SerializeField, JsonProperty, JsonConverter(typeof(AssetReferenceConverter))] private AssetReference eyebrows;
    [JsonIgnore] public AssetReference Eyebrows { get { return eyebrows; } set { eyebrows = value; CustomizationPartChangedEventManager.TryInvokeEventGroup("eyebrows", null); } }

    [SerializeField] private Color32 eyebrowsColor;
    [JsonIgnore] public Color32 EyebrowsColor { get { return eyebrowsColor; } set { eyebrowsColor = value; CustomizationPartChangedEventManager.TryInvokeEventGroup("eyebrowsColor", null); } }

    [SerializeField, JsonProperty, JsonConverter(typeof(AssetReferenceConverter))] private AssetReference hair;
    [JsonIgnore] public AssetReference Hair { get { return hair; } set { hair = value; CustomizationPartChangedEventManager.TryInvokeEventGroup("hair", null); } }

    [SerializeField, JsonProperty] private Color32 hairColor;
    [JsonIgnore] public Color32 HairColor { get { return hairColor; } set { hairColor = value; CustomizationPartChangedEventManager.TryInvokeEventGroup("hairColor", null); } }

    [SerializeField, JsonProperty] private Color32 skinColor;
    [JsonIgnore] public Color32 SkinColor { get { return skinColor; } set { skinColor = value; CustomizationPartChangedEventManager.TryInvokeEventGroup("skinColor", null); } }

    [SerializeField, JsonProperty] private Color32 eyesColor;
    [JsonIgnore] public Color32 EyesColor { get { return eyesColor; } set { eyesColor = value; CustomizationPartChangedEventManager.TryInvokeEventGroup("eyesColor", null); } }

    [SerializeField, JsonProperty] private CharacterSex sex;
    [JsonIgnore] public CharacterSex Sex { get { return sex; } set { sex = value; CustomizationPartChangedEventManager.TryInvokeEventGroup("sex", null); } }

    [SerializeField, JsonProperty] private TwoColorsCosmeticInstance hat = null;
    [JsonIgnore] public TwoColorsCosmeticInstance Hat
    {
        get
        {
            if (hat?.GetItemInformation() == null)
                return null;
            else
                return hat;
        }
        set
        {
            hat = value;
            CustomizationPartChangedEventManager.TryInvokeEventGroup("hat", null);
        }
    }

   [SerializeField, JsonProperty] private TwoColorsCosmeticInstance shirt = null;
   [JsonIgnore] public TwoColorsCosmeticInstance Shirt
    {
        get
        {
            if (shirt?.GetItemInformation() == null)
                return null;
            else
                return shirt;
        }
        set
        {
            shirt = value;
            CustomizationPartChangedEventManager.TryInvokeEventGroup("shirt", null);
        }
    }


    [SerializeField, JsonProperty] private PantsItemInstance pants = null;
    [JsonIgnore] public PantsItemInstance Pants
    {
        get
        {
            if (pants?.GetItemInformation() == null)
                return null;
            else
                return pants;
        }
        set
        {
            pants = value;
            CustomizationPartChangedEventManager.TryInvokeEventGroup("pants", null);
        }
    }

    public static CharacterCustomization Copy(CharacterCustomization original)
    {
        return new CharacterCustomization(original.Eyebrows, original.EyebrowsColor, original.Hair, original.HairColor,
            original.SkinColor, original.EyesColor, original.Sex, original.Hat, original.Shirt, original.Pants);
    }

    //Default constructor for JSON deserialization
    public CharacterCustomization()
    {
    }


    public CharacterCustomization(AssetReference eyebrows, Color32 eyebrowsColor, AssetReference hair, Color32 hairColor,
        Color32 skinColor, Color32 eyesColor, CharacterSex sex, TwoColorsCosmeticInstance hat, TwoColorsCosmeticInstance shirt, 
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

    public static CharacterCustomization RandomCharacterCustomization(CharacterSex characterSex)
    {
        Color32 GetRandomColor()
        {
            return new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
        }

        AssetReference randomEyebrows = ScenesSharedResources.Instance.EyebrowsOptions[Random.Range(0, ScenesSharedResources.Instance.EyebrowsOptions.Length)];
        Color32 randomEyebrowsColor = GetRandomColor();

        AssetReference randomHair = ScenesSharedResources.Instance.HairOptions[Random.Range(0, ScenesSharedResources.Instance.HairOptions.Length)];
        Color32 randomHairColor = GetRandomColor();

        Color32 randomSkinColor;
        //Most times, pick a common skin color
        if (Random.Range(0, 10) < 8)
        {
            randomSkinColor = commonSkinColors[Random.Range(0, commonSkinColors.Count)];
        }
        else
        {
            randomSkinColor = GetRandomColor();
        }

        Color32 randomEyesColor = GetRandomColor();

        PantsItemInstance randomPantsInstance = new PantsItemInstance(ScenesSharedResources.Instance.Pants, GetRandomColor());

        AssetReference randomShirt = ScenesSharedResources.Instance.ShirtOptions[Random.Range(0, ScenesSharedResources.Instance.ShirtOptions.Length)];
        CharacterShirtItemInformation randomShirtInfo = AssetsManager.GetAsset<CharacterShirtItemInformation>(randomShirt);
        TwoColorsCosmeticInstance randomShirtInstance = new TwoColorsCosmeticInstance(randomShirt, GetRandomColor(), GetRandomColor());

        return new CharacterCustomization(randomEyebrows, randomEyebrowsColor, randomHair, randomHairColor, randomSkinColor, randomEyesColor, characterSex, null, randomShirtInstance, randomPantsInstance);
    }
}