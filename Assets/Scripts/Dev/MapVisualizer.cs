using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapVisualizer : MonoBehaviour
{
    [SerializeField] private Tilemap colorTilemap = null;
    [SerializeField] private Tile indicatorTile = null;
    [SerializeField] private Color32[] elevationColors = null;
    public Color32[] ElevationColors => elevationColors;

    [EnumNamedArray(typeof(TileLocation)), SerializeField]
    private Color32[] tileLocationColors = new Color32[Enum.GetNames(typeof(TileLocation)).Length];
    public Color32[] TileLocationColors => tileLocationColors;

    [EnumNamedArray(typeof(ObjectsAndFloorings)), SerializeField]
    private Color32[] objectsAndFlooringsColors = new Color32[Enum.GetNames(typeof(ObjectsAndFloorings)).Length];
    public Color32[] ObjectsAndFlooringsColors => objectsAndFlooringsColors;

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
                ShowVisualization(GetElevationMap());
            }
            else if (currentVisualization == DevMapVisualizations.TileLocation)
            {
                ShowVisualization(GetLocationMap());
            }
            else if (currentVisualization == DevMapVisualizations.ObjectsAndFloorings)
            {
                ShowVisualization(GetObjectsAndFlooringsMap());
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

    private  Color32[,] GetObjectsAndFlooringsMap()
    {
        int mapSize = TileInformationManager.mapSize;

        Color32[,] colorMap = new Color32[mapSize, mapSize];

        Color32[] colorDict = ObjectsAndFlooringsColors;

        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                TileInformation info = TileInformationManager.Instance.GetTileInformation(pos);

                ObjectOnTile topMostObject = info.TopMostObject;

                Color32 proposedColor;
                if (topMostObject != null)
                {
                    switch (topMostObject.ModifiedType)
                    {
                        case (ObjectType.onTop):
                            proposedColor = colorDict[(int)ObjectsAndFloorings.OntopObject];
                            break;
                        case (ObjectType.standard):
                            proposedColor = colorDict[(int)ObjectsAndFloorings.StandardObject];
                            break;
                        case (ObjectType.ground):
                            proposedColor = colorDict[(int)ObjectsAndFloorings.GroundObject];
                            break;
                        default:
                            proposedColor = colorDict[(int)ObjectsAndFloorings.None];
                            break;
                    }
                }
                else if (info.NormalFlooringGroup != null)
                {
                    proposedColor = colorDict[(int)ObjectsAndFloorings.NormalFlooring];
                }
                else if (info.SupportFlooringGroup != null)
                {
                    proposedColor = colorDict[(int)ObjectsAndFloorings.SupportFlooring];
                }
                else
                {
                    proposedColor = colorDict[(int)ObjectsAndFloorings.None];
                }

                colorMap[x, y] = proposedColor;
            }
        }

        return colorMap;
    }

    private Color32[,] GetElevationMap()
    {
        int mapSize = TileInformationManager.mapSize;

        Color32[,] colorMap = new Color32[mapSize, mapSize];

        int elevationColorSize = ElevationColors.Length;

        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                Vector3Int pos = new Vector3Int(i, j, 0);
                int layerNum = TileInformationManager.Instance.GetTileInformation(pos).layerNum;

                if (layerNum != Constants.INVALID_TILE_LAYER)
                {
                    if (layerNum < elevationColorSize)
                    {
                        colorMap[i, j] = ElevationColors[layerNum];
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

    private Color32[,] GetLocationMap()
    {
        int mapSize = TileInformationManager.mapSize;

        Color32[,] colorMap = new Color32[mapSize, mapSize];

        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                TileInformation info = TileInformationManager.Instance.GetTileInformation(pos);

                int devVisualizationIndex = Array.IndexOf(Enum.GetValues(info.tileLocation.GetType()), info.tileLocation);

                Color32 color = TileLocationColors[devVisualizationIndex];
                colorMap[x, y] = color;
            }
        }

        return colorMap;
    }

    private enum DevMapVisualizations
    {
        None,
        Elevation,
        TileLocation,
        ObjectsAndFloorings
    }

    private enum ObjectsAndFloorings
    {
        OntopObject,
        StandardObject,
        GroundObject,
        NormalFlooring,
        SupportFlooring,
        None
    }
}
