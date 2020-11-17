using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MapVisualizer/Hotels")]
public class HotelsMapVisualizer : ColorMapVisualizer
{
    [SerializeField] Color32 occupiedRoomColor = default;
    [SerializeField] Color32 invalidRoomColor = default;
    [SerializeField] Color32 unoccupiedValidRoomColor = default;
    [SerializeField] HotelRoomRegionInformation hotelRoomRegionInfo = null;

    public override void ShowVisualizer()
    {
        base.ShowVisualizer();
        //TODO: Draw lines from hotel front desk to hotel rooms
    }

    public override Color32 GetColor(Vector2Int position)
    {
        TileInformationManager.Instance.TryGetTileInformation(position, out TileInformation tileInfo);
        
        if (tileInfo.Region?.regionInformation == hotelRoomRegionInfo)
        {
            if (((HotelRoomRegionInstance)tileInfo.Region).IsValid)
            {
                return ((HotelRoomRegionInstance)tileInfo.Region).Occupied ? occupiedRoomColor : unoccupiedValidRoomColor;
            }
            else
            {
                return invalidRoomColor;
            }
        }
        else
        {
            return Color.white;
        }
    }
}
