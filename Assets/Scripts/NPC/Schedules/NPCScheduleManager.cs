using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class NPCScheduleManager: StateMachine<NPCSchedule>
{
    protected NPCComponents npcComponents;

    public NPCScheduleManager(NPCSchedule[] schedules, NPCComponents npcComponents) : base(schedules)
    {
        this.npcComponents = npcComponents;
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