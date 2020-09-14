using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuildable
{
    string Name { get; }

    bool Collision { get; }
    
    bool ObjectsCanBePlacedOnTop { get; }

    int OnTopOffsetInPixels { get; }

    //Set to 0 if no transparency
    int TransparencyCapableYSize { get; }

    Vector2Int GetSizeOnTile(BuildRotation rotation);

    void OnRemoveThroughState(BuildOnTile buildOnTile);
}

public enum ObjectPlaceableLocation
{
    Land,
    Water,
    SandOnly,
    GrassOnly
}

public enum ObjectType
{
    Standard,
    Ground,
    OnTop
}