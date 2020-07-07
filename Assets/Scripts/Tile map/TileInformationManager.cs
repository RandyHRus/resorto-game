using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInformationManager : MonoBehaviour
{
    public static int mapSize = 120;

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
                    tileInformationMap[i, j] = new TileInformation();
                }
            }
        }
    }

    public TileInformation GetTileInformation(Vector3Int position)
    {
        if (!PositionInMap(position))
        {
            return null;
        }
        else
        {
            return tileInformationMap[position.x, position.y];
        }
    }

    public bool PositionInMap(Vector3Int position)
    {
        return (position.x >= 0 && position.y >= 0 && position.x < mapSize && position.y < mapSize);
    }

    public Vector3Int GetMouseTile()
    {
        Vector2 mouseToWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector3Int(Mathf.RoundToInt(mouseToWorld.x), Mathf.RoundToInt(mouseToWorld.y), 0);
    }
}

public class TileInformation
{
    public int layerNum; //Water or sand is 0, land will be >1
    public TileLocation tileLocation;

    private ObjectOnTile topMostObject;

    private int regionId;

    public bool Collision { get; private set; }

    public Dictionary<ObjectType, ObjectOnTile> objectTypeToObject { get; private set; }

    public int[] waterBGTracker;

    public TileInformation()
    {
        topMostObject = null;
        layerNum = 0;
        tileLocation = TileLocation.Unknown;       
        Collision = false;
        regionId = 0;
        waterBGTracker = new int[4];

        objectTypeToObject = new Dictionary<ObjectType, ObjectOnTile>();
        objectTypeToObject[ObjectType.onTop] = null;
        objectTypeToObject[ObjectType.standard] = null;
        objectTypeToObject[ObjectType.ground] = null;
    }

    public void SetTileObject(GameObject obj, ObjectInformation info, ObjectType type, List<Vector3Int> occupiedTiles, ObjectRotation rotation = ObjectRotation.front)
    {
        objectTypeToObject[type] = new ObjectOnTile(obj, info, occupiedTiles, rotation, this, type);
        topMostObject = GetTopMostObject();
        Collision = CheckForCollision();
    }

    public void SetRegion(int id)
    {
        regionId = id;
    }

    public RegionInformation GetRegionInformation()
    {
        if (RegionInformationManager.Instance.regionInformationMap.TryGetValue(regionId, out RegionInformation info))
            return info;
        else
            return null;
    }

    public void RemoveTileObject(ObjectType type)
    {
        objectTypeToObject[type] = null;
        topMostObject = GetTopMostObject();
        Collision = CheckForCollision();
    }

    private ObjectOnTile GetTopMostObject()
    {
        ObjectType topMostObjectType = GetTopMostObjectType();
        if (topMostObjectType == ObjectType.None)
            return null;
        else
            return objectTypeToObject[topMostObjectType];
    }

    public ObjectType GetTopMostObjectType()
    {
        if (objectTypeToObject[ObjectType.onTop] != null)
            return ObjectType.onTop;
        else if (objectTypeToObject[ObjectType.standard] != null)
            return ObjectType.standard;
        else if (objectTypeToObject[ObjectType.ground] != null)
            return ObjectType.ground;
        else
            return ObjectType.None;
    }

    public bool TileIsEmpty()
    {
        return (objectTypeToObject[ObjectType.onTop] == null && 
                objectTypeToObject[ObjectType.standard] == null && 
                objectTypeToObject[ObjectType.ground] == null);
    }

    public void StepOn()
    {
        if (topMostObject != null && topMostObject.Functions != null)
            topMostObject.Functions.StepOn();
    }

    public void StepOff()
    {
        if (topMostObject != null && topMostObject.Functions != null)
            topMostObject.Functions.StepOff();
    }

    public void ClickInteract()
    {
        if (topMostObject != null && topMostObject.Functions != null)
            topMostObject.Functions.ClickInteract();
    }

    private bool CheckForCollision()
    {
        return ((objectTypeToObject[ObjectType.onTop] != null    && objectTypeToObject[ObjectType.onTop].ObjectInfo.Collision) || 
                (objectTypeToObject[ObjectType.standard] != null && objectTypeToObject[ObjectType.standard].ObjectInfo.Collision) || 
                (objectTypeToObject[ObjectType.ground] != null   && objectTypeToObject[ObjectType.ground].ObjectInfo.Collision));
    }
}

public class ObjectOnTile
{
    public ObjectOnTile(GameObject gameObjectOnTile, ObjectInformation objectInfo, List<Vector3Int> occupiedTiles, ObjectRotation rotation, TileInformation parentTile, ObjectType modifiedType)
    {
        this.ObjectInfo = objectInfo;
        this.GameObjectOnTile = gameObjectOnTile;
        this.Rotation = rotation;
        this.OccupiedTiles = occupiedTiles;

        this.Functions = gameObjectOnTile.GetComponent<ITileObjectFunctions>();
        if (Functions != null)
            Functions.Initialize(this);

        this.modifiedType = modifiedType;
        this.parentTile = parentTile;
    }

    public void RemoveFromTile()
    {
        TileObjectsManager.Instance.RemoveObject(parentTile, modifiedType);
    }

    public ITileObjectFunctions Functions { get; private set; }

    public List<Vector3Int> OccupiedTiles { get; private set; }

    public ObjectInformation ObjectInfo { get; private set; }

    public GameObject GameObjectOnTile { get; private set; }

    public ObjectRotation Rotation { get; private set; }

    private TileInformation parentTile;
    private ObjectType modifiedType;
}