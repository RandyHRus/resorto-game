using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/SelectRegion")]
public class SelectRegionState : PlayerState
{
    [SerializeField] RegionMapVisualizer regionVisualizer = null;

    public override bool AllowMovement => false;
    public override bool AllowMouseDirectionChange => false;
    public override CameraMode CameraMode => CameraMode.Drag;

    private RegionInstance previousRegion = null;

    public override void StartState(object[] args)
    {
        regionVisualizer.ShowVisualizer();
        previousRegion = null;
    }

    public override void Execute()
    {
        Vector2Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();

        if (!TileInformationManager.Instance.TryGetTileInformation(mouseTilePosition, out TileInformation mouseTileInfo))
        {
            if (previousRegion != null)
            {
                previousRegion = null;
                regionVisualizer.ShowVisualizer(); //Clears highlight
            }
            return;
        }

        RegionInstance mouseRegionInstance = mouseTileInfo.Region;

        if (mouseRegionInstance != previousRegion)
        {
            previousRegion = mouseRegionInstance;
            regionVisualizer.ShowVisualizer(); //Clears highlight

            if (mouseRegionInstance == null)
                return;

            Color32 regionColor = mouseRegionInstance.regionInformation.ShowColor;
            Color32 highlightColor = Color.Lerp(regionColor, Color.white, .5f);

            foreach (Vector2Int pos in mouseRegionInstance.GetRegionPositions())
            {
                regionVisualizer.OverrideColor(pos, highlightColor);
            }
        }

        if (mouseRegionInstance != null && (CheckMouseOverUI.GetButtonDownAndNotOnUI("Primary") || CheckMouseOverUI.GetButtonDownAndNotOnUI("Secondary")))
        {
            SidebarRegionsPanel sidebarTab = ((SidebarRegionsPanel)Sidebar.Instance.GetPanel(SidebarTab.Region));
            sidebarTab.ShowTab(1);
            sidebarTab.SelectRegionManageComponent(mouseRegionInstance);
        }

    }

    public override void EndState()
    {
        regionVisualizer.HideVisualizer();
    }
}
