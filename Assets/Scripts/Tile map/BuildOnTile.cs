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
                TileInformation tileInfo = TileInformationManager.Instance.GetTileInformation(new Vector2Int(i, j));
                if (tileInfo == null)
                    continue;

                transparencySubscriptions.Add(tileInfo);

                tileInfo.TileSteppedOnPlayer += TileSteppedOnPlayerHandler;
                tileInfo.TileSteppedOffPlayer += TileSteppedOffPlayerHandler;
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
