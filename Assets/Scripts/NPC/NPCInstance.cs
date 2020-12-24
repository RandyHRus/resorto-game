using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class NPCInstance
{
    public readonly NPCInformation npcInformation;
    public readonly Transform npcTransform;
    public readonly CharacterVisualDirection npcDirection;
    public readonly float moveSpeed;

    public delegate void NPCDelete();
    public event NPCDelete OnNPCDelete;

    public delegate void ChangeNPCState(Type newState, object[] args);
    public ChangeNPCState ChangeNPCStateEvent;

    public delegate void NPCStateChanged(NPCState previousState, NPCState newState);
    public event NPCStateChanged OnNPCStateChanged;

    public delegate void TryStartScheduleAction();
    public event TryStartScheduleAction OnTryStartScheduleAction;

    public NPCInstance(NPCInformation info, Transform npcTransform)
    {
        this.npcInformation = info;
        this.npcTransform = npcTransform;
        npcDirection = new CharacterVisualDirection(npcTransform);
        moveSpeed = UnityEngine.Random.Range(0.8f, 1.2f);
    }

    public void InvokeOnDelete()
    {
        OnNPCDelete?.Invoke();
    }
    
    public void InvokeOnNPCStateChanged(NPCState previousState, NPCState newState)
    {
        OnNPCStateChanged?.Invoke(previousState, newState);
    }

    public void InvokeChangeNPCState<T>(object[] args = null) where T: NPCState
    {
        InvokeChangeNPCState(typeof(T), args);
    }

    public void InvokeChangeNPCState(Type type, object[] args = null)
    {
        ChangeNPCStateEvent?.Invoke(type, args);
    }

    public void InvokeTryStartScheduleAction()
    {
        OnTryStartScheduleAction?.Invoke();
    }
}
