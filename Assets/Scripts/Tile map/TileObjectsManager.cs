using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObjectsManager
{
    public static bool ObjectPlaceable(Vector3Int mainPos, ObjectInformation info, out ObjectType modifiedType, out float yOffset, ObjectRotation rotation = ObjectRotation.front)
    {
        ObjectType proposedType = info.Type;
        modifiedType = info.Type;

        yOffset = 0;

        TileInformation mainTile = TileInformationManager.Instance.GetTileInformation(mainPos);
        if (mainTile == null)
            return false;

        //So that you can lay ontop objects also in standard position
        if (proposedType == ObjectType.onTop)
        {
            if (mainTile.ObjectTypeToObject[ObjectType.standard] == null)
                proposedType = ObjectType.standard;
        }

        int mainTileLayer = mainTile.layerNum;
        ObjectPlaceableLocation objectLocation = info.Location;

        if (info.HasSprite)
        {
            ObjectSpriteInformation proposedSprite = info.GetSpriteInformation(rotation);

            for (int i = 0; i < proposedSprite.XSize; i++)
            {
                for (int j = 0; j < proposedSprite.YSize; j++)
                {
                    if (!ObjectPlaceableOnTile(new Vector3Int(mainPos.x + i, mainPos.y + j, 0)))
                        return false;
                }
            }
        }
        else
        {
            for (int i = 0; i < info.SizeWhenNoSprite.x; i++)
            {
                for (int j = 0; j < info.SizeWhenNoSprite.y; j++)
                {
                    if (!ObjectPlaceableOnTile(new Vector3Int(mainPos.x + i, mainPos.y + j, 0)))
                        return false;
                }
            }
        }

        //Everything passed
        modifiedType = proposedType;
        //Get yOffset
        if (modifiedType == ObjectType.onTop)
        {
            ObjectInformation belowObjectInfo = mainTile.ObjectTypeToObject[ObjectType.standard].ObjectInfo;
            yOffset = belowObjectInfo.OnTopOffsetInPixels / 16f;
        }
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
                    if (!TileLocation.Land.HasFlag(checkTile.tileLocation))
                    {
                        //Can be still placed on water if there is object there
                        if (checkTile.ObjectTypeToObject[ObjectType.ground] == null || !checkTile.ObjectTypeToObject[ObjectType.ground].ObjectInfo.ObjectsCanBePlacedOnTop)
                            return false;
                    }
                    break;
                case (ObjectPlaceableLocation.Water):
                    if (!TileLocation.Water.HasFlag(checkTile.tileLocation))
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
            if (checkTile.ObjectTypeToObject[proposedType] != null)
                return false;

            //Check if objects can be placed on top
            if (proposedType == ObjectType.onTop && (checkTile.ObjectTypeToObject[ObjectType.standard] != null && !checkTile.ObjectTypeToObject[ObjectType.standard].ObjectInfo.ObjectsCanBePlacedOnTop))
                return false;
            else if (proposedType == ObjectType.standard && (checkTile.ObjectTypeToObject[ObjectType.ground] != null && !checkTile.ObjectTypeToObject[ObjectType.ground].ObjectInfo.ObjectsCanBePlacedOnTop))
                return false;

            return true;
        }
    }

    public static bool TryCreateObject(ObjectInformation info, Vector3Int mainPos, out ObjectOnTile objectOnTile, ObjectRotation rotation = ObjectRotation.front)
    {
        if (!ObjectPlaceable(mainPos, info, out ObjectType modifiedType, out float yOffset, rotation))
        {
            objectOnTile = null;
            return false;
        }

        //Get default type if no modified type
        if (modifiedType == ObjectType.None)
            modifiedType = info.Type;

        //Create gameObject
        GameObject obj;
        {
            //Get position
            Vector3 proposedPos = new Vector3();
            switch (modifiedType)
            {
                case (ObjectType.standard):
                    proposedPos = new Vector3(mainPos.x, mainPos.y + yOffset, DynamicZDepth.GetDynamicZDepth(mainPos, DynamicZDepth.OBJECTS_STANDARD_OFFSET));
                    break;
                case (ObjectType.ground):
                    proposedPos = new Vector2(mainPos.x, mainPos.y + yOffset);
                    break;
                case (ObjectType.onTop):
                    ObjectOnTile objectBelow = TileInformationManager.Instance.GetTileInformation(mainPos).ObjectTypeToObject[ObjectType.standard];
                    Vector3Int objectBelowMainPosition = objectBelow.OccupiedTiles[0];
                    proposedPos = new Vector3(mainPos.x, mainPos.y + yOffset, DynamicZDepth.GetDynamicZDepth(objectBelowMainPosition, DynamicZDepth.OBJECTS_ONTOP_OFFSET));
                    break;
                default:
                    Debug.LogError("Unknown type");
                    break;
            }
            obj = UnityEngine.Object.Instantiate(info.Prefab, proposedPos, Quaternion.identity);
            obj.name = info.ObjectName;
        }

        List<Vector3Int> tilesToOccupy = new List<Vector3Int>();

        //Set sprite
        if (info.HasSprite)
        {
            ObjectSpriteInformation sprInfo = info.GetSpriteInformation(rotation);

            if (obj.TryGetComponent<SpriteRenderer>(out SpriteRenderer oldRenderer))
                Debug.LogError("Sprite renderer exists in prefab... please remove");

            SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();

            //Set sorting layer
            switch (modifiedType)
            {
                case (ObjectType.standard):
                    renderer.sortingLayerName = "DynamicY";
                    break;
                case (ObjectType.ground):
                    renderer.sortingLayerName = "GroundObjects";
                    break;
                case (ObjectType.onTop):
                    renderer.sortingLayerName = "DynamicY";
                    break;
                default:
                    Debug.LogError("Unknown type");
                    break;
            }
            renderer.sprite = sprInfo.Sprite;
            renderer.material = ResourceManager.Instance.Diffuse;

            //Get tiles to occupy when has sprite
            for (int i = 0; i < sprInfo.XSize; i++)
            {
                for (int j = 0; j < sprInfo.YSize; j++)
                {
                    Vector3Int pos = new Vector3Int(mainPos.x + i, mainPos.y + j, 0);
                    tilesToOccupy.Add(pos);
                }
            }
        }
        else
        {
            //Get tiles to occupy when no sprite
            for (int i = 0; i < info.SizeWhenNoSprite.x; i++)
            {
                for (int j = 0; j < info.SizeWhenNoSprite.y; j++)
                {
                    Vector3Int pos = new Vector3Int(mainPos.x + i, mainPos.y + j, 0);
                    tilesToOccupy.Add(pos);
                }
            }
        }

        //Actual set tiles part
        objectOnTile = new ObjectOnTile(obj, info, tilesToOccupy, rotation, modifiedType);

        foreach (Vector3Int checkPos in tilesToOccupy)
        {
            TileInformation t = TileInformationManager.Instance.GetTileInformation(checkPos);
            t.SetTileObject(modifiedType, objectOnTile);
        }
        return true;
    }

    public static bool ObjectRemovable(Vector3Int pos)
    {
        TileInformation tileInfo = TileInformationManager.Instance.GetTileInformation(pos);
        if (tileInfo == null)
            return false;

        ObjectOnTile topMostObject = tileInfo.TopMostObject;
        if (topMostObject == null)
            return false;

        //Check if there is any objects on above, if there is, it cannot be destroyed
        foreach (Vector3Int checkPos in topMostObject.OccupiedTiles)
        {
            if (TileInformationManager.Instance.GetTileInformation(checkPos).TopMostObject != topMostObject)
                return false;
        }

        return true;
    }

    public static bool TryRemoveObject(Vector3Int pos, out ObjectInformation removedObjectInfo)
    {
        if (!ObjectRemovable(pos))
        {
            removedObjectInfo = null;
            return false;
        }

        TileInformation tileInfo = TileInformationManager.Instance.GetTileInformation(pos);

        ObjectOnTile objOnTile = tileInfo.TopMostObject;
        removedObjectInfo = objOnTile.ObjectInfo;

        TileInformationManager.Instance.GetTileInformation(pos).RemoveTopMostTileObject();

        return true;
    }
}
