using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseInformation
{
    public Dictionary<HousePartIndex, Sprite> housePartToSprite;

    public HouseInformation()
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