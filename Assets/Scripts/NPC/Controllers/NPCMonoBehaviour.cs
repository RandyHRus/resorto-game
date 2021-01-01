using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class NPCMonoBehaviour : MonoBehaviour
{
    protected NPCStateMachine stateMachine;
    public NPCState CurrentState => stateMachine.CurrentState;

    public NPCScheduleManager ScheduleManager { get; private set; }
    public NPCSchedule CurrentSchedule => ScheduleManager.CurrentState;

    public Transform ObjectTransform { get; private set; }

    public NPCInformation NpcInformation { get; private set; }
    public NPCComponents NPCComponents { get; private set; }

    public delegate void StateChanged(NPCState previousState, NPCState newState);
    public event StateChanged OnStateChanged;

    public abstract NPCScheduleManager CreateNPCScheduleManager(NPCSchedule[] schedules, NPCComponents components);

    public abstract Type DefaultStateType { get; }

    private NPCClickOn clickOn;

    public virtual void Initialize(NPCInformation npcInfo, NPCComponents components)
    {
        this.NpcInformation = npcInfo;
        this.NPCComponents = components;
        ObjectTransform = transform;

        stateMachine = new NPCStateMachine(NPCComponents, DefaultStateType, GetNPCStates(NPCComponents));
        stateMachine.OnStateChanged += OnStateChangedHandler;
        stateMachine.SwitchState(DefaultStateType);

        ScheduleManager = CreateNPCScheduleManager(GetNPCSchedules(NPCComponents), NPCComponents);
        ScheduleManager.SwitchState(ScheduleManager.OnEndStateGetNextState(out object[] args), args);

        clickOn = gameObject.GetComponent<NPCClickOn>();
        clickOn.OnClick += FollowNPC;

        NPCComponents.SubscribeToEvent(NPCInstanceEvent.Delete, OnDeleteHandler);
        NPCComponents.SubscribeToEvent(NPCInstanceEvent.ChangeState, OnSignalChangeNPCStateDelegateHandler);
    }

    private void OnDestroy()
    {
        stateMachine.OnStateChanged -= OnStateChangedHandler;
        clickOn.OnClick -= FollowNPC;
    }

    public abstract Dialogue GetDialogue();

    public virtual void FollowNPC()
    {
        PlayerStateMachineManager.Instance.SwitchState<FollowNPCState>(new object[] { NPCComponents });
    }

    private void Update()
    {
        stateMachine.RunExecute();
    }

    private void LateUpdate()
    {
        stateMachine.RunLateExecute();
    }

    private void OnStateChangedHandler(NPCState previousState, NPCState newState)
    {
        OnStateChanged?.Invoke(previousState, newState);
    }

    private void OnSignalChangeNPCStateDelegateHandler(object[] args)
    {
        Type state = (Type)args[0];
        object[] stateArgs = (object[])args[1];
        stateMachine.SwitchState(state, stateArgs);
    }

    public void SwitchState<T>(object[] args = null) where T : NPCState => stateMachine.SwitchState<T>(args);

    public void SwitchState(Type type, object[] args = null) => stateMachine.SwitchState(type, args);

    public T GetStateInstance<T>() where T : NPCState => stateMachine.GetStateInstance<T>();

    //Override to add npc specific schedules (such as tourist specific or worker specific)
    public virtual NPCSchedule[] GetNPCSchedules(NPCComponents npcComponents)
    {
        return new NPCSchedule[]
        {
            new NPCLeavingSchedule(npcComponents),
            new NPCActivitiesSchedule(npcComponents),
            new NPCGoingToSleepSchedule(npcComponents),
            new NPCSleepSchedule(npcComponents)
        };
    }

    //Override to add npc specific states (such as tourist specific or worker specific)
    public virtual NPCState[] GetNPCStates(NPCComponents npcComponents)
    {
        return new NPCState[]
        {
            new NPCWalkToPositionState(npcComponents),
            new NPCFishingState(npcComponents),
            new NPCSleepingState(npcComponents),
        };
    }

    public virtual void OnDeleteHandler(object[] args)
    {
        Destroy(gameObject);
        stateMachine.OnStateChanged -= OnStateChangedHandler;

        NPCClickOn c = gameObject.GetComponent<NPCClickOn>();
        c.OnClick -= FollowNPC;

        NPCComponents.UnsubscribeToEvent(NPCInstanceEvent.Delete, OnDeleteHandler);
        NPCComponents.UnsubscribeToEvent(NPCInstanceEvent.ChangeState, OnSignalChangeNPCStateDelegateHandler);
    }
}

public class NPCStateMachine : StateMachineWithDefaultState<NPCState>
{
    public delegate void ActivityCompleted(Activity activity, float completenessFrac);
    public event ActivityCompleted OnActivityCompleted;

    private NPCComponents npcComponents;

    public NPCStateMachine(NPCComponents npcComponents, Type defaultStateType, NPCState[] stateInstances) : base(defaultStateType, stateInstances)
    {
        this.npcComponents = npcComponents;

        OnStateChanged += SubscribeEvents;
        npcComponents.SubscribeToEvent(NPCInstanceEvent.Delete, OnDelete);
    }

    private void SubscribeEvents(NPCState previousState, NPCState currentState)
    {
        if (previousState != null)
        {
            if (previousState is NPCActivityState previousActivityState)
                previousActivityState.OnActivityCompleted -= InvokeActivityCompleted;
        }

        if (currentState is NPCActivityState currentActivityState)
            currentActivityState.OnActivityCompleted += InvokeActivityCompleted;
    }

    private void InvokeActivityCompleted(Activity activity, float completenessFrac)
    {
        OnActivityCompleted?.Invoke(activity, completenessFrac);
    }

    private void OnDelete(object[] args)
    {
        OnStateChanged -= SubscribeEvents;
        npcComponents.UnsubscribeToEvent(NPCInstanceEvent.Delete, OnDelete);

        if (CurrentState is NPCActivityState currentActivityState)
            currentActivityState.OnActivityCompleted -= InvokeActivityCompleted;
    }
}
