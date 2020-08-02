using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseInformationLoader : MonoBehaviour
{
    [EnumNamedArray(typeof(HousePartIndex)), SerializeField]
    private SpriteRenderer[] partIndexToRenderer = null;

    public void LoadInformation(HouseInformation info)
    {
        foreach (KeyValuePair<HousePartIndex, Sprite> pair in info.housePartToSprite)
        {
            partIndexToRenderer[(int)pair.Key].sprite = pair.Value;
        }
    }
}