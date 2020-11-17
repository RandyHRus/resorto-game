using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MapVisualizer/Elevation")]
public class ElevationMapVisualizer : ColorMapVisualizer
{
    [SerializeField] private Color32[] elevationColors = null;

    [SerializeField] private Color32 waterColor = default;

    public override Color32 GetColor(Vector2Int position)
    {
        TileInformationManager.Instance.TryGetTileInformation(position, out TileInformation tileInfo);

        bool isDeepWater = tileInfo.tileLocation == TileLocation.DeepWater;

        if (isDeepWater && tileInfo.layerNum == 0)
        {
            return waterColor;
        }
        else
        {
            int layerNum = tileInfo.layerNum;

            if (layerNum != Constants.INVALID_TILE_LAYER)
            {
                if (layerNum < elevationColors.Length)
                {
                    return elevationColors[layerNum];
                }
                else
                {
                    return elevationColors[elevationColors.Length - 1];
                }
            }
            else
            {
                return Color.white;
            }
        }
    }
}
