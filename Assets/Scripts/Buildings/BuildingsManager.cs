using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingsManager
{
    public static bool BuildingPlaceable(Vector2Int pos, BuildingStructureVariant variant, out HashSet<Vector2Int> tilesToOccupy)
    {
        tilesToOccupy = new HashSet<Vector2Int>();

        for (int i = 0; i < variant.GetSizeOnTile(0).x; i++)
        {
            for (int j = 0; j < variant.GetSizeOnTile(0).y; j++)
            {
                Vector2Int tilePosition = new Vector2Int(pos.x + i, pos.y + j);
                tilesToOccupy.Add(tilePosition);
            }
        }

        foreach (Vector2Int t in tilesToOccupy)
        {
            if (!TileInformationManager.Instance.TryGetTileInformation(t, out TileInformation tileInfo))
                return false;

            if (tileInfo.TopMostBuild != null)
                return false;

            if (!TileLocation.Land.HasFlag(tileInfo.tileLocation))
                return false;
        }

        return true;
    }

    public static bool TryCreateBuilding(Vector2Int pos, BuildingStructureVariant variant, IBuildingCustomization customization)
    {
        if (!BuildingPlaceable(pos, variant, out HashSet<Vector2Int> tilesToOccupy))
            return false;

        GameObject obj = GameObject.Instantiate(variant.Prefab, new Vector3(pos.x, pos.y, DynamicZDepth.GetDynamicZDepth(pos.y, DynamicZDepth.OBJECTS_STANDARD_OFFSET)), Quaternion.identity);

        TileInformationManager.Instance.TryGetTileInformation(pos, out TileInformation tileInfo);
        tileInfo.CreateBuild(obj, variant, tilesToOccupy, BuildRotation.Front, ObjectType.Ground);

        //Load customization here
        obj.GetComponent<IBuildingCustomizationLoader>().LoadCustomization(customization);

        return true;
    }
}
