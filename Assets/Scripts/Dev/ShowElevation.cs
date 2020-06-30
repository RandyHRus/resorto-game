using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class ShowElevation
{

    public static Color32[,] GetColorMap()
    {
        int mapWidth = TileInformationManager.tileCountX;
        int mapHeight = TileInformationManager.tileCountY;

        Color32[,] colorMap = new Color32[mapWidth, mapHeight];

        int elevationColorSize = ResourceManager.Instance.elevationColors.Length;

        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                Vector3Int pos = new Vector3Int(i, j, 0);
                int layerNum = TileInformationManager.Instance.GetTileInformation(pos).layerNum;

                if (layerNum != Constants.INVALID_TILE_LAYER)
                {
                    if (layerNum < elevationColorSize)
                    {
                        colorMap[i,j] = ResourceManager.Instance.elevationColors[layerNum];
                    }
                    else
                    {
                        Debug.Log("No color set for elevation layer");
                        colorMap[i, j] = Color.white;
                    }
                }
                else
                {
                    colorMap[i, j] = Color.white;
                }
            }
        }

        return colorMap;
    }
}
