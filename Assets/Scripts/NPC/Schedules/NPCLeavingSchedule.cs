using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NPCLeavingSchedule : NPCSchedule
{
    public override bool AllowTransitionToGoingToSleep => false;

    public NPCLeavingSchedule(NPCInstance npcInstance) : base(npcInstance) { }

    public override void TryStartScheduleAction()
    {
        Debug.Log("Going back to unloading dock");
        Vector2Int unloadingPosition = RegionManager.GetRandomRegionInstanceOfType(ResourceManager.Instance.BoatUnloadingRegion).GetRegionPositionsAsList()[0];

        Action callBack = OnNPCReachedUnloadingPositionHandler;
        npcInstance.InvokeChangeNPCState<NPCWalkToPositionState>(new object[] { unloadingPosition, callBack, "Leaving island" });
    }

    private void OnNPCReachedUnloadingPositionHandler()
    {
        npcInstance.InvokeOnDelete();
    }
    
    public override void EndState()
    {
        
    }
}