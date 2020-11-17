using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class NPCMonoBehaviour : MonoBehaviour
{
    [SerializeField] private NPCState defaultState = null;
    [SerializeField] private NPCState[] states = null;

    public NPCSchedule Schedule { get; private set; }

    public NPCStateMachine stateMachine;
    public NPCState CurrentState => stateMachine.CurrentState;

    public NPCInformation NpcInformation { get; private set; }
    public Transform ObjectTransform { get; private set; }

    public NPCInstance NpcInstance { get; private set; }

    public delegate void StateChanged(NPCState previousState, NPCState newState);
    public event StateChanged OnStateChanged;

    public abstract NPCInstance CreateNPCInstance(NPCInformation npcInfo, Transform npcTransform);

    public virtual void Initialize(NPCInformation npcInfo)
    {
        this.NpcInformation = npcInfo;

        NpcInstance = CreateNPCInstance(npcInfo, transform);
        Schedule = npcInfo.CreateSchedule(NpcInstance);

        NPCState[] stateInstancesCopy = new NPCState[states.Length];
        for (int i = 0; i < states.Length; i++)
        {
            NPCState copy = Instantiate(states[i]);
            copy.Initialize(gameObject, NpcInstance);
            stateInstancesCopy[i] = copy;
        }

        stateMachine = new NPCStateMachine(NpcInstance, Schedule, defaultState.GetType(), stateInstancesCopy);
        stateMachine.OnStateChanged += InvokeStateChanged; 

        NPCClickOn c = gameObject.GetComponent<NPCClickOn>();
        c.OnClick += FollowNPC; 

        stateMachine.SwitchState(defaultState.GetType());

        ObjectTransform = transform;

        stateMachine.GetStateInstance<NPCLeaveAndDeleteState>().OnNPCDeleting += OnDelete;
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

    private void InvokeStateChanged(NPCState previousState, NPCState newState)
    {
        OnStateChanged?.Invoke(previousState, newState);
        NpcInstance.InvokeOnNPCStateChanged(previousState, newState);
    }

    public void SwitchState<T>(object[] args = null) where T : NPCState => stateMachine.SwitchState<T>(args);

    public void SwitchState(Type type, object[] args = null) => stateMachine.SwitchState(type, args);

    public T GetStateInstance<T>() where T : NPCState => stateMachine.GetStateInstance<T>();

    public virtual void OnDelete()
    {
        Destroy(gameObject);
        stateMachine.OnStateChanged -= InvokeStateChanged;

        NPCClickOn c = gameObject.GetComponent<NPCClickOn>();
        c.OnClick -= FollowNPC;

        stateMachine.GetStateInstance<NPCLeaveAndDeleteState>().OnNPCDeleting -= OnDelete;

        NpcInstance.InvokeOnDelete();
    }
}

public class NPCStateMachine : StateMachine<NPCState>
{
    public delegate void ActivityCompleted(Activity activity, float completenessFrac);
    public event ActivityCompleted OnActivityCompleted;

    private NPCSchedule schedule;
    private NPCInstance npcInstance;

    public NPCStateMachine(NPCInstance npcInstance, NPCSchedule schedule, Type defaultStateType, NPCState[] stateInstances) : base(defaultStateType, stateInstances)
    {
        this.npcInstance = npcInstance;
        this.schedule = schedule;
        OnStateChanged += SubscribeEvents;  
        npcInstance.OnNPCDelete += OnDelete;
    }

    private void SubscribeEvents(NPCState previousState, NPCState currentState)
    {
        if (previousState != null)
        {
            previousState.StartActivity -= GoToActivityLocationAndStartActivity;

            if (previousState is NPCActivityState previousActivityState)
                previousActivityState.OnActivityCompleted -= InvokeActivityCompleted;
        }

        currentState.StartActivity += GoToActivityLocationAndStartActivity;

        if (currentState is NPCActivityState currentActivityState)
            currentActivityState.OnActivityCompleted += InvokeActivityCompleted;
    }

    private void GoToActivityLocationAndStartActivity(Activity activity)
    {
        bool canGoToLocation = activity.GetActivityLocationAndStateToSwitchTo(out Vector2Int? location, out Type switchToState, out object[] switchToStateArgs, out string goingToLocationMessage);

        if (!canGoToLocation)
            return;

        SwitchState<NPCWalkToPositionState>(new object[] { location, switchToState, switchToStateArgs, goingToLocationMessage });
    }

    private void InvokeActivityCompleted(Activity activity, float completenessFrac)
    {
        OnActivityCompleted?.Invoke(activity, completenessFrac);
    }

    private void OnDelete()
    {
        OnStateChanged -= SubscribeEvents;
        npcInstance.OnNPCDelete -= OnDelete;
    }
}
