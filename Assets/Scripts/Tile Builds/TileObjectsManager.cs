using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObjectsManager: MonoBehaviour
{
    private static TileObjectsManager _instance;
    public static TileObjectsManager Instance { get { return _instance; } }
    private void Awake()
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

    public bool ObjectPlaceable(Vector2Int mainPos, ObjectInformation info, out ObjectType modifiedType, out float yOffset, BuildRotation rotation = BuildRotation.Front)
    {
        modifiedType = 0;
        yOffset = 0;
        ObjectType proposedType = info.Type;

        if (!TileInformationManager.Instance.TryGetTileInformation(mainPos, out TileInformation mainTile))
            return false;

        //So that you can lay ontop objects also in standard position
        if (proposedType == ObjectType.OnTop)
        {
            if (mainTile.ObjectTypeToObject[ObjectType.Standard] == null)
                proposedType = ObjectType.Standard;
        }

        int mainTileLayer = mainTile.layerNum;

        if (info.HasSprite)
        {
            ObjectSpriteInformation proposedSprite = info.GetSpriteInformation(rotation);

            for (int i = 0; i < proposedSprite.Size.x; i++)
            {
                for (int j = 0; j < proposedSprite.Size.y; j++)
                {
                    if (!ObjectPlaceableOnTile(new Vector2Int(mainPos.x + i, mainPos.y + j)))
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
                    if (!ObjectPlaceableOnTile(new Vector2Int(mainPos.x + i, mainPos.y + j)))
                        return false;
                }
            }
        }

        //Everything passed

        //Get yOffset
        modifiedType = proposedType;

        if (modifiedType == ObjectType.OnTop)
        {
            IBuildable belowBuildInfo = mainTile.ObjectTypeToObject[ObjectType.Standard].BuildInfo;
            yOffset = belowBuildInfo.OnTopOffsetInPixels / 16f;
        }
        return true;

        bool ObjectPlaceableOnTile(Vector2Int pos)
        {
            if (!TileInformationManager.Instance.TryGetTileInformation(pos, out TileInformation checkTile))
                return false;

            int layer = checkTile.layerNum;

            if (layer != mainTileLayer || layer == Constants.INVALID_TILE_LAYER)
                return false;

            //Check for valid terrain
            {
                //Case when checkTile is water
                if (TileLocation.Water.HasFlag(checkTile.tileLocation))
                {
                    if (checkTile.NormalFlooringGroup != null)
                    {
                        if (!info.PlaceableLocations.HasFlag(TileLocation.Land))
                            return false;
                    }
                    else if (!info.PlaceableLocations.HasFlag(checkTile.tileLocation))
                    {
                        return false;
                    }
                }
                //Other tiles can be directly checked easily
                else if (!info.PlaceableLocations.HasFlag(checkTile.tileLocation))
                {
                    return false;
                }
            }

            //Check for valid objects
            if (checkTile.ObjectTypeToObject[proposedType] != null)
                return false;

            //Check if objects can be placed on top
            if (proposedType == ObjectType.OnTop && (checkTile.ObjectTypeToObject[ObjectType.Standard] != null && !checkTile.ObjectTypeToObject[ObjectType.Standard].BuildInfo.ObjectsCanBePlacedOnTop))
                return false;
            else if (proposedType == ObjectType.Standard && (checkTile.ObjectTypeToObject[ObjectType.Ground] != null && !checkTile.ObjectTypeToObject[ObjectType.Ground].BuildInfo.ObjectsCanBePlacedOnTop))
                return false;

            return true;
        }
    }

    public bool TryCreateObject(ObjectInformation info, Vector2Int mainPos, out BuildOnTile buildOnTile, BuildRotation rotation = BuildRotation.Front)
    {
        if (!ObjectPlaceable(mainPos, info, out ObjectType modifiedType, out float yOffset, rotation))
        {
            buildOnTile = null;
            return false;
        }

        TileInformationManager.Instance.TryGetTileInformation(mainPos, out TileInformation tileInfo);

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
                    BuildOnTile objectBelow = tileInfo.ObjectTypeToObject[ObjectType.Standard];
                    Vector2Int objectBelowMainPosition = (Vector2Int)objectBelow.BottomLeft;
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

        HashSet<Vector2Int> tilesToOccupy = new HashSet<Vector2Int>();

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
                    Vector2Int pos = new Vector2Int(mainPos.x + i, mainPos.y + j);
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
                    Vector2Int pos = new Vector2Int(mainPos.x + i, mainPos.y + j);
                    tilesToOccupy.Add(pos);
                }
            }
        }

        //Actual set tiles part
        buildOnTile = tileInfo.CreateBuild(obj, info, tilesToOccupy, rotation, modifiedType);

        return true;
    }
}
