using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Create Stairs")]
public class CreateStairsState : PlayerState
{
    private TilesIndicatorManager indicatorManager;

    private StairsVariant stairsVariant;

    public override bool AllowMovement { get { return true; } }

    public override void Execute()
    {
        
    }

    public override void StartState(object[] args)
    {
        stairsVariant = (StairsVariant)args[0];

        indicatorManager = new TilesIndicatorManager();
    }

    public override bool TryEndState()
    {
        indicatorManager.HideCurrentTiles();
        return true;
    }
}
