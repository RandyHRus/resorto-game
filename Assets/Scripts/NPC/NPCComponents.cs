using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class NPCComponents
{
    public readonly NPCInformation npcInformation;
    public readonly Transform npcTransform;
    public readonly CharacterVisualDirection npcDirection;
    public readonly float moveSpeed;

    private CustomEventManager<NPCInstanceEvent> eventManager = new CustomEventManager<NPCInstanceEvent>();

    public NPCComponents(NPCInformation info, Transform npcTransform)
    {
        this.npcInformation = info;
        this.npcTransform = npcTransform;
        npcDirection = new CharacterVisualDirection(npcTransform);
        moveSpeed = UnityEngine.Random.Range(0.8f, 1.2f);
    }

    public void SubscribeToEvent(NPCInstanceEvent _event, CustomEventGroup<NPCInstanceEvent>.Delegate eventHandler)
    {
        eventManager.Subscribe(_event, eventHandler);
    }

    public void UnsubscribeToEvent(NPCInstanceEvent _event, CustomEventGroup<NPCInstanceEvent>.Delegate eventHandler)
    {
        eventManager.Unsubscribe(_event, eventHandler);
    }

    public void InvokeEvent(NPCInstanceEvent _event, object[] args)
    {
        eventManager.TryInvokeEventGroup(_event, args);
    }
}

public enum NPCInstanceEvent
{
    Delete,
    ChangeState,
    TryStartScheduleAction,
    DropLuggage
}

