using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/Create Buildings")]
public class CreateBuildingsState : PlayerState
{
    [SerializeField] private Sprite indicatorSprite = null;

    private BuildingStructureVariant buildingVariant;
    private IBuildingCustomization buildingCustomization = null;

    public override bool AllowMovement => false;
    public override bool AllowMouseDirectionChange => false;
    public override CameraMode CameraMode => CameraMode.Drag;

    public override void Execute()
    {
        Vector2Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();
        bool buildingPlaceable = BuildingsManager.Instance.BuildingPlaceable(mouseTilePosition, buildingVariant, out HashSet<Vector2Int> tilesToOccupy);

        //Indicator things TODO: CHANGE
        {
            TilesIndicatorManager.Instance.SwapCurrentTiles(tilesToOccupy);
            foreach (Vector2Int pos in tilesToOccupy)
            {
                TilesIndicatorManager.Instance.SetColor(pos, buildingPlaceable ? ResourceManager.Instance.Green : ResourceManager.Instance.Red);
                TilesIndicatorManager.Instance.SetSprite(pos, indicatorSprite);
            }
        }

        if (buildingPlaceable && CheckMouseOverUI.GetButtonDownAndNotOnUI("Primary"))
        {
            if (BuildingsManager.Instance.TryCreateBuilding(mouseTilePosition, buildingVariant, buildingCustomization))
            {
                //Todo: remove resources from inventory. etc
            }
        }

    }

    public override void StartState(object[] args)
    {
        buildingVariant = (BuildingStructureVariant)args[0];
        buildingCustomization = (IBuildingCustomization)args[1];
    }

    public override void EndState()
    {
        TilesIndicatorManager.Instance.ClearCurrentTiles();
    }
}
