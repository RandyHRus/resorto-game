using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

[CreateAssetMenu(menuName = "States/Create Objects")]
public class CreateObjectsState : PlayerState
{
    [SerializeField] private Sprite defaultIndicatorSprite = null;
    private ObjectRotation objectRotation;

    private ObjectInformation selectedObject;
    private TilesIndicatorManager indicatorManager;

    public override bool AllowMovement
    {
        get { return true; }
    }


    //args[0] = InventoryItem
    public override void StartState(object[] args)
    {
        objectRotation = ObjectRotation.front;

        ObjectItemInformation selectedItem = (ObjectItemInformation)args[0];

        if (selectedItem == null)
            Debug.LogError("No item selected! This should not happen!");

        selectedObject = selectedItem.ObjectToPlace;

        indicatorManager = new TilesIndicatorManager();
    }

    public override bool TryEndState()
    {
        indicatorManager.HideCurrentTiles();
        return true;
    }

    public override void Execute()
    {
        if (Input.GetButtonDown("RotateObject") && selectedObject.HasSprite)
        {
            objectRotation += 1;
            if ((int)objectRotation == Enum.GetNames(typeof(ObjectRotation)).Length)
                objectRotation = 0;

            int tryCount = 0;

            while (selectedObject.GetSpriteInformation(objectRotation).Sprite == null)
            {
                objectRotation += 1;
                if ((int)objectRotation == Enum.GetNames(typeof(ObjectRotation)).Length) objectRotation = 0;
                tryCount++;
                if (tryCount >= 4)
                {
                    Debug.LogError("No sprite set for object!");
                    break;
                }
            }
        }

        Vector3Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();
        bool objectIsPlaceable = TileObjectsManager.ObjectPlaceable(mouseTilePosition, selectedObject, out ObjectType modifiedType, out float yOffset, objectRotation);

        //Indicator things
        {
            Sprite proposedSprite = (selectedObject.HasSprite) ? selectedObject.GetSpriteInformation(objectRotation).Sprite : defaultIndicatorSprite;

            if (indicatorManager.SwapCurrentTiles(mouseTilePosition))
            {
                indicatorManager.Offset(mouseTilePosition, new Vector2(0, yOffset));
                indicatorManager.SetSprite(mouseTilePosition, proposedSprite);
                indicatorManager.SetColor(mouseTilePosition, objectIsPlaceable ? ResourceManager.Instance.Green : ResourceManager.Instance.Red);
            }
        }

        //Create object
        if (objectIsPlaceable && CheckMouseOverUI.GetButtonDownAndNotOnUI("Primary"))
        {
            if (TileObjectsManager.TryCreateObject(selectedObject, mouseTilePosition, out ObjectOnTile objectOnTile, objectRotation))
            {
                InventoryManager.Instance.RemoveItem(InventoryManager.Instance.SelectedSlotIndex, 1);
            }
        }
    }
}