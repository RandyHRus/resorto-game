using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Create Buildings")]
public class CreateBuildingsState : PlayerState
{
    [SerializeField] private Sprite indicatorSprite = null;

    private BuildingStructureVariant buildingVariant;
    private IBuildingCustomization buildingCustomization = null;

    private TilesIndicatorManager indicatorManager;

    public override bool AllowMovement { get { return true; } }

    public override void Execute()
    {
        Vector3Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();
        bool buildingPlaceable = BuildingsManager.BuildingPlaceable(mouseTilePosition, buildingVariant, out HashSet<Vector3Int> tilesToOccupy);

        //Indicator things TODO: CHANGE
        {
            indicatorManager.SwapCurrentTiles(tilesToOccupy);
            foreach (Vector3Int pos in tilesToOccupy)
            {
                indicatorManager.SetColor(pos, buildingPlaceable ? ResourceManager.Instance.Green : ResourceManager.Instance.Red);
                indicatorManager.SetSprite(pos, indicatorSprite);
            }
        }

        if (buildingPlaceable && CheckMouseOverUI.GetButtonDownAndNotOnUI("Primary"))
        {
            if (BuildingsManager.TryCreateBuilding(mouseTilePosition, buildingVariant, buildingCustomization))
            {
                //Todo: remove resources from inventory. etc
            }
        }

    }

    public override void StartState(object[] args)
    {
        indicatorManager = new TilesIndicatorManager();
        buildingVariant = (BuildingStructureVariant)args[0];
        buildingCustomization = (IBuildingCustomization)args[1];
    }

    public override void EndState()
    {
        indicatorManager.ClearCurrentTiles();
    }
}
