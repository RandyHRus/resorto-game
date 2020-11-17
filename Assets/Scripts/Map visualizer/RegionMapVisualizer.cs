using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MapVisualizer/Region")]
public class RegionMapVisualizer : ColorMapVisualizer
{
    bool currentlyShown = false;

    public override Color32 GetColor(Vector2Int position)
    {
        TileInformationManager.Instance.TryGetTileInformation(position, out TileInformation tileInfo);

        if (tileInfo.Region != null)
        {
            return tileInfo.Region.regionInformation.ShowColor;
        }
        else
        {
            return Color.white;
        }
    }

    public override void ShowVisualizer()
    {
        base.ShowVisualizer();

        if (!currentlyShown)
        {
            RegionManager.OnRegionCreated += OnRegionsModified;
            RegionManager.OnRegionRemoved += OnRegionsModified;
        }

        currentlyShown = true;
    }

    public override void HideVisualizer()
    {
        base.HideVisualizer();

        if (currentlyShown)
        {
            RegionManager.OnRegionCreated -= OnRegionsModified;
            RegionManager.OnRegionRemoved -= OnRegionsModified;
        }

        currentlyShown = false;
    }

    private void OnRegionsModified(RegionInstance region)
    {
        //Refreshes
        ShowVisualizer();
    }
}
