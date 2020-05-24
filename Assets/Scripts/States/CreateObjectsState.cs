using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CreateObjectsState : MonoBehaviour, IPlayerState
{
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

        InventoryItem selectedItem = (InventoryItem)args[0];

        if (selectedItem == null)
            Debug.LogError("No item selected! This should not happen!");

        if (ObjectInformationManager.Instance.objectInformationMap.TryGetValue(selectedItem.id, out ObjectInformation info))
            selectedObject = info;
    }

    public bool TryEndState()
    {
        indicator.SetActive(false);
        return true;
    }

    public void Execute()
    {
        if (selectedObject == null)
        {
            Debug.Log("No object selected! This should not happen!");
            return;
        }

        Vector3Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();
        TileInformation mouseTile = TileInformationManager.Instance.GetTileInformation(mouseTilePosition);

        SpriteInformation proposedSprite = selectedObject.GetSpriteInformation(objectRotation); //TODO optimize so it doesn't have to fetch this every frame

        //So that you can lay ontop objects also in standard position
        ObjectType type = selectedObject.type;
        if (type == ObjectType.onTop)
        {
            if (mouseTile.GetObjectOnTile(ObjectType.standard) == null)
                type = ObjectType.standard;
        }

        //Check if object is placeable
        bool objectIsPlaceable = true;
        {
            if (mouseTile == null)
            {
                objectIsPlaceable = false;
                goto EarlyBreak;
            }
        }

        int mouseTileLayer = mouseTile.layerNum;

        {
            ObjectPlaceableLocation location = selectedObject.location;

            for (int i = 0; i < proposedSprite.xSize; i++)
            {
                for (int j = 0; j < proposedSprite.ySize; j++)
                {
                    Vector3Int checkTilePos = new Vector3Int(mouseTilePosition.x + i, mouseTilePosition.y + j, 0);

                    if (!TileInformationManager.Instance.PositionInMap(checkTilePos))
                    {
                        objectIsPlaceable = false;
                        goto EarlyBreak;
                    }

                    TileInformation checkTile = TileInformationManager.Instance.GetTileInformation(new Vector3Int(mouseTilePosition.x + i, mouseTilePosition.y + j, 0));
                    int layer = checkTile.layerNum;

                    if (layer != mouseTileLayer || layer == Constants.INVALID_TILE_LAYER)
                    {
                        objectIsPlaceable = false;
                        goto EarlyBreak;
                    }

                    //Check for valid terrain
                    switch (location)
                    {
                        case (ObjectPlaceableLocation.land):
                            if (layer == 0 && !TerrainManager.Instance.LandTileExists(0, mouseTilePosition))
                            {
                                if (mouseTile.GetObjectOnTile(ObjectType.ground) == null || !mouseTile.GetObjectOnTile(ObjectType.ground).objectsCanBePlacedOnTop)
                                {
                                    objectIsPlaceable = false;
                                    goto EarlyBreak;
                                }
                            }
                            break;
                        case (ObjectPlaceableLocation.water):
                            if (layer != 0 || TerrainManager.Instance.LandTileExists(0, mouseTilePosition))
                            {
                                objectIsPlaceable = false;
                                goto EarlyBreak;
                            }
                            break;
                        default:
                            Debug.Log("Unknown location type");
                            break;
                    }

                    //Check for valid objects
                    if (mouseTile.GetObjectOnTile(type) != null)
                    {
                        objectIsPlaceable = false;
                        goto EarlyBreak;
                    }
                }
            }
        }
        EarlyBreak:

        //Indicator things
        {
            //Indicator position
            indicator.transform.position = new Vector2(mouseTilePosition.x, mouseTilePosition.y);

            //Change indicator sprite
            if (indicatorRenderer.sprite != proposedSprite.sprite)
                indicatorRenderer.sprite = proposedSprite.sprite;
            //Change indicator color
            if (objectIsPlaceable)
                indicatorRenderer.color = ResourceManager.Instance.green;
            else
                indicatorRenderer.color = ResourceManager.Instance.red;
        }
        {
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
                CreateObject(selectedObject, objectRotation, mouseTilePosition, type);
            }
        }
    }

    public void CreateObject(ObjectInformation info, ObjectRotation rotation, Vector3Int pos, ObjectType type)
    {
        //Create gameObject
        GameObject obj;
        {
            if (info.prefab == null)
            {
                obj = new GameObject(info.name);
                obj.transform.position = new Vector2(pos.x, pos.y);
            }
            else
            {
                obj = Instantiate(info.prefab, new Vector2(pos.x, pos.y), Quaternion.identity);
            }
        }

        SpriteInformation sprInfo = info.GetSpriteInformation(rotation);

        //Set sprite
        {
            if (obj.TryGetComponent<SpriteRenderer>(out SpriteRenderer oldRenderer))
            {
                Debug.Log("Sprite renderer exists in prefab... please remove");
            }
            else
            {
                SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();

                //Set sorting layer
                switch (type)
                {
                    case (ObjectType.standard):
                        renderer.sortingLayerName = "DynamicY";
                        obj.transform.position = new Vector3(pos.x, pos.y, DynamicZDepth.GetDynamicZDepth(pos, DynamicZDepth.OBJECTS_STANDARD_OFFSET));
                        break;
                    case (ObjectType.ground):
                        renderer.sortingLayerName = "GroundObjects";
                        break;
                    case (ObjectType.onTop):
                        renderer.sortingLayerName = "DynamicY";
                        obj.transform.position = new Vector3(pos.x, pos.y, DynamicZDepth.GetDynamicZDepth(pos, DynamicZDepth.OBJECTS_ONTOP_OFFSET));
                        break;
                    default:
                        Debug.Log("Unknown type");
                        break;
                }
                renderer.sprite = sprInfo.sprite;
            }
        }
        //Set tiles
        {
            for (int i = 0; i < sprInfo.xSize; i++)
            {
                for (int j = 0; j < sprInfo.ySize; j++)
                {
                    TileInformation t = TileInformationManager.Instance.GetTileInformation(new Vector3Int(pos.x + i, pos.y + j, 0));

                    if (t.SetTileObject(obj, info.id, info.collision, rotation, type, info.objectsCanBePlacedOnTop) == false)
                    {
                        Destroy(obj);
                        Debug.Log("Tried to create object but failed. This should not happen! Check beforehand!");
                    }
                }
            }
        }
    }
}