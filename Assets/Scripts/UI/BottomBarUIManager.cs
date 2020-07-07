using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomBarUIManager : MonoBehaviour
{
    public void BreakModeButtonPressed()
    {
        PlayerStatesManager.Instance.TrySwitchState(RemoveObjectsState.Instance, null);
    }

    public void TerrainModeButtonClicked()
    {
        PlayerStatesManager.Instance.TrySwitchState(TerrainState.Instance, null);
    }

    public void RegionsModeButtonClicked()
    {
        PlayerStatesManager.Instance.TrySwitchState(RegionsManager.Instance, null);
    }
}
