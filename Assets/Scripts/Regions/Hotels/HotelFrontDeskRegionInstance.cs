using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotelFrontDeskRegionInstance : RegionInstance
{
    public override string AdditionalButtonText => "Connect rooms";
    public override void OnAdditionalButtonClicked()
    {
        base.OnAdditionalButtonClicked();

        PlayerStateMachineManager.Instance.SwitchState<HotelConnectionsState>(new object[] { this });
    }

    public HotelFrontDeskRegionInstance(string instanceName, RegionInformation info, HashSet<Vector2Int> positions): base(instanceName, info, positions)
    {
        
    }
}
