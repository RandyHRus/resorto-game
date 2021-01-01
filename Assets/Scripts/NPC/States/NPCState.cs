using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class NPCState : IStateMachineState
{
    protected GameObject npcGameObject;
    protected NPCComponents npcComponents;

    public event ChangeState OnChangeState;
    public event EndState OnEndState;

    public NPCState(NPCComponents npcComponents)
    {
        this.npcGameObject = npcComponents.npcTransform.gameObject;
        this.npcComponents = npcComponents;
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
