using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseCustomizationLoader : MonoBehaviour, IBuildingCustomizationLoader
{
    [EnumNamedArray(typeof(HousePartIndex)), SerializeField]
    private SpriteRenderer[] partIndexToRenderer = null;

    public void LoadCustomization(IBuildingCustomization customization)
    {
        HouseCustomization houseCustomization = (HouseCustomization)customization;

        foreach (KeyValuePair<HousePartIndex, Sprite> pair in houseCustomization.housePartToSprite)
        {
            partIndexToRenderer[(int)pair.Key].sprite = pair.Value;
        }
    }
}