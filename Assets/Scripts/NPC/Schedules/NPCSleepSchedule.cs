﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSleepSchedule : NPCSchedule
{
    private InGameTime wakeTime;

    public override bool AllowTransitionToGoingToSleep => false;

    public NPCSleepSchedule(NPCInstance npcInstance): base(npcInstance) { }

    public override void StartState(object[] args)
    {
        wakeTime = (InGameTime)args[0];

        //base.StartState(args);
        TryStartScheduleAction();
    }

    public override void TryStartScheduleAction()
    {
        npcInstance.InvokeChangeNPCState<NPCSleepingState>();

        if (TimeManager.Instance.GetCurrentTime() >= wakeTime)
        {
            InvokeEndState();
        }
        else
        {
            TimeManager.Instance.SubscribeToTime(wakeTime, OnWakeTimeHandler);
        }
    }

    private void OnWakeTimeHandler(object[] args)
    {
        TimeManager.Instance.UnsubscribeFromTime(wakeTime, OnWakeTimeHandler);
        InvokeEndState();
    }

    public override void EndState()
    {
        //Nothing needed
    }
}
