using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapVisualizer : MonoBehaviour
{
    [SerializeField] private Tilemap colorTilemap = null;
    [SerializeField] private Tile indicatorTile = null;

    private DevMapVisualizations currentVisualization = DevMapVisualizations.None;

    private void Update()
    {
        if (Input.GetButtonDown("Dev"))
        {
            //Cycle through visualizations
            {
                int nextVisualizationIndex = (int)currentVisualization + 1;
                if (nextVisualizationIndex >= Enum.GetNames(typeof(DevMapVisualizations)).Length)
                {
                    nextVisualizationIndex = 0;
                }
                currentVisualization = (DevMapVisualizations)nextVisualizationIndex;
            }

            //Activative/Deactivate tilemap
            {
                if (currentVisualization == DevMapVisualizations.None)
                    colorTilemap.gameObject.SetActive(false);
                else
                    colorTilemap.gameObject.SetActive(true);
            }

            if (currentVisualization == DevMapVisualizations.Elevation)
            {
                ShowVisualization(ShowElevation.GetColorMap());
            }
            else if (currentVisualization == DevMapVisualizations.TileLocation)
            {
                ShowVisualization(ShowTileLocation.GetColorMap());
            }
        }
    }

    private void ShowVisualization(Color32[,] colorMap)
    {
        for (int x = 0; x < colorMap.GetLength(0); x++)
        {
            for (int y = 0; y < colorMap.GetLength(1); y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                colorTilemap.SetTile(pos, indicatorTile);
                colorTilemap.SetTileFlags(pos, TileFlags.None);
                colorTilemap.SetColor(pos, colorMap[x,y]);
            }
        }
    }

    private enum DevMapVisualizations
    {
        None,
        Elevation,
        TileLocation
    }
}
