using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class NPCState : ScriptableObject, IStateMachineState
{
    protected static readonly float moveSpeed = 1f;

    protected GameObject npcGameObject;
    protected NPCInstance npcInstance;

    public event ChangeState OnChangeState;
    public event EndState OnEndState;

    public delegate void StartActivityDelegate(Activity activity);
    public event StartActivityDelegate StartActivity;

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

    //DO NOT RUN IN STARTSTATE FUNCTION!!
    //(Because start state is run before OnStartActivity is correctly subscribed in NPCMonobehavior state manager)
    public void InvokeStartActivity(Activity activity)
    {
        StartActivity?.Invoke(activity);
    }

    public abstract void EndState();

    public abstract void Execute();

    public void Initialize(GameObject npcGameObject, NPCInstance npcInstance)
    {
        this.npcGameObject = npcGameObject;
        this.npcInstance = npcInstance;
        Initialize();
    }

    public virtual void Initialize()
    {

    }

    public virtual void LateExecute()
    {

    }

    public abstract void StartState(object[] args);

    //Displayed in (ex. tourist panel says "Idle" or something.
    public abstract string DisplayMessage { get; }
}
