using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

[CreateAssetMenu(menuName = "States/Player/Create Objects")]
public class CreateObjectsState : PlayerState
{
    [SerializeField] private Sprite defaultIndicatorSprite = null;
    private BuildRotation objectRotation;

    private ObjectInformation selectedObject;

    public override bool AllowMovement => false;
    public override bool AllowMouseDirectionChange => false;
    public override CameraMode CameraMode => CameraMode.Drag;

    public override void StartState(object[] args)
    {
        objectRotation = BuildRotation.Front;

        selectedObject = (ObjectInformation)args[0];
    }

    public override void EndState()
    {
        TilesIndicatorManager.Instance.ClearCurrentTiles();
    }

    public override void Execute()
    {
        if (Input.GetButtonDown("RotateObject") && selectedObject.HasSprite)
        {
            objectRotation += 1;
            if ((int)objectRotation == Enum.GetNames(typeof(BuildRotation)).Length)
                objectRotation = 0;

            int tryCount = 0;

            while (selectedObject.GetSpriteInformation(objectRotation).Sprite == null)
            {
                objectRotation += 1;
                if ((int)objectRotation == Enum.GetNames(typeof(BuildRotation)).Length) objectRotation = 0;
                tryCount++;
                if (tryCount >= 4)
                {
                    Debug.LogError("No sprite set for object!");
                    break;
                }
            }
            TilesIndicatorManager.Instance.ClearCurrentTiles();
        }

        Vector2Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();
        bool objectIsPlaceable = TileObjectsManager.Instance.ObjectPlaceable(mouseTilePosition, selectedObject, out ObjectType modifiedType, out float yOffset, objectRotation);

        //Indicator things
        {
            Sprite proposedSprite = (selectedObject.HasSprite) ? selectedObject.GetSpriteInformation(objectRotation).Sprite : defaultIndicatorSprite;

            if (TilesIndicatorManager.Instance.SwapCurrentTiles(mouseTilePosition))
            {
                TilesIndicatorManager.Instance.Offset(mouseTilePosition, new Vector2(0, yOffset));
                TilesIndicatorManager.Instance.SetSprite(mouseTilePosition, proposedSprite);
                TilesIndicatorManager.Instance.SetColor(mouseTilePosition, objectIsPlaceable ? ResourceManager.Instance.Green : ResourceManager.Instance.Red);
            }
        }

        //Create object
        if (objectIsPlaceable && CheckMouseOverUI.GetButtonDownAndNotOnUI("Primary"))
        {
            if (TileObjectsManager.Instance.TryCreateObject(selectedObject, mouseTilePosition, out BuildOnTile buildOnTile, objectRotation))
            {
                InventoryManager.Instance.SelectedSlot.RemoveItem(1);  //TODO make listener of an event (Event can be OnObjectCreated or something)
                TilesIndicatorManager.Instance.ClearCurrentTiles();
            }
        }
    }
}