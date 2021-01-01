using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class NPCSchedule : IStateMachineState
{
    public event ChangeState OnChangeState;
    public event EndState OnEndState;

    public abstract bool AllowTransitionToGoingToSleep { get; }

    protected NPCComponents npcComponents;

    public NPCSchedule(NPCComponents npcComponents)
    {
        this.npcComponents = npcComponents;
    }

    public virtual void StartState(object[] args)
    {
        TryStartScheduleAction();
    }

    public void Execute()
    {
        //We are only using the stateMachine as a way to switch between states, not for running loops.
        throw new System.Exception("NPC Schedule does not support executing");
    }

    public void LateExecute()
    {
        throw new System.Exception("NPC Schedule does not support executing");
    }

    public abstract void EndState();

    public abstract void TryStartScheduleAction();

    protected void InvokeChangeSchedule(Type stateType, object[] args)
    {
        OnChangeState?.Invoke(stateType, args);
    }

    protected void InvokeEndState()
    {
        OnEndState?.Invoke();
    }
}
