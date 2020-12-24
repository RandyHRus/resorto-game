using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/HotelConnections")]
public class HotelConnectionsState : PlayerState
{
    [SerializeField] HotelsMapVisualizer hotelVisualizer = null;
    [SerializeField] HotelRoomRegionInformation hotelRoomInfo = null;
    [SerializeField] HotelFrontDeskRegionInformation hotelFrontDeskInfo = null;

    public override bool AllowMovement => false;
    public override bool AllowMouseDirectionChange => false;
    public override CameraMode CameraMode => CameraMode.Drag;

    private HotelFrontDeskRegionInstance frontDeskInstance = null;
    private Vector3 hotelFrontDeskMiddlePosition;

    [SerializeField] private GameObject dottedLine = null;
    private LineRenderer dottedLineInstance;

    private Color32 LINE_COLOR = new Color32(235, 209, 63, 255);

    private Vector2 previousMousePosition = new Vector3(-1, -1, -1000);
    private RegionInstance previousRegion = null;

    public override void Initialize()
    {
        base.Initialize();

        dottedLineInstance = GameObject.Instantiate(dottedLine).GetComponent<LineRenderer>();
        dottedLineInstance.gameObject.SetActive(false);
        dottedLineInstance.startColor = LINE_COLOR;
        dottedLineInstance.endColor = LINE_COLOR;
    }

    public override void StartState(object[] args)
    {
        frontDeskInstance = (HotelFrontDeskRegionInstance)args[0];
        hotelVisualizer.ShowVisualizer();
        dottedLineInstance.gameObject.SetActive(true);
        hotelFrontDeskMiddlePosition = frontDeskInstance.GetWeightedMiddlePos();
    }

    public override void Execute()
    {
        Vector2 mouseToWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();

        if (previousMousePosition != mouseToWorld)
        {
            dottedLineInstance.SetPositions(new Vector3[] { hotelFrontDeskMiddlePosition, mouseToWorld });
            previousMousePosition = mouseToWorld;
        }

        //Highlight on hover
        {
            if (!TileInformationManager.Instance.TryGetTileInformation(mouseTilePosition, out TileInformation mouseTileInfo))
            {
                if (previousRegion != null)
                {
                    previousRegion = null;
                    hotelVisualizer.ShowVisualizer(); //Clears highlight
                }
                return;
            }

            RegionInstance mouseRegionInstance = mouseTileInfo.Region;

            if (mouseRegionInstance != previousRegion)
            {
                previousRegion = mouseRegionInstance;
                hotelVisualizer.ShowVisualizer(); //Clears highlight

                if (mouseRegionInstance == null || (mouseRegionInstance.regionInformation != hotelRoomInfo && mouseRegionInstance.regionInformation != hotelFrontDeskInfo))
                    return;

                Color32 regionColor = hotelVisualizer.GetColor(mouseTilePosition);
                Color32 highlightColor = Color.Lerp(regionColor, Color.white, .5f);

                foreach (Vector2Int pos in mouseRegionInstance.GetRegionPositions())
                {
                    hotelVisualizer.OverrideColor(pos, highlightColor);
                }
            }
        }

        {
            if (CheckMouseOverUI.GetButtonDownAndNotOnUI("Primary") &&
                TileInformationManager.Instance.TryGetTileInformation(TileInformationManager.Instance.GetMouseTile(), out TileInformation mouseTileInfo))
            {
                //Creating connections
                if (mouseTileInfo.Region?.regionInformation == hotelRoomInfo)
                {
                    //Try to make connection to room
                    HotelRoomRegionInstance roomAtMouse = (HotelRoomRegionInstance)mouseTileInfo.Region;
                    if (HotelsManager.Instance.GetFrontDeskConnectedToRoom(roomAtMouse) == null)
                    {
                        HotelsManager.Instance.AddConnection(frontDeskInstance, roomAtMouse);

                        hotelVisualizer.ShowVisualizer(); //Need to refresh to show new connection
                    }
                }
                //Selecting different frontDesk
                else if (mouseTileInfo.Region?.regionInformation == hotelFrontDeskInfo)
                {
                    frontDeskInstance = (HotelFrontDeskRegionInstance)(mouseTileInfo.Region);

                    //Refresh line (Needs to be done here because the one above does not refresh every frame
                    //If the mouse isn't moved, it won't refresh
                    hotelFrontDeskMiddlePosition = frontDeskInstance.GetWeightedMiddlePos();
                    dottedLineInstance.SetPositions(new Vector3[] { hotelFrontDeskMiddlePosition, mouseToWorld });
                }
            }
            else if (CheckMouseOverUI.GetButtonDownAndNotOnUI("Secondary") &&
                TileInformationManager.Instance.TryGetTileInformation(TileInformationManager.Instance.GetMouseTile(), out mouseTileInfo)) {

                //Remove connection
                if (mouseTileInfo.Region?.regionInformation == hotelRoomInfo)
                {
                    HotelRoomRegionInstance roomAtMouse = (HotelRoomRegionInstance)mouseTileInfo.Region;

                    if (HotelsManager.Instance.GetFrontDeskConnectedToRoom(roomAtMouse) != null)
                    {
                        HotelsManager.Instance.RemoveConnection(roomAtMouse);

                        hotelVisualizer.ShowVisualizer(); //Need to refresh to hide connection
                    }
                }
            }
        }
    }

    public override void EndState()
    {
        hotelVisualizer.HideVisualizer();
        dottedLineInstance.gameObject.SetActive(false);
    }
}
