using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInformationManager : MonoBehaviour
{
    public static readonly int mapSize = 120;

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
    public RegionInformation region;
    public ObjectOnTile TopMostObject { get; private set; }
    public bool Collision { get; private set; }
    public Dictionary<ObjectType, ObjectOnTile> ObjectTypeToObject { get; private set; }
    public FlooringGroup NormalFlooringGroup { get; private set; }
    public FlooringGroup SupportFlooringGroup { get; private set; }
    public int[] waterBGTracker;

    public TileInformation()
    {
        TopMostObject = null;
        layerNum = 0;
        tileLocation = TileLocation.Unknown;       
        Collision = false;
        region = null;

        ObjectTypeToObject = new Dictionary<ObjectType, ObjectOnTile>
        {
            [ObjectType.onTop] = null,
            [ObjectType.standard] = null,
            [ObjectType.ground] = null
        };

        NormalFlooringGroup = null;
        SupportFlooringGroup = null;

        waterBGTracker = new int[4];
    }

    public void SetTileObject(ObjectType type, ObjectOnTile objOnTile)
    {
        ObjectTypeToObject[type] = objOnTile;
        RefreshTopMostObject();
        RefreshCollision();
    }

    public void RemoveTopMostTileObject()
    {
        if (TopMostObject != null)
        {
            //Actual destroy object part
            UnityEngine.Object.Destroy(TopMostObject.GameObjectOnTile);

            //Clear tiles
            foreach (Vector3Int checkPos in TopMostObject.OccupiedTiles)
            {
                TileInformationManager.Instance.GetTileInformation(checkPos).ClearTileObject(TopMostObject.ModifiedType);
            }
        }
    }

    private void ClearTileObject(ObjectType type)
    {
        ObjectTypeToObject[type] = null;
        RefreshTopMostObject();
        RefreshCollision();
    }

    public void SetNormalFlooringGroup(FlooringGroup flooringGroup)
    {
        this.NormalFlooringGroup = flooringGroup;
    }

    public void SetSupportFlooringGroup(FlooringGroup flooringGroup)
    {
        this.SupportFlooringGroup = flooringGroup;
    }

    public FlooringGroup GetTopFlooringGroup()
    {
        return NormalFlooringGroup != null ? NormalFlooringGroup : SupportFlooringGroup;
    }

    public void RemoveFlooring()
    {
        FlooringGroup topGroup = GetTopFlooringGroup();

        if (topGroup == null)
            return;

        topGroup.Destroy();

        //Clear tiles
        {
            //Clear normal floorings
            foreach (KeyValuePair<Vector3Int, FlooringNormalPartOnTile> pair in topGroup.NormalFloorings)
            {
                TileInformation tileInfo = TileInformationManager.Instance.GetTileInformation(pair.Key);
                tileInfo.NormalFlooringGroup = null;
            }

            //Clear support floorings
            foreach (Vector3Int s in topGroup.SupportFloorings)
            {
                TileInformation tileInfo = TileInformationManager.Instance.GetTileInformation(s);
                tileInfo.SupportFlooringGroup = null;
            }
        }
    }

    private void RefreshTopMostObject()
    {
        if (ObjectTypeToObject[ObjectType.onTop] != null)
            TopMostObject = ObjectTypeToObject[ObjectType.onTop];
        else if (ObjectTypeToObject[ObjectType.standard] != null)
            TopMostObject = ObjectTypeToObject[ObjectType.standard];
        else if (ObjectTypeToObject[ObjectType.ground] != null)
            TopMostObject = ObjectTypeToObject[ObjectType.ground];
        else
            TopMostObject = null;
    }

    public bool TileIsEmpty()
    {
        return (ObjectTypeToObject[ObjectType.onTop] == null &&
                ObjectTypeToObject[ObjectType.standard] == null &&
                ObjectTypeToObject[ObjectType.ground] == null);
    }

    public void StepOn()
    {
        if (TopMostObject != null && TopMostObject.Functions != null)
            TopMostObject.Functions.StepOn();
    }

    public void StepOff()
    {
        if (TopMostObject != null && TopMostObject.Functions != null)
            TopMostObject.Functions.StepOff();
    }

    public void ClickInteract()
    {
        if (TopMostObject != null && TopMostObject.Functions != null)
            TopMostObject.Functions.ClickInteract();
    }

    private void RefreshCollision()
    {
        Collision = ((ObjectTypeToObject[ObjectType.onTop] != null    && ObjectTypeToObject[ObjectType.onTop].ObjectInfo.Collision) || 
                (ObjectTypeToObject[ObjectType.standard] != null && ObjectTypeToObject[ObjectType.standard].ObjectInfo.Collision) || 
                (ObjectTypeToObject[ObjectType.ground] != null   && ObjectTypeToObject[ObjectType.ground].ObjectInfo.Collision));
    }
}

public class ObjectOnTile
{
    public ObjectOnTile(GameObject gameObjectOnTile, ObjectInformation objectInfo, List<Vector3Int> occupiedTiles, ObjectRotation rotation, ObjectType modifiedType)
    {
        this.ObjectInfo = objectInfo;
        this.GameObjectOnTile = gameObjectOnTile;
        this.Rotation = rotation;
        this.OccupiedTiles = occupiedTiles;

        this.Functions = gameObjectOnTile.GetComponent<ITileObjectFunctions>();
        if (Functions != null)
            Functions.Initialize(this);

        this.ModifiedType = modifiedType;
    }

    public ITileObjectFunctions Functions { get; private set; }
    public List<Vector3Int> OccupiedTiles { get; private set; }
    public ObjectInformation ObjectInfo { get; private set; }
    public GameObject GameObjectOnTile { get; private set; }
    public ObjectRotation Rotation { get; private set; }
    public ObjectType ModifiedType { get; private set; }
}

public class FlooringGroup
{
    public FlooringRotation Rotation { get; private set; }
    public Dictionary<Vector3Int, FlooringNormalPartOnTile> NormalFloorings { get; private set; }
    public HashSet<Vector3Int> SupportFloorings { get; private set; }
    private List<GameObject> supportObjects;
    public FlooringVariantBase FlooringVariant { get; private set; }
    public Vector2Int BottomLeft { get; private set; } //This is the bottom left EXCLUDING the supports
    public Vector2Int TopRight { get; private set; }

    public FlooringGroup(Dictionary<Vector3Int, FlooringNormalPartOnTile> normalFloorings, HashSet<Vector3Int> supportFloorings,
        Vector2Int bottomLeft, Vector2Int topRight, List<GameObject> supportObjects, FlooringVariantBase flooringVariant, FlooringRotation rotation)
    {
        this.NormalFloorings = normalFloorings;
        this.SupportFloorings = supportFloorings;
        this.FlooringVariant = flooringVariant;
        this.Rotation = rotation;
        this.supportObjects = supportObjects;
        this.BottomLeft = bottomLeft;
        this.TopRight = topRight;
    }

    public void Destroy()
    {
        foreach (KeyValuePair<Vector3Int, FlooringNormalPartOnTile> f in NormalFloorings)
        {
            UnityEngine.Object.Destroy(f.Value.GameObjectOnTile);
        }

        foreach (GameObject s in supportObjects)
        {
            UnityEngine.Object.Destroy(s);
        }
    }
}

public class FlooringNormalPartOnTile
{
    public GameObject GameObjectOnTile { get; private set; }
    public SpriteRenderer Renderer { get; private set; }
    //Should be the support that is set BELOW this tile

    public FlooringNormalPartOnTile(GameObject gameObjectOnTile)
    {
        this.GameObjectOnTile = gameObjectOnTile;
        Renderer = gameObjectOnTile.GetComponent<SpriteRenderer>();
    }
}

[Flags]
public enum TileLocation
{
    Unknown = 0,

    DeepWater = 1 << 0,
    WaterEdge = 1 << 1,
    Grass = 1 << 2,
    Sand = 1 << 3,
    CliffFront = 1 << 4,
    CliffBack = 1 << 5,

    Water = DeepWater | WaterEdge,
    Cliff = CliffFront | CliffBack,
    Land = Grass | Sand
}