using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouristIdleState : NPCIdleState
{
    private readonly float timeBetweenTryStartScheduleAction = 5f;
    private float nextTryStartScheduleActionTimer = 0;

    public TouristIdleState(NPCComponents npcComponents): base(npcComponents)
    {

    }

    public override void StartState(object[] args)
    {
        base.StartState(args);
        nextTryStartScheduleActionTimer = timeBetweenTryStartScheduleAction;
    }

    public override void Execute()
    {
        base.Execute();

        nextTryStartScheduleActionTimer -= Time.deltaTime;
        if (nextTryStartScheduleActionTimer <= 0)
        {
            nextTryStartScheduleActionTimer = timeBetweenTryStartScheduleAction;

            //If still in idle state, it means action wasn't successfully started so keep trying
            npcComponents.InvokeEvent(NPCInstanceEvent.TryStartScheduleAction, null);
            return;
        }
    }
}
