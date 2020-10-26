using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandObjectsGenerator : MonoBehaviour
{
    //[Header("Grass")]
    //[SerializeField] private ObjectInformation grassObjectInfo = null;
    //[SerializeField] private float tilesToGrassPatchesTryCountRatio = 0.003f;
    //[SerializeField] private int minGrassPatchRadius = 2;
    //[SerializeField] private int maxGrassPatchRadius = 3;  
    //[SerializeField] private float grassPatchDensity = 0.8f;

    [Header("Seashells")]
    [SerializeField] private ObjectInformation[] seashellObjectInfos = null;
    [SerializeField] private float tilesToSeashellsTryCountRatio = 0.003f;

    [Header("Bush")]
    [SerializeField] private ObjectInformation[] bushObjectInfos = null;
    [SerializeField] private float tilesToBushTryCountRatio = 0.003f;

    [Header("Trees")]
    [SerializeField] private ObjectInformation[] palmTreesInfo = null;
    [SerializeField] private float tilesToPalmTreesTryCountRatio = 0.002f;

    [Header("Driftwood")]
    [SerializeField] private ObjectInformation[] driftwoodInfo = null;
    [SerializeField] private float tilesToDriftwoodTryCountRatio = 0.002f;

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
        GenerateTrees();
        GenerateDriftwood();
    }

    public void RemoveAllBuilds()
    {
        int mapSize = TileInformationManager.mapSize;

        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                TileInformationManager.Instance.GetTileInformation(new Vector2Int(i, j)).RemoveAllBuilds();
            }
        }
    }
    /*
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
            Vector2Int middlePosition = new Vector2Int(randomX, randomY);

            int radius = Random.Range(minGrassPatchRadius, maxGrassPatchRadius + 1);

            for (int x = randomX - radius; x <= randomX + radius; x++)
            {
                for (int y = randomY - radius; y <= randomY + radius; y++)
                {
                    Vector2Int proposedPos = new Vector2Int(x, y);
                    float distanceToTile = Vector2.Distance(middlePosition, new Vector2(proposedPos.x, proposedPos.y));

                    if (distanceToTile > radius)
                        continue;

                    bool bSpawn = Random.Range(0f, 1f) < grassPatchDensity;

                    if (bSpawn)
                        TileObjectsManager.TryCreateObject(grassObjectInfo, proposedPos, out BuildOnTile buildOnTile);
                }
            }
        }
    }
    */

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
            Vector2Int proposedPos = new Vector2Int(randomX, randomY);

            if (TileInformationManager.Instance.GetTileInformation(proposedPos).tileLocation != TileLocation.Grass)
                continue;

            TileObjectsManager.TryCreateObject(bushObjectInfo, proposedPos, out BuildOnTile buildOnTile);
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
            Vector2Int proposedPos = new Vector2Int(randomX, randomY);

            if (TileInformationManager.Instance.GetTileInformation(proposedPos).tileLocation != TileLocation.Sand)
                continue;

            TileObjectsManager.TryCreateObject(seashellObjectInfo, proposedPos, out BuildOnTile buildOnTile);
        }
    }

    private void GenerateTrees()
    {
        int mapSize = TileInformationManager.mapSize;
        int tileCount = mapSize * mapSize;
        int palmTreesTryCount = (int)(tileCount * tilesToPalmTreesTryCountRatio);

        for (int c = 0; c < palmTreesTryCount; c++)
        {
            //Get random seashell
            ObjectInformation palmTreeObjectInfo = palmTreesInfo[Random.Range(0, palmTreesInfo.Length)];

            //Get random point
            int randomX = Random.Range(0, mapSize);
            int randomY = Random.Range(0, mapSize);
            Vector2Int proposedPos = new Vector2Int(randomX, randomY);

            if (TileInformationManager.Instance.GetTileInformation(proposedPos).tileLocation != TileLocation.Sand)
                continue;

            TileObjectsManager.TryCreateObject(palmTreeObjectInfo, proposedPos, out BuildOnTile buildOnTile);
        }
    }

    private void GenerateDriftwood()
    {
        int mapSize = TileInformationManager.mapSize;
        int tileCount = mapSize * mapSize;
        int driftwoodTryCount = (int)(tileCount * tilesToDriftwoodTryCountRatio);

        for (int c = 0; c < driftwoodTryCount; c++)
        {
            //Get random seashell
            ObjectInformation driftwoodObjectInfo = driftwoodInfo[Random.Range(0, driftwoodInfo.Length)];

            //Get random point
            int randomX = Random.Range(0, mapSize);
            int randomY = Random.Range(0, mapSize);
            Vector2Int proposedPos = new Vector2Int(randomX, randomY);

            if (!TileObjectsManager.ObjectPlaceable(proposedPos, driftwoodObjectInfo, out ObjectType modifiedType, out float yOffset))
                continue;

            bool placeable = true;
            for (int i = 0; i < driftwoodObjectInfo.GetSizeOnTile(0).x; i++)
            {
                for (int j = 0; j < driftwoodObjectInfo.GetSizeOnTile(0).y; j++)
                {
                    Vector2Int thisLocation = proposedPos + new Vector2Int(i, j);
                    TileInformation tileInfo = TileInformationManager.Instance.GetTileInformation(thisLocation);
                    if (!(tileInfo.tileLocation == TileLocation.Sand || tileInfo.tileLocation == TileLocation.WaterEdge))
                        placeable = false;
                }
            }

            if (!placeable)
                continue;

            TileObjectsManager.TryCreateObject(driftwoodObjectInfo, proposedPos, out BuildOnTile buildOnTile);
        }
    }
}
