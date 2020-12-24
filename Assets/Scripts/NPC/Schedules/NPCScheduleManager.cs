using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class NPCScheduleManager: StateMachine<NPCSchedule>
{
    protected NPCInstance npcInstance;

    public NPCScheduleManager(NPCSchedule[] schedules, NPCInstance npcInstance) : base(schedules)
    {
        this.npcInstance = npcInstance;
    }

    public sealed override void RunExecute()
    {
        throw new System.Exception("Schedule manager does not have this method");
    }

    public sealed override void RunLateExecute()
    {
        throw new System.Exception("Schedule manager does not have this method");
    }
}