using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NPCGoingToSleepSchedule : NPCSchedule
{
    public override bool AllowTransitionToGoingToSleep => false;

    public NPCGoingToSleepSchedule(NPCInstance npcInstance) : base(npcInstance) { }

    private InGameTime nextWakeTime;

    public override void StartState(object[] args)
    {
        //base.StartState(args);

        //First time TryStartScheduleAction runs, we want to try to go to the
        //Bed access location provided by argument because we want the npc to get to
        //The accessLocation at a almost exact specific time and we don't want to get
        //A different access point
        //
        //After that though, we want to calculate the bed access location again
        //Because it could have changed.
        Vector2Int? bedAccessLocation = (Vector2Int?)args[0];
        nextWakeTime = (InGameTime)args[1];
        TryStartScheduleAction(bedAccessLocation);
    }

    public override void TryStartScheduleAction()
    {
        //Calculate accessLocation again
        Vector2Int? bedAccessLocation = ((TouristInstance)npcInstance).GetRandomBedAccessLocation();
        TryStartScheduleAction(bedAccessLocation);
    }

    private void TryStartScheduleAction(Vector2Int? bedAccessLocation)
    {   
        //No way to get to bed
        if (bedAccessLocation == null)
        {
            Debug.Log("No way to get to bed");
            return;
        }

        Debug.Log("Going to bed");
        Action callBack = OnNPCReachedBedHandler;
        npcInstance.InvokeChangeNPCState<NPCWalkToPositionState>(new object[] { (Vector2Int)bedAccessLocation, callBack, "Going to bed" });
    }

    private void OnNPCReachedBedHandler()
    {
        InvokeChangeSchedule(typeof(NPCSleepSchedule), new object[] { nextWakeTime });
    }

    public override void EndState()
    {
        
    }
}
