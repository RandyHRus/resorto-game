using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouristCheckInSchedule : NPCSchedule
{
    public TouristCheckInSchedule(TouristInstance touristInstance) : base(touristInstance) { }

    public TouristInstance touristInstance => (TouristInstance)npcInstance;

    public override bool AllowTransitionToGoingToSleep => false;

    public override void EndState()
    {
        throw new System.NotImplementedException();
    }

    public override void TryStartScheduleAction()
    {
        HotelRoomRegionInstance assignedRoom = touristInstance.AssignedRoom;
    }
}
