using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionManager
{
    public static bool RegionPlaceable(RegionInformation info, Vector3Int position)
    {
        return true;
    }

    public static bool RegionRemoveable(Vector3Int position)
    {
        return true;
    }

    public static bool TryCreateRegion(RegionInformation info, HashSet<Vector3Int> positions)
    {
        foreach (Vector3Int pos in positions) {
            if (!RegionPlaceable(info, pos))
            {
                return false;
            }
        }

        foreach (Vector3Int pos in positions)
        {
            TileInformationManager.Instance.GetTileInformation(pos).region = info;
        }

        return true;
    }
}
