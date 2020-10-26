using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Regions/Fishing")]
public class FishingRegionInformation : RegionInformation
{
    public override RegionInstance CreateInstance(HashSet<Vector2Int> positions)
    {
        return new FishingRegionInstance(this, positions);
    }
}
