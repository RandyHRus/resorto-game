using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NPCActivitiesSchedule : NPCSchedule
{
    private TouristInstance touristInstance => (TouristInstance)npcInstance;

    public override bool AllowTransitionToGoingToSleep => true;

    public NPCActivitiesSchedule(NPCInstance npcInstance): base(npcInstance) { }

    private Type switchToState;
    private object[] switchToStateArgs;

    public override void TryStartScheduleAction()
    {
        TouristInterest randomInterest = touristInstance.interests[UnityEngine.Random.Range(0, touristInstance.interests.Length)];
        Activity randomActivity = randomInterest.Activies[UnityEngine.Random.Range(0, randomInterest.Activies.Length)];

        bool locationExists = randomActivity.GetActivityLocationAndStateToSwitchTo(out Vector2Int? location, out switchToState, out switchToStateArgs, out string goingToLocationMessage);

        if (!locationExists)
            return;

        Action callback = OnNPCAtActivityLocationHandler;
        npcInstance.InvokeChangeNPCState<NPCWalkToPositionState>(new object[] { location, callback, goingToLocationMessage });
    }

    private void OnNPCAtActivityLocationHandler()
    {
        npcInstance.InvokeChangeNPCState(switchToState, switchToStateArgs);
    }

    public override void EndState()
    {
        
    }
}
