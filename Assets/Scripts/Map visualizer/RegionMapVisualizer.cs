using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MapVisualizer/Region")]
public class RegionMapVisualizer : ColorMapVisualizer
{
    public override Color32 GetColor(Vector2Int position)
    {
        TileInformation tileInfo = TileInformationManager.Instance.GetTileInformation(position);

        if (tileInfo.region != null)
        {
            return tileInfo.region.regionInformation.ShowColor;
        }
        else
        {
            return Color.white;
        }
    }
}
