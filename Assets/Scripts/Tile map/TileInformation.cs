using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TileInformation
{
    public int layerNum; //Water or sand is 0, land will be >1
    public TileLocation tileLocation;
    public RegionInstance Region { get; private set; }
    public bool BuildCollision { get; private set; }
    public Dictionary<ObjectType, BuildOnTile> ObjectTypeToObject { get; private set; }
    public BuildOnTile TopMostBuild { get; private set; }
    public int[] waterBGTracker;
    public readonly Vector2Int position;

    public Stairs StairsOnTile { get; private set; }
    public List<StairsStartPosition> StairsStartPositions { get; private set; }

    //The 4 neighbours
    public readonly Vector2Int[] neighbours;

    public FlooringGroup TopFlooringGroup => NormalFlooringGroup ?? SupportFlooringGroup ?? null;
    public FlooringGroup NormalFlooringGroup { get; private set; }
    public FlooringGroup SupportFlooringGroup { get; private set; }

    public delegate void TileSteppedDelegate();
    public event TileSteppedDelegate TileSteppedOnPlayer;
    public event TileSteppedDelegate TileSteppedOffPlayer;

    public delegate void TileModifiedDelegate(TileInformation tileInfo);
    public event TileModifiedDelegate OnTileModified;

    private static ComponentsListPanel<RightClickOptionUI> shownRightClickListUI;

    public TileInformation(Vector2Int position)
    {
        this.position = position;
        neighbours = new Vector2Int[]
        {
            new Vector2Int(position.x + 1,  position.y),
            new Vector2Int(position.x - 1,  position.y),
            new Vector2Int(position.x,      position.y + 1),
            new Vector2Int(position.x,      position.y - 1)
        };

        layerNum = 0;
        tileLocation = TileLocation.Unknown;
        BuildCollision = false;
        Region = null;

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
        StairsStartPositions = new List<StairsStartPosition>();
    }

    public BuildOnTile CreateBuild(GameObject gameObjectOnTile, IBuildable buildInfo, HashSet<Vector2Int> tilesToOccupy, BuildRotation rotation, ObjectType modifiedType)
    {
        BuildOnTile buildOnTile = new BuildOnTile(gameObjectOnTile, buildInfo, tilesToOccupy, rotation, modifiedType, position);
        //Set tiles
        foreach (Vector2Int t in tilesToOccupy)
        {
            TileInformationManager.Instance.TryGetTileInformation(t, out TileInformation tileInfo);
            tileInfo.SetTileBuild(buildOnTile);
        }

        buildInfo.OnCreate(buildOnTile);

        return buildOnTile;
    }

    private void SetTileBuild(BuildOnTile objOnTile)
    {
        if (ObjectTypeToObject[objOnTile.ModifiedType] != null)
            throw new System.Exception("Object already exists!");

        objOnTile.OnBuildRemoved += OnBuildRemoved;

        ObjectTypeToObject[objOnTile.ModifiedType] = objOnTile;
        RefreshTile();

        OnTileModified?.Invoke(this);
    }

    private bool BuildRemovable(out BuildOnTile buildOnTile)
    {
        buildOnTile = null;

        if (TopMostBuild == null)
            return false;

        //Check if there is any objects on above, if there is, it cannot be destroyed
        foreach (Vector2Int checkPos in TopMostBuild.OccupiedTiles)
        {
            TileInformationManager.Instance.TryGetTileInformation(checkPos, out TileInformation thisInfo);

            if (thisInfo.TopMostBuild != TopMostBuild)
                return false;
        }

        buildOnTile = TopMostBuild;
        return true;
    }

    private void OnBuildRemoved(BuildOnTile sender)
    {
        if (ObjectTypeToObject[sender.ModifiedType] == null)
            throw new System.Exception("Nothing to clear!");

        ObjectTypeToObject[sender.ModifiedType].OnBuildRemoved -= OnBuildRemoved;

        ObjectTypeToObject[sender.ModifiedType] = null;
        RefreshTile();

        sender.BuildInfo.OnRemove(sender);

        OnTileModified?.Invoke(this);
    }

    public FlooringGroup CreateFlooringGroup(Dictionary<Vector2Int, FlooringNormalPartOnTile> normalFloorings, HashSet<Vector2Int> supportFloorings, Vector2Int topRight,
        List<GameObject> supportObjects, FlooringVariantBase flooringVariant, FlooringRotation rotation)
    {
        FlooringGroup group = new FlooringGroup(normalFloorings, supportFloorings, position, topRight, supportObjects, flooringVariant, rotation);

        foreach (KeyValuePair<Vector2Int, FlooringNormalPartOnTile> pair in normalFloorings)
        {
            TileInformationManager.Instance.TryGetTileInformation(pair.Key, out TileInformation tileInfo);
            tileInfo.SetNormalFlooringGroup(group);
            tileInfo.layerNum++;
        }
        foreach (Vector2Int p in supportFloorings)
        {
            TileInformationManager.Instance.TryGetTileInformation(p, out TileInformation tileInfo);
            tileInfo.SetSupportFlooringGroup(group);
        }

        return group;
    }

    private void SetNormalFlooringGroup(FlooringGroup group)
    {
        if (group != null && NormalFlooringGroup != null)
            throw new System.Exception("Normal flooring already set!");

        NormalFlooringGroup = group;

        OnTileModified?.Invoke(this);

        group.OnFlooringRemoved += OnFlooringRemoved;
    }

    private void SetSupportFlooringGroup(FlooringGroup group)
    {
        if (group != null && SupportFlooringGroup != null)
            throw new System.Exception("Support flooring already set!");

        SupportFlooringGroup = group;

        OnTileModified?.Invoke(this);

        group.OnFlooringRemoved += OnFlooringRemoved;
    }

    public bool FlooringRemovable(out FlooringGroup group)
    {
        group = TopFlooringGroup;

        if (TopFlooringGroup == null)
            return false;

        foreach (KeyValuePair<Vector2Int, FlooringNormalPartOnTile> floor in TopFlooringGroup.NormalFloorings)
        {
            TileInformationManager.Instance.TryGetTileInformation(floor.Key, out TileInformation thisTile);
            if (thisTile.TopMostBuild != null)
                return false;
        }

        return true;
    }

    private void OnFlooringRemoved(FlooringGroup sender)
    {
        //Decrease layer if sender is dock && not support
        if (sender.FlooringVariant.GetType() == typeof(DockFlooringVariant) && !sender.SupportFloorings.Contains(position))
        {
            layerNum--;
        }

        //Remove normal flooring type
        if (sender.NormalFloorings.ContainsKey(position))
            NormalFlooringGroup = null;

        //Remove support flooring type
        if (sender.SupportFloorings.Contains(position))
            SupportFlooringGroup = null;

        sender.OnFlooringRemoved -= OnFlooringRemoved;

        OnTileModified?.Invoke(this);
    }

    private void RefreshTile()
    {
        RefreshTopMostObject();
        RefreshCollision();
    }

    private void RefreshCollision()
    {
        BuildCollision = ((ObjectTypeToObject[ObjectType.OnTop] != null && ObjectTypeToObject[ObjectType.OnTop].BuildInfo.Collision) ||
                (ObjectTypeToObject[ObjectType.Standard] != null && ObjectTypeToObject[ObjectType.Standard].BuildInfo.Collision) ||
                (ObjectTypeToObject[ObjectType.Ground] != null && ObjectTypeToObject[ObjectType.Ground].BuildInfo.Collision));
    }

    public void RemoveAllBuilds()
    {
        while (TopMostBuild != null)
        {
            TopMostBuild.Remove(false);
        }
    }

    public void ResetTerrainInformation()
    {
        waterBGTracker = new int[4];
        tileLocation = TileLocation.Unknown;
        layerNum = 0;
    }

    private void RefreshTopMostObject()
    {
        TopMostBuild = ObjectTypeToObject[ObjectType.OnTop] ?? ObjectTypeToObject[ObjectType.Standard] ?? ObjectTypeToObject[ObjectType.Ground] ?? null;
    }

    public void StepOn()
    {
        if (TopMostBuild != null && TopMostBuild.Functions != null)
            TopMostBuild.Functions.StepOn();

        TileSteppedOnPlayer?.Invoke();
    }

    public void StepOff()
    {
        if (TopMostBuild != null && TopMostBuild.Functions != null)
            TopMostBuild.Functions.StepOff();

        TileSteppedOffPlayer?.Invoke();
    }

    public void ClickInteract()
    {
        void DestroyUI()
        {
            shownRightClickListUI.OnSelected -= DestroyUIOnSelected;

            shownRightClickListUI.Destroy();
            shownRightClickListUI = null;
            UIManager.OnAllUIClosed -= DestroyUI;
            PlayerMovement.PlayerMoved -= DestroyUIOnMove;
        }

        void DestroyUIOnMove(Vector2 position, bool show, Vector2 directionVector)
        {
            DestroyUI();
        }

        void DestroyUIOnSelected(ListComponentUI component)
        {
            DestroyUI();
        }

        if (shownRightClickListUI != null)
        {
            DestroyUI();
        }

        bool buildRemovable = BuildRemovable(out BuildOnTile build);
        bool flooringRemovable = FlooringRemovable(out FlooringGroup flooringGroup);

        List<RightClickOptionUI> options = new List<RightClickOptionUI>();

        if (buildRemovable || flooringRemovable) {
            RemoveRightClickOptionUI removeOption = new RemoveRightClickOptionUI(buildRemovable ? (IRemovable)build : (IRemovable)flooringGroup, null);
            options.Add(removeOption);
        }

        //Don't show anything if nothing to show.
        if (options.Count == 0)
            return;

        shownRightClickListUI = new ComponentsListPanel<RightClickOptionUI>(ResourceManager.Instance.IndicatorsCanvas.transform, position + new Vector2Int(3,0), 100f, false);
        UIManager.OnAllUIClosed += DestroyUI;
        PlayerMovement.PlayerMoved += DestroyUIOnMove;
        shownRightClickListUI.OnSelected += DestroyUIOnSelected;

        foreach (RightClickOptionUI o in options)
            shownRightClickListUI.InsertListComponent(o);
    }

    public void InvokeTerrainModified()
    {
        OnTileModified?.Invoke(this);
    }

    public void SetStairs(Stairs stairs)
    {
        if (StairsOnTile != null)
            throw new System.Exception("Stairs already here");

        StairsOnTile = stairs;

        foreach (StairsStartPosition startPos in (new StairsStartPosition[] { stairs.startPositionAbove, stairs.startPositionBelow }))
        {
            TileInformationManager.Instance.TryGetTileInformation(startPos.startPosition, out TileInformation tileInfo);
            if (tileInfo != null)
                tileInfo.StairsStartPositions.Add(startPos);
        }
    }

    public void UnSetStairs()
    {
        if (StairsOnTile == null)
            throw new System.Exception("No stairs to remove");

        foreach (StairsStartPosition startPos in (new StairsStartPosition[] { StairsOnTile.startPositionAbove, StairsOnTile.startPositionBelow }))
        {
            TileInformationManager.Instance.TryGetTileInformation(startPos.startPosition, out TileInformation tileInfo);
            if (tileInfo != null)
                tileInfo.StairsStartPositions.Remove(startPos);
        }

        StairsOnTile = null;
    }

    public bool StairsStartPositionWithDirectionExists(Direction direction, out StairsStartPosition startPosition)
    {
        foreach (StairsStartPosition pos in StairsStartPositions)
        {
            if (pos.directionToStairs == direction.DirectionVector() && pos.startLayerNum == layerNum)
            {
                startPosition = pos;
                return true;
            }
        }
        startPosition = null;
        return false;
    }

    public void SetRegion(RegionInstance instance)
    {
        if (Region != null)
            throw new System.Exception("Region already set!");

        Region = instance;

        OnTileModified?.Invoke(this);

        Region.OnRegionRemoved += OnRegionRemoved;
    }

    public void OnRegionRemoved()
    {
        Region.OnRegionRemoved -= OnRegionRemoved;

        Region = null;
        OnTileModified?.Invoke(this);
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