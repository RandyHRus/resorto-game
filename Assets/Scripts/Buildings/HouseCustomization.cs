using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseCustomization: IBuildingCustomization
{
    public Dictionary<HousePartIndex, Sprite> housePartToSprite;

    public HouseCustomization()
    {
        housePartToSprite = new Dictionary<HousePartIndex, Sprite>();
    }
}

public enum HousePartIndex
{
    Wall,
    Base,
    WallSupport,
    Chimney,
    Window,
    Door,
    Roof
}