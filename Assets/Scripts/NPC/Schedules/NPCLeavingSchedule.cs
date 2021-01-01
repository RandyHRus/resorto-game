using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NPCLeavingSchedule : NPCSchedule
{
    public override bool AllowTransitionToGoingToSleep => false;

    public NPCLeavingSchedule(NPCComponents npcComponents) : base(npcComponents) { }

    public override void TryStartScheduleAction()
    {
        Debug.Log("Going back to unloading dock");
        Vector2Int unloadingPosition = RegionManager.Instance.GetRandomRegionInstanceOfType(ResourceManager.Instance.BoatUnloadingRegion).GetRegionPositions()[0];

        Action callBack = OnNPCReachedUnloadingPositionHandler;
        npcComponents.InvokeEvent(NPCInstanceEvent.ChangeState, new object[] { typeof(NPCWalkToPositionState),
                                new object[] { unloadingPosition, callBack, "Leaving island" } });
    }

    private void OnNPCReachedUnloadingPositionHandler()
    {
        npcComponents.InvokeEvent(NPCInstanceEvent.Delete, null);
    }
    
    public override void EndState()
    {
        
    }
}