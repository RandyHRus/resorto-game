using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObjectsManager
{
    public static bool ObjectPlaceable(Vector3Int mainPos, ObjectInformation info, out ObjectType modifiedType, out float yOffset, BuildRotation rotation = BuildRotation.Front)
    {
        modifiedType = 0;
        yOffset = 0;
        ObjectType proposedType = info.Type;

        TileInformation mainTile = TileInformationManager.Instance.GetTileInformation(mainPos);
        if (mainTile == null)
            return false;

        BuildGroupOnTile buildsOnTile = mainTile.BuildsOnTile;

        //So that you can lay ontop objects also in standard position
        if (proposedType == ObjectType.OnTop)
        {
            if (buildsOnTile.ObjectTypeToObject[ObjectType.Standard] == null)
                proposedType = ObjectType.Standard;
        }

        int mainTileLayer = mainTile.layerNum;
        ObjectPlaceableLocation objectLocation = info.Location;

        if (info.HasSprite)
        {
            ObjectSpriteInformation proposedSprite = info.GetSpriteInformation(rotation);

            for (int i = 0; i < proposedSprite.Size.x; i++)
            {
                for (int j = 0; j < proposedSprite.Size.y; j++)
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

        //Get yOffset
        modifiedType = proposedType;

        if (modifiedType == ObjectType.OnTop)
        {
            IBuildable belowBuildInfo = buildsOnTile.ObjectTypeToObject[ObjectType.Standard].BuildInfo;
            yOffset = belowBuildInfo.OnTopOffsetInPixels / 16f;
        }
        return true;

        bool ObjectPlaceableOnTile(Vector3Int pos)
        {
            TileInformation checkTile = TileInformationManager.Instance.GetTileInformation(pos);
            if (checkTile == null)
                return false;

            int layer = checkTile.layerNum;

            if (layer != mainTileLayer || layer == Constants.INVALID_TILE_LAYER)
                return false;

            //Check for valid terrain
            switch (objectLocation)
            {
                case (ObjectPlaceableLocation.Land):
                    if (!TileLocation.Land.HasFlag(checkTile.tileLocation))
                    {
                        //Can be still placed on water if there is dock
                        if (TileLocation.Water.HasFlag(checkTile.tileLocation))
                        {
                            if (checkTile.NormalFlooringGroup == null)
                                return false;
                        }
                        else
                        {
                            return false;
                        }
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
            if (checkTile.BuildsOnTile.ObjectTypeToObject[proposedType] != null)
                return false;

            //Check if objects can be placed on top
            if (proposedType == ObjectType.OnTop && (checkTile.BuildsOnTile.ObjectTypeToObject[ObjectType.Standard] != null && !checkTile.BuildsOnTile.ObjectTypeToObject[ObjectType.Standard].BuildInfo.ObjectsCanBePlacedOnTop))
                return false;
            else if (proposedType == ObjectType.Standard && (checkTile.BuildsOnTile.ObjectTypeToObject[ObjectType.Ground] != null && !checkTile.BuildsOnTile.ObjectTypeToObject[ObjectType.Ground].BuildInfo.ObjectsCanBePlacedOnTop))
                return false;

            return true;
        }
    }

    public static bool TryCreateObject(ObjectInformation info, Vector3Int mainPos, out BuildOnTile buildOnTile, BuildRotation rotation = BuildRotation.Front)
    {
        if (!ObjectPlaceable(mainPos, info, out ObjectType modifiedType, out float yOffset, rotation))
        {
            buildOnTile = null;
            return false;
        }

        TileInformation tileInfo = TileInformationManager.Instance.GetTileInformation(mainPos);

        //Create gameObject
        GameObject obj;
        {
            //Get position
            Vector3 proposedPos = new Vector3();
            switch (modifiedType)
            {
                case (ObjectType.Standard):
                    proposedPos = new Vector3(mainPos.x, mainPos.y + yOffset, DynamicZDepth.GetDynamicZDepth(mainPos, DynamicZDepth.OBJECTS_STANDARD_OFFSET));
                    break;
                case (ObjectType.Ground):
                    proposedPos = new Vector2(mainPos.x, mainPos.y + yOffset);
                    break;
                case (ObjectType.OnTop):
                    BuildOnTile objectBelow = tileInfo.BuildsOnTile.ObjectTypeToObject[ObjectType.Standard];
                    Vector3Int objectBelowMainPosition = objectBelow.OccupiedTiles[0];
                    proposedPos = new Vector3(mainPos.x, mainPos.y + yOffset, DynamicZDepth.GetDynamicZDepth(objectBelowMainPosition, DynamicZDepth.OBJECTS_ONTOP_OFFSET));
                    break;
                default:
                    Debug.LogError("Unknown type");
                    break;
            }

            if (info.Prefab == null)
            {
                obj = new GameObject("Object");
                obj.transform.position = proposedPos;
            }
            else
                obj = Object.Instantiate(info.Prefab, proposedPos, Quaternion.identity);

            obj.name = info.Name;
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
                case (ObjectType.Standard):
                    renderer.sortingLayerName = "DynamicY";
                    break;
                case (ObjectType.Ground):
                    renderer.sortingLayerName = "GroundObjects";
                    break;
                case (ObjectType.OnTop):
                    renderer.sortingLayerName = "DynamicY";
                    break;
                default:
                    Debug.LogError("Unknown type");
                    break;
            }
            renderer.sprite = sprInfo.Sprite;
            renderer.material = ResourceManager.Instance.Diffuse;

            //Get tiles to occupy when has sprite
            for (int i = 0; i < sprInfo.Size.x; i++)
            {
                for (int j = 0; j < sprInfo.Size.y; j++)
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
        buildOnTile = new BuildOnTile(obj, info, tilesToOccupy, rotation, modifiedType);

        foreach (Vector3Int checkPos in tilesToOccupy)
        {
            TileInformation t = TileInformationManager.Instance.GetTileInformation(checkPos);

            t.BuildsOnTile.SetTileObject(buildOnTile);
        }
        return true;
    }
}
