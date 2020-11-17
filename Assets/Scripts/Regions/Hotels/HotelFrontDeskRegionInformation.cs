using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Regions/HotelFrontDesk")]
public class HotelFrontDeskRegionInformation : RegionInformation
{
    public override RegionInstance CreateInstance(string instanceName, HashSet<Vector2Int> positions)
    {
        return new HotelFrontDeskRegionInstance(instanceName, this, positions);
    }
}
