using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NPCActivitiesSchedule : NPCSchedule
{
    private TouristComponents touristComponents => (TouristComponents)npcComponents;

    public override bool AllowTransitionToGoingToSleep => true;

    public NPCActivitiesSchedule(NPCComponents npcComponents): base(npcComponents) { }

    private Type switchToState;
    private object[] switchToStateArgs;

    public override void TryStartScheduleAction()
    {
        TouristInterest randomInterest = touristComponents.interests[UnityEngine.Random.Range(0, touristComponents.interests.Length)];
        Activity randomActivity = randomInterest.Activies[UnityEngine.Random.Range(0, randomInterest.Activies.Length)];

        bool locationExists = randomActivity.GetActivityLocationAndStateToSwitchTo(out Vector2Int? location, out switchToState, out switchToStateArgs, out string goingToLocationMessage);

        if (!locationExists)
            return;

        Action callback = OnNPCAtActivityLocationHandler;
        npcComponents.InvokeEvent(NPCInstanceEvent.ChangeState, new object[] {
                            typeof(NPCWalkToPositionState),
                            new object[] { (Vector2Int)location, callback, goingToLocationMessage }});
    }

    private void OnNPCAtActivityLocationHandler()
    {
        npcComponents.InvokeEvent(NPCInstanceEvent.ChangeState, new object[] { switchToState, switchToStateArgs });
    }

    public override void EndState()
    {
        
    }
}
