using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Regions/Fishing")]
public class FishingRegionInformation : RegionInformation
{
    public override RegionInstance CreateInstance(string instanceName, HashSet<Vector2Int> positions)
    {
        return new FishingRegionInstance(instanceName, this, positions);
    }
}
