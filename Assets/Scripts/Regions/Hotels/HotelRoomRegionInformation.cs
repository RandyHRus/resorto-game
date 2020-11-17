using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Regions/HotelRoom")]
public class HotelRoomRegionInformation : RegionInformation
{
    public override RegionInstance CreateInstance(string instanceName, HashSet<Vector2Int> positions)
    {
        return new HotelRoomRegionInstance(instanceName, this, positions);
    }
}
