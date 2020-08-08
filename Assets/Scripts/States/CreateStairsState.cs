using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Create Stairs")]
public class CreateStairsState : CreateBuildState
{
    private TilesIndicatorManager indicatorManager;

    private StairsVariant stairsVariant;

    public override void Execute()
    {
        Vector3Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();
        bool placeable = StairsManager.StairsPlaceable(mouseTilePosition, out BuildRotation rot);

        //Indicator things
        Sprite proposedSprite = stairsVariant.GetSprite(rot);
        if (indicatorManager.SwapCurrentTiles(mouseTilePosition))
        {
            indicatorManager.SetSprite(mouseTilePosition, proposedSprite);
            indicatorManager.SetColor(mouseTilePosition, placeable ? ResourceManager.Instance.Green : ResourceManager.Instance.Red);
        }

        //Create stairs
        if (placeable && CheckMouseOverUI.GetButtonDownAndNotOnUI("Primary"))
        {
            if (StairsManager.TryCreateStairs(stairsVariant, mouseTilePosition))
            {
                indicatorManager.ClearCurrentTiles(); //Resets tiles so that sprite color (etc) can be changed
            }
        }
    }

    public override void StartState(object[] args)
    {
        stairsVariant = (StairsVariant)args[0];

        indicatorManager = new TilesIndicatorManager();
    }

    public override bool TryEndState()
    {
        indicatorManager.ClearCurrentTiles();
        return true;
    }
}
