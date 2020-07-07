using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TileLocationManager
{
    public static TileLocation isWater = TileLocation.DeepWater | TileLocation.WaterEdge;
    public static TileLocation isCliff = TileLocation.CliffFront | TileLocation.CliffBack;
    public static TileLocation isLand  = TileLocation.Grass | TileLocation.Sand;
}

[Flags]
public enum TileLocation
{
    Unknown = 0,

    DeepWater = 1 << 0,
    WaterEdge = 1 << 1,
    Grass = 1 << 2,
    Sand = 1 << 3,
    CliffFront = 1 << 4,
    CliffBack = 1 << 5,
}