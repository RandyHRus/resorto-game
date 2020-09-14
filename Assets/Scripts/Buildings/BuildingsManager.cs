using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingsManager
{
    public static bool BuildingPlaceable(Vector3Int pos, BuildingStructureVariant variant, out HashSet<Vector3Int> tilesToOccupy)
    {
        tilesToOccupy = new HashSet<Vector3Int>();

        for (int i = 0; i < variant.GetSizeOnTile(0).x; i++)
        {
            for (int j = 0; j < variant.GetSizeOnTile(0).y; j++)
            {
                Vector3Int tilePosition = new Vector3Int(pos.x + i, pos.y + j, 0);
                tilesToOccupy.Add(tilePosition);
            }
        }

        foreach (Vector3Int t in tilesToOccupy)
        {
            TileInformation tileInfo = TileInformationManager.Instance.GetTileInformation(t);

            if (tileInfo == null)
                return false;

            if (tileInfo.TopMostBuild != null)
                return false;

            if (!TileLocation.Land.HasFlag(tileInfo.tileLocation))
                return false;
        }

        return true;
    }

    public static bool TryCreateBuilding(Vector3Int pos, BuildingStructureVariant variant, IBuildingCustomization customization)
    {
        if (!BuildingPlaceable(pos, variant, out HashSet<Vector3Int> tilesToOccupy))
            return false;

        GameObject obj = GameObject.Instantiate(variant.Prefab, new Vector3(pos.x, pos.y, DynamicZDepth.GetDynamicZDepth(pos.y, DynamicZDepth.OBJECTS_STANDARD_OFFSET)), Quaternion.identity);
        new BuildOnTile(obj, variant, tilesToOccupy, BuildRotation.Front, ObjectType.Ground, (Vector2Int)pos);

        //Load customization here
        obj.GetComponent<IBuildingCustomizationLoader>().LoadCustomization(customization);

        return true;
    }
}
