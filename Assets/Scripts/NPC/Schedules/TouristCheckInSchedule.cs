using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TouristCheckInSchedule : NPCSchedule
{
    public TouristComponents touristComponents => (TouristComponents)npcComponents;
    public override bool AllowTransitionToGoingToSleep => false;

    public TouristCheckInSchedule(TouristComponents touristComponents) : base(touristComponents) { }

    public override void TryStartScheduleAction()
    {
        HotelRoomRegionInstance assignedRoom = touristComponents.AssignedRoom;

        if (assignedRoom == null)
            return;

        Vector2Int luggageDropOffLocation = assignedRoom.GetRandomPosition();

        Action callBack = OnReachedDropOffLocationHandler;
        npcComponents.InvokeEvent(NPCInstanceEvent.ChangeState, new object[] { typeof(NPCWalkToPositionState),
                    new object[] { luggageDropOffLocation, callBack, "Dropping off luggage" } });

        //TODO drop off luggage at front desk if there is one
    }

    public void OnReachedDropOffLocationHandler()
    {
        npcComponents.InvokeEvent(NPCInstanceEvent.DropLuggage, null);
        InvokeEndState();
    }

    public override void EndState()
    {

    }
}
