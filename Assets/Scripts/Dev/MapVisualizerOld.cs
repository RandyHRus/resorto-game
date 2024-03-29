﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapVisualizerOld : MonoBehaviour
{
    [SerializeField] private Tilemap colorTilemap = null;
    [SerializeField] private Tile indicatorTile = null;

    [EnumNamedArray(typeof(TileLocation)), SerializeField]
    private Color32[] tileLocationColors = new Color32[Enum.GetNames(typeof(TileLocation)).Length];
    public Color32[] TileLocationColors => tileLocationColors;

    [EnumNamedArray(typeof(ObjectsAndFloorings)), SerializeField]
    private Color32[] objectsAndFlooringsColors = new Color32[Enum.GetNames(typeof(ObjectsAndFloorings)).Length];
    public Color32[] ObjectsAndFlooringsColors => objectsAndFlooringsColors;

    private DevMapVisualizations currentVisualization = DevMapVisualizations.None;

    private void Update()
    {
        /*
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
                {
                    colorTilemap.ClearAllTiles();
                    colorTilemap.gameObject.SetActive(false);
                }
                else
                    colorTilemap.gameObject.SetActive(true);
            }

            Color32[,] proposedVisualization = null;

            switch (currentVisualization)
            {
                case (DevMapVisualizations.TileLocation):
                    proposedVisualization = GetLocationMap();
                    break;
                case (DevMapVisualizations.ObjectsAndFloorings):
                    proposedVisualization = GetObjectsAndFlooringsMap();
                    break;
                case (DevMapVisualizations.Stairs):
                    proposedVisualization = GetStairsMap();
                    break;
                case (DevMapVisualizations.FishingPositions):
                    proposedVisualization = GetFishingPositionsMap();
                    break;
                case (DevMapVisualizations.None):
                    break;
                default:
                    throw new System.NotImplementedException();
            }

            if (proposedVisualization != null)
                ShowVisualization(proposedVisualization);
        }
        */
    }

    private void ShowVisualization(Color32[,] colorMap)
    {
        colorTilemap.ClearAllTiles();

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

    private Color32[,] GetObjectsAndFlooringsMap()
    {
        int mapSize = TileInformationManager.mapSize;

        Color32[,] colorMap = new Color32[mapSize, mapSize];

        Color32[] colorDict = ObjectsAndFlooringsColors;

        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                TileInformationManager.Instance.TryGetTileInformation(pos, out TileInformation info);

                Color32 proposedColor;

                BuildOnTile topMostBuild = info.TopMostBuild;
                if (topMostBuild != null)
                {
                    switch (topMostBuild.ModifiedType)
                    {
                        case (ObjectType.OnTop):
                            proposedColor = colorDict[(int)ObjectsAndFloorings.OntopObject];
                            break;
                        case (ObjectType.Standard):
                            proposedColor = colorDict[(int)ObjectsAndFloorings.StandardObject];
                            break;
                        case (ObjectType.Ground):
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

    private Color32[,] GetLocationMap()
    {
        int mapSize = TileInformationManager.mapSize;

        Color32[,] colorMap = new Color32[mapSize, mapSize];

        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                TileInformationManager.Instance.TryGetTileInformation(pos, out TileInformation info);

                int devVisualizationIndex = Array.IndexOf(Enum.GetValues(info.tileLocation.GetType()), info.tileLocation);

                Color32 color = TileLocationColors[devVisualizationIndex];
                colorMap[x, y] = color;
            }
        }

        return colorMap;
    }

    private Color32[,] GetStairsMap()
    {
        int mapSize = TileInformationManager.mapSize;

        Color32[,] colorMap = new Color32[mapSize, mapSize];

        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                TileInformationManager.Instance.TryGetTileInformation(pos, out TileInformation info);

                int stairsConnectionsCount = info.StairsStartPositions.Count;

                Color32 color = objectsAndFlooringsColors[stairsConnectionsCount];
                colorMap[x, y] = color;
            }
        }

        return colorMap;
    }

    private Color32[,] GetFishingPositionsMap()
    {
        int mapSize = TileInformationManager.mapSize;

        Color32[,] colorMap = new Color32[mapSize, mapSize];

        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                TileInformationManager.Instance.TryGetTileInformation(pos, out TileInformation info);

                Color32 color;
                if (info.Region?.regionInformation.GetType() == typeof(FishingRegionInformation))
                {
                    if (((FishingRegionInstance)info.Region).IsValidFishingPositionInThisRegion(pos))
                    {
                        color = objectsAndFlooringsColors[0];
                    }
                    else
                    {
                        color = objectsAndFlooringsColors[1];
                    }
                }
                else
                {
                    color = objectsAndFlooringsColors[1];
                }

                colorMap[x, y] = color;
            }
        }

        return colorMap;
    }

    private enum DevMapVisualizations
    {
        None,
        TileLocation,
        ObjectsAndFloorings,
        Stairs,
        FishingPositions
    }

    private enum ObjectsAndFloorings
    {
        OntopObject,
        StandardObject,
        GroundObject,
        NormalFlooring,
        SupportFlooring,
        Structure,
        None
    }
}