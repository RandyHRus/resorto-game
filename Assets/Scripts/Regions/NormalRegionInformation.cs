using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Regions/Normal")]
public class NormalRegionInformation : RegionInformation
{
    public override RegionInstance CreateInstance(HashSet<Vector2Int> positions)
    {
        return new RegionInstance(this, positions);
    }
}
