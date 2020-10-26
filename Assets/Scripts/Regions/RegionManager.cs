using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionManager
{
    private static Dictionary<RegionInformation, ArrayHashSet<RegionInstance>> regionsInMap = new Dictionary<RegionInformation, ArrayHashSet<RegionInstance>>();

    public static bool RegionPlaceable(RegionInformation info, Vector2Int position)
    {
        return true;
    }

    public static bool RegionRemoveable(Vector2Int position)
    {
        return true;
    }

    public static bool TryCreateRegion(RegionInformation info, HashSet<Vector2Int> positions)
    {
        foreach (Vector2Int pos in positions) {
            if (!RegionPlaceable(info, pos))
            {
                return false;
            }
        }

        RegionInstance instance = info.CreateInstance(positions);

        //Add region to dictionary
        if (regionsInMap.TryGetValue(info, out ArrayHashSet<RegionInstance> set))
        {
            set.Add(instance);
        }
        else
        {
            ArrayHashSet<RegionInstance> newSet = new ArrayHashSet<RegionInstance>();
            newSet.Add(instance);
            regionsInMap.Add(info, newSet);
        }

        //Set regions on tilemap
        foreach (Vector2Int pos in positions)
        {
            TileInformationManager.Instance.GetTileInformation(pos).region = instance;
        }

        return true;
    }

    public static RegionInstance GetRandomRegionInstanceOfType(RegionInformation type)
    {
        if (regionsInMap.TryGetValue(type, out ArrayHashSet<RegionInstance> set))
        {
            return (set.Count > 0) ? set.GetRandom() : null;
        }
        else
        {
            return null;
        }
    }
}
