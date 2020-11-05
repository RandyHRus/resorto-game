using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionInstance
{
    public readonly RegionInformation regionInformation;
    protected HashSet<Vector2Int> regionPositions;

    public RegionInstance(RegionInformation regionInformation, HashSet<Vector2Int> positions)
    {
        this.regionInformation = regionInformation;
        this.regionPositions = positions;
    }

    public List<Vector2Int> GetRegionPositions()
    {
        return new List<Vector2Int>(regionPositions);
    }
}
