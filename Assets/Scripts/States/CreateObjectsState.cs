using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CreateObjectsState : MonoBehaviour, IPlayerState
{
    [SerializeField] private Sprite defaultIndicatorSprite = null;

    private GameObject indicator;
    private SpriteRenderer indicatorRenderer;
    private ObjectRotation objectRotation;

    private ObjectInformation selectedObject;

    private static CreateObjectsState _instance;
    public static CreateObjectsState Instance { get { return _instance; } }

    private void Awake()
    {
        //Singleton
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }
        {
            indicator = new GameObject("Indicator");
            indicatorRenderer = indicator.AddComponent<SpriteRenderer>();
            indicatorRenderer.sortingLayerName = "Indicator";
            indicator.SetActive(false);
        }
    }

    public bool AllowMovement
    {
        get { return true; }
    }


    //args[0] = InventoryItem
    public void StartState(object[] args)
    {
        indicator.SetActive(true);
        objectRotation = ObjectRotation.front;

        ObjectItemInformation selectedItem = (ObjectItemInformation)args[0];

        if (selectedItem == null)
            Debug.LogError("No item selected! This should not happen!");

        selectedObject = selectedItem.ObjectToPlace;
    }

    public bool TryEndState()
    {
        indicator.SetActive(false);
        return true;
    }

    public void Execute()
    {
        Vector3Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();
        bool objectIsPlaceable = TileObjectsManager.Instance.ObjectPlaceable(mouseTilePosition, selectedObject, out ObjectType modifiedType, objectRotation);

        //Indicator things
        {
            Sprite proposedSprite;
            if (selectedObject.HasSprite)
                proposedSprite = selectedObject.GetSpriteInformation(objectRotation).sprite;
            else
                proposedSprite = defaultIndicatorSprite;

            indicator.transform.position = new Vector2(mouseTilePosition.x, mouseTilePosition.y);

            //Change indicator sprite
            if (indicatorRenderer.sprite != proposedSprite)
                indicatorRenderer.sprite = proposedSprite;

            //Change indicator color
            if (objectIsPlaceable)
                indicatorRenderer.color = ResourceManager.Instance.Green;
            else
                indicatorRenderer.color = ResourceManager.Instance.Red;
        }

        if (Input.GetButtonDown("RotateObject"))
        {
            objectRotation += 1;
            if ((int)objectRotation == 4) objectRotation = 0;

            int tryCount = 0;

            while (selectedObject.GetSpriteInformation(objectRotation).sprite == null)
            {
                objectRotation += 1;
                if ((int)objectRotation == 4) objectRotation = 0;
                tryCount++;
                if (tryCount >= 4)
                {
                    Debug.Log("No sprite set for object!");
                    break;
                }
            }
        }
        else if (objectIsPlaceable && Input.GetButtonDown("Primary"))
        {
            TileObjectsManager.Instance.CreateObject(selectedObject, mouseTilePosition, modifiedType, objectRotation);
        }
    }
}