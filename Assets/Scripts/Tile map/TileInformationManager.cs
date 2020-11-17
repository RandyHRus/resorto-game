using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInformationManager : MonoBehaviour
{
    public static readonly int mapSize = 100;
    public static readonly int totalTilesCount = mapSize * mapSize;

    private TileInformation[,] tileInformationMap;

    private static TileInformationManager _instance;
    public static TileInformationManager Instance { get { return _instance; } }
    private void Awake()
    {
        //Singleton
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }
        //Initialize information map
        {
            tileInformationMap = new TileInformation[mapSize, mapSize];

            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    tileInformationMap[i, j] = new TileInformation(new Vector2Int(i,j));
                }
            }
        }
    }

    public bool TryGetTileInformation(Vector2Int position, out TileInformation tileInfo)
    {
        if (!PositionInMap(position))
        {
            tileInfo = null;
            return false;
        }
        else
        {
            tileInfo = tileInformationMap[position.x, position.y];
            return true;
        }
    }

    public bool PositionInMap(Vector2Int position)
    {
        return (position.x >= 0 && position.y >= 0 && position.x < mapSize && position.y < mapSize);
    }

    public Vector2Int GetMouseTile()
    {
        Vector2 mouseToWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector2Int(Mathf.RoundToInt(mouseToWorld.x), Mathf.RoundToInt(mouseToWorld.y));
    }
}