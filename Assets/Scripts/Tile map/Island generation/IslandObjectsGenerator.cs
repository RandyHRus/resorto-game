using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandObjectsGenerator : MonoBehaviour
{
    [Header("Grass")]
    [SerializeField] private ObjectInformation grassObjectInfo = null;
    [SerializeField] private float tilesToGrassPatchesTryCountRatio = 0.003f;
    [SerializeField] private int minGrassPatchRadius = 2;
    [SerializeField] private int maxGrassPatchRadius = 3;  
    [SerializeField] private float grassPatchDensity = 0.8f;

    [Header("Seashells")]
    [SerializeField] private ObjectInformation[] seashellObjectInfos = null;
    [SerializeField] private float tilesToSeashellsTryCountRatio = 0.003f;

    [Header("Bush")]
    [SerializeField] private ObjectInformation[] bushObjectInfos = null;
    [SerializeField] private float tilesToBushTryCountRatio = 0.003f;

    private static IslandObjectsGenerator _instance;
    public static IslandObjectsGenerator Instance { get { return _instance; } }
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

    public void GenerateIslandObjects()
    {
        //GenerateGrass();
        GenerateSeashells();
        GenerateBush();
    }

    private void GenerateGrass()
    {
        int mapSize = TileInformationManager.mapSize;
        int tileCount = mapSize * mapSize;

        int grassPatchesTryCount = (int)(tileCount * tilesToGrassPatchesTryCountRatio);

        for (int i = 0; i < grassPatchesTryCount; i++)
        {
            //Get random point
            int randomX = Random.Range(0, mapSize);
            int randomY = Random.Range(0, mapSize);
            Vector3Int middlePosition = new Vector3Int(randomX, randomY, 0);

            int radius = Random.Range(minGrassPatchRadius, maxGrassPatchRadius + 1);

            for (int x = randomX - radius; x <= randomX + radius; x++)
            {
                for (int y = randomY - radius; y <= randomY + radius; y++)
                {
                    Vector3Int proposedPos = new Vector3Int(x, y, 0);
                    float distanceToTile = Vector2.Distance(new Vector2(middlePosition.x, middlePosition.y), new Vector2(proposedPos.x, proposedPos.y));

                    if (distanceToTile > radius)
                        continue;

                    bool bSpawn = Random.Range(0f, 1f) < grassPatchDensity;

                    if (bSpawn)
                        TileObjectsManager.TryCreateObject(grassObjectInfo, proposedPos, out ObjectOnTile objectOnTile);
                }
            }
        }
    }

    private void GenerateBush()
    {
        int mapSize = TileInformationManager.mapSize;
        int tileCount = mapSize * mapSize;
        int bushTryCount = (int)(tileCount * tilesToBushTryCountRatio);

        for (int c = 0; c < bushTryCount; c++)
        {
            //Get random bush
            ObjectInformation bushObjectInfo = bushObjectInfos[Random.Range(0, bushObjectInfos.Length)];

            //Get random point
            int randomX = Random.Range(0, mapSize);
            int randomY = Random.Range(0, mapSize);
            Vector3Int proposedPos = new Vector3Int(randomX, randomY, 0);

            if (TileInformationManager.Instance.GetTileInformation(proposedPos).tileLocation != TileLocation.Grass)
                continue;

            TileObjectsManager.TryCreateObject(bushObjectInfo, proposedPos, out ObjectOnTile objectOnTile);
        }
    }

    private void GenerateSeashells()
    {
        int mapSize = TileInformationManager.mapSize;
        int tileCount = mapSize * mapSize;
        int seashellsTryCount = (int)(tileCount * tilesToSeashellsTryCountRatio);

        for (int c = 0; c < seashellsTryCount; c++)
        {
            //Get random seashell
            ObjectInformation seashellObjectInfo = seashellObjectInfos[Random.Range(0, seashellObjectInfos.Length)];

            //Get random point
            int randomX = Random.Range(0, mapSize);
            int randomY = Random.Range(0, mapSize);
            Vector3Int proposedPos = new Vector3Int(randomX, randomY, 0);

            if (TileInformationManager.Instance.GetTileInformation(proposedPos).tileLocation != TileLocation.Sand)
                continue;

            TileObjectsManager.TryCreateObject(seashellObjectInfo, proposedPos, out ObjectOnTile objectOnTile);
        }
    }
}
