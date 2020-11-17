using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Regions/Normal")]
public class NormalRegionInformation : RegionInformation
{
    public override RegionInstance CreateInstance(string instanceName, HashSet<Vector2Int> positions)
    {
        return new RegionInstance(instanceName, this, positions);
    }
}
