using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class IslandGenerator : MonoBehaviour
{
    [SerializeField] private float noiseScale = 1f;
    [SerializeField] private int octaves = 1;
    [SerializeField] private float persistance = 2f;
    [SerializeField] private float lacunarity = 2f;
    [SerializeField] private Tilemap waterTilemap = null;
    [SerializeField] private Tilemap waterBGTilemap = null;
    [SerializeField] private TileBase waterTile = null;
    [SerializeField] private TileBase waterBGFullDarkTile = null;

    [SerializeField] private int maxLayerHeight = 7; //For layer heights, -1 for water, 0 for sand, 1& above for land
    private int mapWidth = TileInformationManager.tileCountX;
    private int mapHeight = TileInformationManager.tileCountY;

    public delegate void OnMapLoadDelegate();
    public static event OnMapLoadDelegate OnMapLoad;

    private void Start()
    {
        //Create water tilemap (and water background
        {
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    waterTilemap.SetTile(pos, waterTile);
                    waterBGTilemap.SetTile(pos, waterBGFullDarkTile);
                    TileInformationManager.Instance.GetTileInformation(pos).tileLocation = TileLocation.DeepWater;
                }
            }
        }

        float[,] mapData = GenerateMapData();

        int[,] layerHeightMap = new int[mapWidth, mapHeight];

        bool[,,] layerHeightToBCreateTerrain = new bool[maxLayerHeight+1, mapWidth, mapHeight]; //Each element in array is false by default

        //Determine which layers to create terrain on
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                int layerNum = (int)Mathf.Lerp(-1, maxLayerHeight, mapData[x, y]);
                layerHeightMap[x, y] = layerNum;

                for (int j = layerNum; j >= 0; j--)
                {
                    layerHeightToBCreateTerrain[j, x, y] = true;
                }
            }
        }

        //Actual terrain creation;
        for (int i = 0; i < maxLayerHeight; i++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    bool toCreate = layerHeightToBCreateTerrain[i, x, y];
                    if (toCreate) {
                        if (i == 0)
                        {
                            TerrainManager.Instance.TryCreateSand(new Vector3Int(x, y, 0));
                        }
                        else
                        {
                            TerrainManager.Instance.TryCreateLand(new Vector3Int(x, y, 0), i);
                        }
                    }
                }
            }
        }

        OnMapLoad?.Invoke();
    }

    private float[,] GenerateMapData()
    {
        float[,] noiseMap = GenerateNoiseMap();
        float[,] falloffMap = GenerateFalloffMap();

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
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



        float[,] noiseMap = new float[mapWidth, mapHeight];

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
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

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                //InverseLerp will return 0 to 1
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }

    private float[,] GenerateFalloffMap()
    {
        float[,] map = new float[mapWidth, mapHeight];

        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                float x = i / (float)mapWidth * 2 - 1;
                float y = j / (float)mapHeight * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));

                map[i, j] = value;
            }
        }

        return map;
    }
}