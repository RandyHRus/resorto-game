using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MapVisualizer/Region")]
public class RegionMapVisualizer : ColorMapVisualizer
{
    public override Color32 GetColor(Vector2Int position)
    {
        TileInformationManager.Instance.TryGetTileInformation(position, out TileInformation tileInfo);

        if (tileInfo.Region != null)
        {
            return tileInfo.Region.regionInformation.ShowColor;
        }
        else
        {
            return Color.white;
        }
    }
}
