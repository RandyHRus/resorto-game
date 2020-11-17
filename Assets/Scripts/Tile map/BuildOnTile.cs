using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildOnTile: IRemovable
{
    public ITileObjectFunctions Functions { get; private set; }
    public HashSet<Vector2Int> OccupiedTiles { get; private set; }
    public Vector2Int BottomLeft { get; private set; }
    public IBuildable BuildInfo { get; private set; }
    public GameObject GameObjectOnTile { get; private set; }
    public BuildRotation Rotation { get; private set; }
    public ObjectType ModifiedType { get; private set; }
    public readonly HashSet<Vector2Int> neighbours;

    private HashSet<TileInformation> transparencySubscriptions;
    private int currentSteppedOnTilesCount;

    public delegate void OnBuildRemove(BuildOnTile sender);
    public event OnBuildRemove OnBuildRemoved;

    public BuildOnTile(GameObject gameObjectOnTile, IBuildable buildInfo, HashSet<Vector2Int> tilesToOccupy, BuildRotation rotation, ObjectType modifiedType, Vector2Int bottomLeft)
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

        transparencySubscriptions = new HashSet<TileInformation>();
        //Subscripe to step events
        for (int i = transparencySubscribeBottomLeft.x; i < transparencySubscribeBottomLeft.x + sizeOnTile.x; i++)
        {
            for (int j = transparencySubscribeBottomLeft.y; j < transparencySubscribeBottomLeft.y + buildInfo.TransparencyCapableYSize; j++)
            {
                if (!TileInformationManager.Instance.TryGetTileInformation(new Vector2Int(i, j), out TileInformation tileInfo))
                    continue;

                transparencySubscriptions.Add(tileInfo);

                tileInfo.TileSteppedOnPlayer += TileSteppedOnPlayerHandler;
                tileInfo.TileSteppedOffPlayer += TileSteppedOffPlayerHandler;
            }
        }

        //Initialize neighbours
        {
            neighbours = new HashSet<Vector2Int>();
            foreach (Vector2Int buildPos in tilesToOccupy)
            {
                Vector2Int[] checkNeighbours = new Vector2Int[]
                {
                new Vector2Int(buildPos.x + 1,  buildPos.y),
                new Vector2Int(buildPos.x - 1,  buildPos.y),
                new Vector2Int(buildPos.x,      buildPos.y + 1),
                new Vector2Int(buildPos.x,      buildPos.y - 1)
                };

                foreach (Vector2Int n in checkNeighbours)
                {
                    if (TileInformationManager.Instance.TryGetTileInformation(n, out TileInformation tileInfo))
                    {
                        if (!tilesToOccupy.Contains(n) && !neighbours.Contains(n))
                            neighbours.Add(n);
                    }
                }
            }
        }
    }

    public void Remove(bool removedThroughPlayerInteraction)
    {
        //Actual destroy object part
        UnityEngine.Object.Destroy(GameObjectOnTile);

        //Unsubscribe here
        foreach (TileInformation t in transparencySubscriptions)
        {
            t.TileSteppedOnPlayer -= TileSteppedOnPlayerHandler;
            t.TileSteppedOffPlayer -= TileSteppedOffPlayerHandler;
        }

        if (removedThroughPlayerInteraction)
            BuildInfo.OnRemoveThroughPlayerInteraction(this);

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
