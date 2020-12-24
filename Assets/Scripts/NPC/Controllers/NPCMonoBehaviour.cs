using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class NPCMonoBehaviour : MonoBehaviour
{
    protected NPCStateMachine stateMachine;
    public NPCState CurrentState => stateMachine.CurrentState;

    public NPCScheduleManager ScheduleManager { get; private set; }

    public NPCInformation NpcInformation { get; private set; }
    public Transform ObjectTransform { get; private set; }

    public NPCInstance NpcInstance { get; private set; }

    public delegate void StateChanged(NPCState previousState, NPCState newState);
    public event StateChanged OnStateChanged;

    public abstract NPCInstance CreateNPCInstance(NPCInformation npcInfo, Transform npcTransform);

    public abstract NPCScheduleManager CreateNPCScheduleManager(NPCSchedule[] schedules, NPCInstance instance);

    public abstract Type DefaultStateType { get; }

    public virtual void Initialize(NPCInformation npcInfo)
    {
        this.NpcInformation = npcInfo;

        NpcInstance = CreateNPCInstance(npcInfo, transform);

        stateMachine = new NPCStateMachine(NpcInstance, DefaultStateType, CreateNPCStates(NpcInstance));
        stateMachine.OnStateChanged += OnStateChangedHandler;

        NpcInstance.OnNPCDelete += OnDelete;

        ScheduleManager = CreateNPCScheduleManager(CreateNPCSchedules(NpcInstance), NpcInstance);

        NPCClickOn c = gameObject.GetComponent<NPCClickOn>();
        c.OnClick += FollowNPC; 

        ObjectTransform = transform;

        ScheduleManager.SwitchState(ScheduleManager.OnEndStateGetNextState(out object[] args), args);
        stateMachine.SwitchState(DefaultStateType);

        NpcInstance.ChangeNPCStateEvent += OnSignalChangeNPCStateDelegateHandler;
    }

    public abstract Dialogue GetDialogue();

    public virtual void FollowNPC()
    {
        PlayerStateMachineManager.Instance.SwitchState<FollowNPCState>(new object[] { NpcInstance });
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
        NpcInstance.InvokeOnNPCStateChanged(previousState, newState);
    }

    private void OnSignalChangeNPCStateDelegateHandler(Type schedule, object[] args)
    {
        stateMachine.SwitchState(schedule, args);
    }

    public void SwitchState<T>(object[] args = null) where T : NPCState => stateMachine.SwitchState<T>(args);

    public void SwitchState(Type type, object[] args = null) => stateMachine.SwitchState(type, args);

    public T GetStateInstance<T>() where T : NPCState => stateMachine.GetStateInstance<T>();

    //Override to add npc specific schedules (such as tourist specific or worker specific)
    public virtual NPCSchedule[] CreateNPCSchedules(NPCInstance npcInstance)
    {
        return new NPCSchedule[]
        {
            new NPCLeavingSchedule(npcInstance),
            new NPCActivitiesSchedule(npcInstance),
            new NPCGoingToSleepSchedule(npcInstance),
            new NPCSleepSchedule(npcInstance)
        };
    }

    //Override to add npc specific states (such as tourist specific or worker specific)
    public virtual NPCState[] CreateNPCStates(NPCInstance npcInstance)
    {
        return new NPCState[]
        {
            new NPCWalkToPositionState(npcInstance),
            new NPCFishingState(npcInstance),
            new NPCSleepingState(npcInstance),
        };
    }

    public virtual void OnDelete()
    {
        Destroy(gameObject);
        stateMachine.OnStateChanged -= OnStateChangedHandler;

        NPCClickOn c = gameObject.GetComponent<NPCClickOn>();
        c.OnClick -= FollowNPC;

        NpcInstance.ChangeNPCStateEvent -= OnSignalChangeNPCStateDelegateHandler;
    }
}

public class NPCStateMachine : StateMachineWithDefaultState<NPCState>
{
    public delegate void ActivityCompleted(Activity activity, float completenessFrac);
    public event ActivityCompleted OnActivityCompleted;

    private NPCInstance npcInstance;

    public NPCStateMachine(NPCInstance npcInstance, Type defaultStateType, NPCState[] stateInstances) : base(defaultStateType, stateInstances)
    {
        this.npcInstance = npcInstance;

        OnStateChanged += SubscribeEvents;  
        npcInstance.OnNPCDelete += OnDelete;
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

    private void OnDelete()
    {
        OnStateChanged -= SubscribeEvents;
        npcInstance.OnNPCDelete -= OnDelete;

        if (CurrentState is NPCActivityState currentActivityState)
            currentActivityState.OnActivityCompleted -= InvokeActivityCompleted;
    }
}
