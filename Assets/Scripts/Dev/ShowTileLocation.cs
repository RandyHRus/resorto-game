using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShowTileLocation
{
    public static Color32[,] GetColorMap()
    {
        int mapWidth = TileInformationManager.tileCountX;
        int mapHeight = TileInformationManager.tileCountY;

        Color32[,] colorMap = new Color32[mapWidth, mapHeight];

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                TileInformation info = TileInformationManager.Instance.GetTileInformation(pos);

                int devVisualizationIndex = Array.IndexOf(Enum.GetValues(info.tileLocation.GetType()), info.tileLocation);

                Color32 color = ResourceManager.Instance.tileLocationColors[devVisualizationIndex];
                colorMap[x, y] = color;
            }
        }

        return colorMap;
    }
}
