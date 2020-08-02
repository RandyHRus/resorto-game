using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Remove Objects")]
public class RemoveObjectsState : PlayerState
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

        bool objectRemovable = TileObjectsManager.ObjectRemovable(mouseTilePosition);
        bool flooringRemovable = false;
        if (!objectRemovable)
        {
            flooringRemovable = FlooringManager.FlooringRemoveable(mouseTilePosition);
        }

        //Indicator things
        {
            TileInformation tileInfo = TileInformationManager.Instance.GetTileInformation(mouseTilePosition);
            if (tileInfo != null)
            {
                if (objectRemovable) {
                    ObjectOnTile topMostObject = tileInfo.TopMostObject;
                    ObjectSpriteInformation sprInfo = topMostObject.ObjectInfo.GetSpriteInformation(topMostObject.Rotation);
                    Vector2Int bottomLeft = (Vector2Int)topMostObject.OccupiedTiles[0];
                    Vector2Int topRight = new Vector2Int(bottomLeft.x + sprInfo.XSize - 1, bottomLeft.y + sprInfo.YSize - 1);
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
            }

            indicatorManager.SetColor(objectRemovable || flooringRemovable ? ResourceManager.Instance.Green : ResourceManager.Instance.Red);
        }

        //Actual remove part
        if (CheckMouseOverUI.GetButtonDownAndNotOnUI("Primary")) {
            if (objectRemovable)
            {
                if (TileObjectsManager.TryRemoveObject(mouseTilePosition, out ObjectInformation objectInfo)) {
                    if (objectInfo.DropItem != null)
                        DropItems.DropItem(new Vector2(mouseTilePosition.x, mouseTilePosition.y), objectInfo.DropItem, 1, true);
                }
            }
            else if (flooringRemovable)
            {
                if (FlooringManager.TryRemoveFlooring(mouseTilePosition))
                {
                    //TODO: do something?? add money or something idk
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
