using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObjectsManager : MonoBehaviour
{
    private static TileObjectsManager _instance;
    public static TileObjectsManager Instance { get { return _instance; } }

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
    }

    public bool ObjectPlaceable(Vector3Int mainPos, ObjectInformation info, out ObjectType modifiedType, ObjectRotation rotation = ObjectRotation.front)
    {
        ObjectType proposedType = info.Type;
        modifiedType = info.Type;

        if (!TileInformationManager.Instance.PositionInMap(mainPos))
            return false;

        TileInformation mainTile = TileInformationManager.Instance.GetTileInformation(mainPos);

        //So that you can lay ontop objects also in standard position
        if (proposedType == ObjectType.onTop)
        {
            if (mainTile.objectTypeToObject[ObjectType.standard] == null)
                proposedType = ObjectType.standard;
        }

        int mainTileLayer = mainTile.layerNum;
        ObjectPlaceableLocation objectLocation = info.Location;

        if (info.HasSprite)
        {
            SpriteInformation proposedSprite = info.GetSpriteInformation(rotation);

            for (int i = 0; i < proposedSprite.xSize; i++)
            {
                for (int j = 0; j < proposedSprite.ySize; j++)
                {
                    if (!ObjectPlaceableOnTile(new Vector3Int(mainPos.x + i, mainPos.y + j, 0)))
                        return false;
                }
            }
        }
        else
        {
            if (!ObjectPlaceableOnTile(mainPos))
                return false;
        }

        //Everything passed
        modifiedType = proposedType;
        return true;

        bool ObjectPlaceableOnTile(Vector3Int pos)
        {
            if (!TileInformationManager.Instance.PositionInMap(pos))
            {
                return false;
            }

            TileInformation checkTile = TileInformationManager.Instance.GetTileInformation(pos);
            int layer = checkTile.layerNum;

            if (layer != mainTileLayer || layer == Constants.INVALID_TILE_LAYER)
            {
                return false;
            }

            //Check for valid terrain
            switch (objectLocation)
            {
                case (ObjectPlaceableLocation.Land):
                    if (!TileLocationManager.isLand.HasFlag(checkTile.tileLocation))
                    {
                        //Can be still placed on water if there is object there
                        if (checkTile.objectTypeToObject[ObjectType.ground] == null || !checkTile.objectTypeToObject[ObjectType.ground].ObjectInfo.ObjectsCanBePlacedOnTop)
                            return false;
                    }
                    break;
                case (ObjectPlaceableLocation.Water):
                    if (!TileLocationManager.isWater.HasFlag(checkTile.tileLocation))
                        return false;
                    break;
                case (ObjectPlaceableLocation.GrassOnly):
                    if (checkTile.tileLocation != TileLocation.Grass)
                        return false;
                    break;
                case (ObjectPlaceableLocation.SandOnly):
                    if (checkTile.tileLocation != TileLocation.Sand)
                        return false;
                    break;
                default:
                    Debug.Log("Unknown location type");
                    break;
            }

            //Check for valid objects
            if (checkTile.objectTypeToObject[proposedType] != null)
                return false;

            //Check if objects can be placed on top
            if (proposedType == ObjectType.onTop && (checkTile.objectTypeToObject[ObjectType.standard] != null && !checkTile.objectTypeToObject[ObjectType.standard].ObjectInfo.ObjectsCanBePlacedOnTop))
                return false;
            else if (proposedType == ObjectType.standard && (checkTile.objectTypeToObject[ObjectType.ground] != null && !checkTile.objectTypeToObject[ObjectType.ground].ObjectInfo.ObjectsCanBePlacedOnTop))
                return false;

            return true;
        }
    }

    public void CreateObject(ObjectInformation info, Vector3Int mainPos, ObjectType modifiedType = ObjectType.None, ObjectRotation rotation = ObjectRotation.front)
    {
        //Get default type if no modified type
        if (modifiedType == ObjectType.None)
            modifiedType = info.Type;

        //Create gameObject
        GameObject obj;
        {
            obj = Instantiate(info.Prefab, new Vector2(mainPos.x, mainPos.y), Quaternion.identity);
            obj.name = info.ObjectName;
        }

        //Set sprite
        if (info.HasSprite)
        {
            SpriteInformation sprInfo = info.GetSpriteInformation(rotation);

            if (obj.TryGetComponent<SpriteRenderer>(out SpriteRenderer oldRenderer))
                Debug.LogError("Sprite renderer exists in prefab... please remove");

            SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();

            //Set sorting layer
            switch (modifiedType)
            {
                case (ObjectType.standard):
                    renderer.sortingLayerName = "DynamicY";
                    obj.transform.position = new Vector3(mainPos.x, mainPos.y, DynamicZDepth.GetDynamicZDepth(mainPos, DynamicZDepth.OBJECTS_STANDARD_OFFSET));
                    break;
                case (ObjectType.ground):
                    renderer.sortingLayerName = "GroundObjects";
                    break;
                case (ObjectType.onTop):
                    renderer.sortingLayerName = "DynamicY";
                    obj.transform.position = new Vector3(mainPos.x, mainPos.y, DynamicZDepth.GetDynamicZDepth(mainPos, DynamicZDepth.OBJECTS_ONTOP_OFFSET));
                    break;
                default:
                    Debug.LogError("Unknown type");
                    break;
            }
            renderer.sprite = sprInfo.sprite;
            renderer.material = ResourceManager.Instance.Diffuse;

            //Set tiles
            {
                List<Vector3Int> tilesToOccupy = new List<Vector3Int>();
                for (int i = 0; i < sprInfo.xSize; i++)
                {
                    for (int j = 0; j < sprInfo.ySize; j++)
                    {
                        Vector3Int pos = new Vector3Int(mainPos.x + i, mainPos.y + j, 0);
                        tilesToOccupy.Add(pos);
                    }
                }
                foreach (Vector3Int pos in tilesToOccupy)
                {
                    TileInformation t = TileInformationManager.Instance.GetTileInformation(pos);
                    t.SetTileObject(obj, info, modifiedType, tilesToOccupy, rotation);
                }
            }
        }
        else
        {
            TileInformation t = TileInformationManager.Instance.GetTileInformation(mainPos);
            t.SetTileObject(obj, info, modifiedType, new List<Vector3Int> { mainPos });
        }
    }

    public void RemoveObject(TileInformation info, ObjectType type)
    {
        ObjectOnTile objOnTile = info.objectTypeToObject[type];
        List<Vector3Int> positions = objOnTile.OccupiedTiles;

        //Actual destroy object part
        if (info.objectTypeToObject[type] != null)
            Destroy(info.objectTypeToObject[type].GameObjectOnTile);

        foreach (Vector3Int pos in positions)
        {
            TileInformationManager.Instance.GetTileInformation(pos).RemoveTileObject(type);
        }

        InventoryManager.Instance.AddItem(objOnTile.ObjectInfo.DropItem, 1);
    }
}
