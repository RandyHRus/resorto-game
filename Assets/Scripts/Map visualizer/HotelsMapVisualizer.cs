using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MapVisualizer/Hotels")]
public class HotelsMapVisualizer : ColorMapVisualizer
{
    [SerializeField] private Color32 occupiedRoomColor = default;
    [SerializeField] private Color32 invalidRoomColor = default;
    [SerializeField] private Color32 unoccupiedValidRoomColor = default;
    [SerializeField] private Color32 frontDeskColor = default;
    [SerializeField] private HotelRoomRegionInformation hotelRoomRegionInfo = null;
    [SerializeField] private HotelFrontDeskRegionInformation hotelFrontDeskRegionInfo = null;
    [SerializeField] private GameObject dottedLine = null;

    private Queue<LineRenderer> renderersPool = new Queue<LineRenderer>();
    private Queue<LineRenderer> renderersActive = new Queue<LineRenderer>();

    public override void ShowVisualizer()
    {
        base.ShowVisualizer();

        ClearLines(); //Refresh lines

        ArrayHashSet<RegionInstance> frontDeskRegions = RegionManager.Instance.GetAllRegionInstancesOfType(hotelFrontDeskRegionInfo);
        if (frontDeskRegions != null)
        {
            foreach (RegionInstance f in frontDeskRegions.ToList())
            {
                HotelFrontDeskRegionInstance frontDeskRegion = (HotelFrontDeskRegionInstance)f;
                Vector3 frontDeskMiddlePos = frontDeskRegion.GetWeightedMiddlePos();
                HashSet<HotelRoomRegionInstance> connectedRooms = HotelsManager.Instance.GetRoomsConnectedToFrontDesk(frontDeskRegion);

                if (connectedRooms != null)
                {
                    foreach (HotelRoomRegionInstance connectedRoom in connectedRooms)
                    {
                        LineRenderer renderer;
                        if (renderersPool.Count > 0)
                        {
                            renderer = renderersPool.Dequeue();
                        }
                        else
                        {
                            renderer = GameObject.Instantiate(dottedLine).GetComponent<LineRenderer>();
                        }

                        renderer.gameObject.SetActive(true);
                        renderersActive.Enqueue(renderer);
                        renderer.SetPositions(new Vector3[] { frontDeskMiddlePos, connectedRoom.GetWeightedMiddlePos() });
                    }
                }
            }
        }
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
        else if (tileInfo.Region?.regionInformation == hotelFrontDeskRegionInfo)
        {
            return frontDeskColor;
        }
        else
        {
            return Color.white;
        }
    }

    public override void HideVisualizer()
    {
        base.HideVisualizer();
        ClearLines();
    }

    private void ClearLines()
    {
        foreach (LineRenderer renderer in renderersActive)
        {
            renderersPool.Enqueue(renderer);
            renderer.gameObject.SetActive(false);
        }

        renderersActive.Clear();
    }
}
