using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class IslandTerrainGenerator : MonoBehaviour
{
    [SerializeField] private float noiseScale = 1f;
    [SerializeField] private int octaves = 1;
    [SerializeField] private float persistance = 2f;
    [SerializeField] private float lacunarity = 2f;
    [SerializeField] private int maxLayerHeight = 7; //For layer heights, -1 for water, 0 for sand, 1& above for land

    [SerializeField] private Tilemap waterTilemap = null;
    [SerializeField] private Tilemap waterBGTilemap = null;
    [SerializeField] private TileBase waterTile = null;
    [SerializeField] private TileBase waterBGFullDarkTile = null;

    private readonly int waterPaddingOutsideMap = 50;

    private readonly int mapSize = TileInformationManager.mapSize;

    private static IslandTerrainGenerator _instance;
    public static IslandTerrainGenerator Instance { get { return _instance; } }
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
    }

    public void GenerateNewIsland()
    {
        float[,] mapData = GenerateMapData();

        int[,] layerHeightMap = new int[mapSize, mapSize];

        //Determine which layers to create terrain on
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                int layerNum = (int)Mathf.Lerp(-1, maxLayerHeight, mapData[x, y]);
                layerHeightMap[x, y] = layerNum;
            }
        }

        GenerateIslandTerrain(layerHeightMap);
    }

    //LayerHeightMap should be:
    // -1 for water
    // 0 for sand
    // 1 and above for land
    public void GenerateIslandTerrain(int[,] layerHeightMap)
    {
        //Create water tilemap (and water background
        {
            for (int x = -waterPaddingOutsideMap; x < mapSize + waterPaddingOutsideMap; x++)
            {
                for (int y = -waterPaddingOutsideMap; y < mapSize + waterPaddingOutsideMap; y++)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    waterTilemap.SetTile((Vector3Int)pos, waterTile);
                    waterBGTilemap.SetTile((Vector3Int)pos, waterBGFullDarkTile);

                    if (TileInformationManager.Instance.TryGetTileInformation(pos, out TileInformation tileInfo))
                        tileInfo.tileLocation = TileLocation.DeepWater;
                }
            }
        }

        for (int i = 0; i < maxLayerHeight; i++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    if (layerHeightMap[x, y] >= i)
                    {
                        if (i == 0)
                        {
                            TerrainManager.Instance.TryCreateSand(new Vector2Int(x, y));
                        }
                        else
                        {
                            TerrainManager.Instance.TryCreateLand(new Vector2Int(x, y), i);
                        }
                    }
                }
            }
        }
    }

    //Returns float between 0 to 1
    private float[,] GenerateMapData()
    {
        float[,] noiseMap = GenerateNoiseMap();
        float[,] falloffMap = GenerateFalloffMap();

        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
            }
        }

        return noiseMap;
    }

    private float[,] GenerateNoiseMap()
    {
        if (noiseScale <= 0)
        {
            Debug.Log("Scale cannot be 0!");
            noiseScale = 0.0001f;
        }

        int generateSeed = (int)System.DateTime.Now.Ticks;
        System.Random prng = new System.Random(generateSeed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000);
            float offsetY = prng.Next(-100000, 100000);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }



        float[,] noiseMap = new float[mapSize, mapSize];

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = x / noiseScale * frequency + octaveOffsets[i].x;
                    float sampleY = y / noiseScale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude += persistance;
                    frequency += lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                    maxNoiseHeight = noiseHeight;
                else if (noiseHeight < minNoiseHeight)
                    minNoiseHeight = noiseHeight;

                noiseMap[x, y] = noiseHeight;
            }
        }

        //Normalize
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                //InverseLerp will return 0 to 1
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }

    private float[,] GenerateFalloffMap()
    {
        float center = mapSize / 2f;

        float maxDistanceFromCenter = (mapSize / 2);
        float minDistanceFromCenter = float.MaxValue;

        float[,] map = new float[mapSize, mapSize];

        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                float distanceX = (center - x) * (center - x);
                float distanceY = (center - y) * (center - y);

                float distanceToCenter = Mathf.Sqrt(distanceX + distanceY);

                if (distanceToCenter > maxDistanceFromCenter)
                    distanceToCenter = maxDistanceFromCenter; //If outside maxDistance, we don't want to create land


                if (distanceToCenter < minDistanceFromCenter)
                    minDistanceFromCenter = distanceToCenter;

                map[x, y] = distanceToCenter;
            }
        }

        //Normalize
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                //InverseLerp will return 0 to 1
                map[x, y] =  Mathf.InverseLerp(minDistanceFromCenter, maxDistanceFromCenter, map[x, y]);
            }
        }

        return map;
    }
}