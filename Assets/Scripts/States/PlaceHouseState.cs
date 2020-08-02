using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Place House")]
public class PlaceHouseState : PlayerState
{
    [SerializeField] private ObjectInformation houseObject = null;
    [SerializeField] private Sprite indicatorSprite = null;

    private HouseInformation houseInfo;
    private TilesIndicatorManager indicatorManager;

    public override bool AllowMovement { get { return true; } }

    public override void Execute()
    {
        Vector3Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();
        bool objectIsPlaceable = TileObjectsManager.ObjectPlaceable(mouseTilePosition, houseObject, out ObjectType modifiedType, out float yOffset);

        HashSet<Vector3Int> positions = new HashSet<Vector3Int>();
        for (int i = mouseTilePosition.x; i < mouseTilePosition.x + houseObject.SizeWhenNoSprite.x; i++)
        {
            for (int j = mouseTilePosition.y; j < mouseTilePosition.y + houseObject.SizeWhenNoSprite.y; j++)
            {
                positions.Add(new Vector3Int(i, j, 0));
            }
        }

        //Indicator things TODO: CHANGE
        {
            indicatorManager.SwapCurrentTiles(positions);
            foreach (Vector3Int pos in positions)
            {
                indicatorManager.SetColor(pos, objectIsPlaceable ? ResourceManager.Instance.Green : ResourceManager.Instance.Red);
                indicatorManager.SetSprite(pos, indicatorSprite);
            }
        }

        if (objectIsPlaceable && CheckMouseOverUI.GetButtonDownAndNotOnUI("Primary"))
        {
            if (TileObjectsManager.TryCreateObject(houseObject, mouseTilePosition, out ObjectOnTile objectOnTile))
            {
                objectOnTile.GameObjectOnTile.GetComponent<HouseInformationLoader>().LoadInformation(houseInfo);
            }
        }

    }

    public override void StartState(object[] args)
    {
        indicatorManager = new TilesIndicatorManager();
        houseInfo = (HouseInformation)args[0];
    }

    public override bool TryEndState()
    {
        indicatorManager.HideCurrentTiles();
        return true;
    }
}
