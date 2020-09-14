using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildTransparencyManager
{
    private static float targetTransparency = 0.5f;

    public static void ToggleTransparencies(BuildOnTile build, bool transparent)
    {
        SpriteRenderer[] renderers = build.GameObjectOnTile.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer r in renderers)
        {
            Color32 currColor = r.color;
            currColor.a = transparent ? (byte)(targetTransparency * 255) : (byte)(1 * 255);
            r.color = currColor;
        }
    }
}
