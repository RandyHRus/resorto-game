using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class NPCState : IStateMachineState
{
    protected GameObject npcGameObject;
    protected NPCInstance npcInstance;

    public event ChangeState OnChangeState;
    public event EndState OnEndState;

    public NPCState(NPCInstance npcInstance)
    {
        this.npcGameObject = npcInstance.npcTransform.gameObject;
        this.npcInstance = npcInstance;
    }

    public void InvokeChangeState(Type stateType, object[] args = null)
    {
        if (stateType == null) {
            InvokeEndState();
            return;
        }

        OnChangeState?.Invoke(stateType, args);
    }

    public void InvokeEndState()
    {
        OnEndState?.Invoke();
    }

    public abstract void EndState();

    public abstract void Execute();

    public virtual void LateExecute()
    {

    }

    public abstract void StartState(object[] args);

    //Displayed in (ex. tourist panel says "Idle" or something.
    public abstract string DisplayMessage { get; }
}
