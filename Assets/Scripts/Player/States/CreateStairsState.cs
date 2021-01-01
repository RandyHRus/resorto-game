using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/Create Stairs")]
public class CreateStairsState : PlayerState
{
    private StairsVariant stairsVariant;

    public override bool AllowMovement => false;
    public override bool AllowMouseDirectionChange => false;
    public override CameraMode CameraMode => CameraMode.Drag;

    public override void Execute()
    {
        Vector2Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();
        bool placeable = StairsManager.Instance.StairsPlaceable(mouseTilePosition, out BuildRotation rot);

        //Indicator things
        Sprite proposedSprite = stairsVariant.GetSprite(rot);
        if (TilesIndicatorManager.Instance.SwapCurrentTiles(mouseTilePosition))
        {
            TilesIndicatorManager.Instance.SetSprite(mouseTilePosition, proposedSprite);
            TilesIndicatorManager.Instance.SetColor(mouseTilePosition, placeable ? ResourceManager.Instance.Green : ResourceManager.Instance.Red);
        }

        //Create stairs
        if (placeable && CheckMouseOverUI.GetButtonDownAndNotOnUI("Primary"))
        {
            if (StairsManager.Instance.TryCreateStairs(stairsVariant, mouseTilePosition))
            {
                TilesIndicatorManager.Instance.ClearCurrentTiles(); //Resets tiles so that sprite color (etc) can be changed
            }
        }
    }

    public override void StartState(object[] args)
    {
        stairsVariant = (StairsVariant)args[0];
    }

    public override void EndState()
    {
        TilesIndicatorManager.Instance.ClearCurrentTiles();
    }
}
