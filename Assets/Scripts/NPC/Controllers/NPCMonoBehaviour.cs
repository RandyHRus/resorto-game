using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class NPCMonoBehaviour : MonoBehaviour
{
    [SerializeField] private NPCState defaultState = null;
    [SerializeField] private NPCState[] states = null;

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

        NPCState[] stateInstancesCopy = new NPCState[states.Length];
        for (int i = 0; i < states.Length; i++)
        {
            NPCState copy = Instantiate(states[i]);
            copy.Initialize(gameObject, NpcInstance);
            stateInstancesCopy[i] = copy;
        }

        stateMachine = new NPCStateMachine(defaultState.GetType(), stateInstancesCopy);
        stateMachine.OnStateChanged += InvokeStateChanged; //Remember to unsubscribe if I ever implement destroying     

        NPCClickOn c = gameObject.GetComponent<NPCClickOn>();
        c.OnClick += FollowNPC; //Remember to unsubscribe if I ever implement destroying     

        stateMachine.SwitchState(defaultState.GetType());

        ObjectTransform = transform;
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
    }

    public void SwitchState<T>(object[] args = null) where T : NPCState => stateMachine.SwitchState<T>(args);

    public void SwitchState(Type type, object[] args = null) => stateMachine.SwitchState(type, args);

    public T GetStateInstance<T>() where T : NPCState => stateMachine.GetStateInstance<T>();
}

public class NPCStateMachine : StateMachine<NPCState>
{
    public delegate void ActivityCompleted(Activity activity, float completenessFrac);
    public event ActivityCompleted OnActivityCompleted;

    public NPCStateMachine(Type defaultStateType, NPCState[] stateInstances) : base(defaultStateType, stateInstances)
    {
        OnStateChanged += SubscribeEvents;  //Remember to unsubscribe if I ever implement destroying     
    }

    private void SubscribeEvents(NPCState previousState, NPCState currentState)
    {
        if (previousState != null)
        {
            previousState.StartActivity -= TryStartActivity;

            if (previousState is NPCActivityState previousActivityState)
                previousActivityState.OnActivityCompleted -= InvokeActivityCompleted;
        }

        currentState.StartActivity += TryStartActivity;

        if (currentState is NPCActivityState currentActivityState)
            currentActivityState.OnActivityCompleted += InvokeActivityCompleted;
    }

    private void TryStartActivity(Activity activity)
    {
        if (activity.CanStartActivity(out Type switchToState, out object[] switchToStateArgs))
        {
            SwitchState(switchToState, switchToStateArgs);
        }
    }

    private void InvokeActivityCompleted(Activity activity, float completenessFrac)
    {
        OnActivityCompleted?.Invoke(activity, completenessFrac);
    }
}
