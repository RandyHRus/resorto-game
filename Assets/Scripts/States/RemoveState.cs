using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Remove")]
public class RemoveState : PlayerState
{
    private OutlineIndicatorManager indicatorManager;

    public override bool AllowMovement
    {
        get { return true; }
    }

    public override void StartState(object[] args)
    {
        indicatorManager = new OutlineIndicatorManager();
        indicatorManager.Toggle(true);
    }

    public override void Execute()
    {
        Vector3Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();

        bool buildRemovable = RemoveManager.BuildRemovable(mouseTilePosition, out BuildOnTile build);
        bool flooringRemovable = false;
        if (!buildRemovable)
        {
            flooringRemovable = FlooringManager.FlooringRemoveable(mouseTilePosition);
        }

        //Indicator things
        {
            TileInformation tileInfo = TileInformationManager.Instance.GetTileInformation(mouseTilePosition);
            if (buildRemovable) {
                Vector2Int sizeOnTile = build.BuildInfo.GetSizeOnTile(build.Rotation);
                Vector2Int bottomLeft = (Vector2Int)build.OccupiedTiles[0];
                Vector2Int topRight = new Vector2Int(bottomLeft.x + sizeOnTile.x - 1, bottomLeft.y + sizeOnTile.y - 1);
                indicatorManager.SetSizeAndPosition(bottomLeft, topRight);
            }
            else if (flooringRemovable)
            {
                FlooringGroup group = tileInfo.GetTopFlooringGroup();
                indicatorManager.SetSizeAndPosition(group.BottomLeft, group.TopRight);
            }
            else
            {
                indicatorManager.SetSizeAndPosition((Vector2Int)mouseTilePosition, (Vector2Int)mouseTilePosition);
            }

            indicatorManager.SetColor(buildRemovable || flooringRemovable ? ResourceManager.Instance.Green : ResourceManager.Instance.Red);
        }

        //Actual remove part
        if (CheckMouseOverUI.GetButtonDownAndNotOnUI("Primary")) {
            if (buildRemovable)
            {
                if (RemoveManager.TryRemoveBuild(mouseTilePosition, out IBuildable buildInfo))
                {
                    buildInfo.OnRemove(build);
                }
            }
            else if (flooringRemovable)
            {
                if (FlooringManager.TryRemoveFlooring(mouseTilePosition))
                {

                }
            }
        }
    }

    public override bool TryEndState()
    {
        indicatorManager.Toggle(false);
        return true;
    }
}
