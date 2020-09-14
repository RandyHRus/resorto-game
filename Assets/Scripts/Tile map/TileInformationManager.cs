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
    public bool Collision { get; private set; }
    public Dictionary<ObjectType, BuildOnTile> ObjectTypeToObject { get; private set; }
    public BuildOnTile TopMostBuild { get; private set; }
    public int[] waterBGTracker;

    private FlooringGroup normalFlooringGroup;
    public FlooringGroup NormalFlooringGroup
    {
        get
        {
            return normalFlooringGroup;
        }
        set
        {
            if (value != null && normalFlooringGroup != null)
                throw new System.Exception("Normal flooring already set!");

            normalFlooringGroup = value;
        }
    }

    private FlooringGroup supportFlooringGroup;
    public FlooringGroup SupportFlooringGroup
    {
        get
        {
            return supportFlooringGroup;
        }
        set
        {
            if (value != null && supportFlooringGroup != null)
                throw new System.Exception("Support flooring already set!");

            supportFlooringGroup = value;
        }
    }

    public delegate void TileSteppedDelegate();
    public event TileSteppedDelegate tileSteppedOnPlayer;
    public event TileSteppedDelegate tileSteppedOffPlayer;

    public TileInformation()
    {
        layerNum = 0;
        tileLocation = TileLocation.Unknown;       
        Collision = false;
        region = null;

        NormalFlooringGroup = null;
        SupportFlooringGroup = null;

        waterBGTracker = new int[4];

        ObjectTypeToObject = new Dictionary<ObjectType, BuildOnTile>
        {
            [ObjectType.OnTop] = null,
            [ObjectType.Standard] = null,
            [ObjectType.Ground] = null
        };

        TopMostBuild = null;
    }

    public void RefreshTile()
    {
        RefreshTopMostObject();
        RefreshCollision();
    }

    public FlooringGroup GetTopFlooringGroup()
    {
        return NormalFlooringGroup ?? SupportFlooringGroup ?? null;
    }

    private void RefreshCollision()
    {
        Collision = ((ObjectTypeToObject[ObjectType.OnTop] != null && ObjectTypeToObject[ObjectType.OnTop].BuildInfo.Collision) ||
                (ObjectTypeToObject[ObjectType.Standard] != null && ObjectTypeToObject[ObjectType.Standard].BuildInfo.Collision) ||
                (ObjectTypeToObject[ObjectType.Ground] != null && ObjectTypeToObject[ObjectType.Ground].BuildInfo.Collision));
    }

    public void RemoveAllBuilds()
    {
        while (TopMostBuild != null)
        {
            TopMostBuild.RemoveBuild();
        }
    }

    public void ResetTerrainInformation()
    {
        waterBGTracker = new int[4];
        tileLocation = TileLocation.Unknown;
        layerNum = 0;
    }

    public void SetTileObject(BuildOnTile objOnTile)
    {
        if (ObjectTypeToObject[objOnTile.ModifiedType] != null)
            throw new System.Exception("Object already exists!");

        ObjectTypeToObject[objOnTile.ModifiedType] = objOnTile;
        RefreshTile();
    }

    public void ClearTileObject(ObjectType type)
    {
        if (ObjectTypeToObject[type] == null)
            throw new System.Exception("Nothing to clear!");

        ObjectTypeToObject[type] = null;
        RefreshTile();
    }

    private void RefreshTopMostObject()
    {
        TopMostBuild = ObjectTypeToObject[ObjectType.OnTop] ?? ObjectTypeToObject[ObjectType.Standard] ?? ObjectTypeToObject[ObjectType.Ground] ?? null;
    }

    public void StepOn()
    {
        if (TopMostBuild != null && TopMostBuild.Functions != null)
            TopMostBuild.Functions.StepOn();

        tileSteppedOnPlayer?.Invoke();
    }

    public void StepOff()
    {
        if (TopMostBuild != null && TopMostBuild.Functions != null)
            TopMostBuild.Functions.StepOff();

        tileSteppedOffPlayer?.Invoke();
    }

    public void ClickInteract()
    {
        if (TopMostBuild != null && TopMostBuild.Functions != null)
            TopMostBuild.Functions.ClickInteract();
    }
}

public class BuildOnTile
{
    public ITileObjectFunctions Functions { get; private set; }
    public HashSet<Vector3Int> OccupiedTiles { get; private set; }
    public Vector2Int BottomLeft { get; private set; }
    public IBuildable BuildInfo { get; private set; }
    public GameObject GameObjectOnTile { get; private set; }
    public BuildRotation Rotation { get; private set; }
    public ObjectType ModifiedType { get; private set; }

    private HashSet<TileInformation> transparencySubscriptions;
    private int currentSteppedOnTilesCount;

    public delegate void OnBuildRemove(BuildOnTile sender);
    public event OnBuildRemove OnBuildRemoved;

    public BuildOnTile(GameObject gameObjectOnTile, IBuildable buildInfo, HashSet<Vector3Int> tilesToOccupy, BuildRotation rotation, ObjectType modifiedType, Vector2Int bottomLeft)
    {
        this.BuildInfo = buildInfo;
        this.GameObjectOnTile = gameObjectOnTile;
        this.Rotation = rotation;
        this.OccupiedTiles = tilesToOccupy;

        this.Functions = gameObjectOnTile.GetComponent<ITileObjectFunctions>();
        if (Functions != null)
            Functions.Initialize(this);

        this.ModifiedType = modifiedType;
        this.BottomLeft = bottomLeft;

        currentSteppedOnTilesCount = 0;

        Vector2Int sizeOnTile = buildInfo.GetSizeOnTile(rotation);
        Vector2Int transparencySubscribeBottomLeft = bottomLeft + new Vector2Int(0, sizeOnTile.y);

        //Set tiles
        foreach (Vector3Int t in tilesToOccupy)
        {
            TileInformation tileInfo = TileInformationManager.Instance.GetTileInformation(t);
            tileInfo.SetTileObject(this);
        }

        transparencySubscriptions = new HashSet<TileInformation>();
        //Subscripe to step events
        for (int i = transparencySubscribeBottomLeft.x; i < transparencySubscribeBottomLeft.x + sizeOnTile.x; i++)
        {
            for (int j = transparencySubscribeBottomLeft.y; j < transparencySubscribeBottomLeft.y + buildInfo.TransparencyCapableYSize; j++)
            {
                TileInformation tileInfo = TileInformationManager.Instance.GetTileInformation(new Vector3Int(i, j, 0));
                if (tileInfo == null)
                    continue;

                transparencySubscriptions.Add(tileInfo);

                tileInfo.tileSteppedOnPlayer += TileSteppedOnPlayerHandler;
                tileInfo.tileSteppedOffPlayer += TileSteppedOffPlayerHandler;
            }
        }
    }

    public void RemoveBuild()
    {
        //Actual destroy object part
        UnityEngine.Object.Destroy(GameObjectOnTile);

        //Clear tiles
        foreach (Vector3Int checkPos in OccupiedTiles)
        {
            TileInformationManager.Instance.GetTileInformation(checkPos).ClearTileObject(ModifiedType);
        }

        //Unsubscribe here
        foreach (TileInformation t in transparencySubscriptions)
        {
            t.tileSteppedOnPlayer -= TileSteppedOnPlayerHandler;
            t.tileSteppedOffPlayer -= TileSteppedOffPlayerHandler;
        }

        OnBuildRemoved?.Invoke(this);
    }

    private void TileSteppedOnPlayerHandler()
    {
        currentSteppedOnTilesCount++;
        if (currentSteppedOnTilesCount == 1)
        {
            BuildTransparencyManager.ToggleTransparencies(this, true);
        }
    }

    private void TileSteppedOffPlayerHandler()
    {
        currentSteppedOnTilesCount--;
        if (currentSteppedOnTilesCount == 0)
        {
            BuildTransparencyManager.ToggleTransparencies(this, false);
        }
    }
}

public class FlooringGroup
{
    public FlooringRotation Rotation { get; private set; }
    public Dictionary<Vector3Int, FlooringNormalPartOnTile> NormalFloorings { get; private set; }
    public HashSet<Vector3Int> SupportFloorings { get; private set; }
    private List<GameObject> supportObjects;  //Only used for docks for now
    public FlooringVariantBase FlooringVariant { get; private set; }
    public Vector2Int BottomLeft { get; private set; } //This is the bottom left EXCLUDING the supports
    public Vector2Int TopRight { get; private set; }
    private List<BuildOnTile> connectedBuilds; //Stairs if on dock etc...

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
        this.connectedBuilds = new List<BuildOnTile>();
    }

    public void AddConnectedBuild(BuildOnTile build)
    {
        connectedBuilds.Add(build);
        build.OnBuildRemoved += OnConnectedBuildRemoved;
    }

    private void OnConnectedBuildRemoved(BuildOnTile sender)
    {
        connectedBuilds.Remove(sender);
        sender.OnBuildRemoved -= OnConnectedBuildRemoved;
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

        for (int i = connectedBuilds.Count -1; i >= 0; i--)
        {
            if (!RemoveManager.TryRemoveBuild(connectedBuilds[i]))
            {
                throw new System.Exception("Failed to remove build for some reason...");
            }
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
    CliffRight = 1 << 6,
    CliffLeft = 1 << 7,
    CliffCornerCurveOut = 1 << 8,
    CliffCornerCurveIn = 1 << 9,
    CliffDoubleCorner = 1 << 10,

    Cliff = CliffFront | CliffBack | CliffRight | CliffLeft | CliffCornerCurveOut | CliffCornerCurveIn,
    Water = DeepWater | WaterEdge,
    Land = Grass | Sand
}