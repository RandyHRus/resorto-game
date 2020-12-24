using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class StateMachine<T> where T: IStateMachineState
{
    public T CurrentState { get; private set; }
    protected Dictionary<Type, T> typeToStateInstance = new Dictionary<Type, T>();

    public delegate void StateChanged(T previousState, T newState);
    public event StateChanged OnStateChanged;

    public StateMachine(T[] stateInstances)
    {
        foreach (T s in stateInstances)
        {
            Type stateType = s.GetType();
            typeToStateInstance.Add(stateType, s);
        }
    }

    public virtual void RunExecute()
    {
        CurrentState.Execute();
    }

    public virtual void RunLateExecute()
    {
        CurrentState.LateExecute();
    }

    public T GetStateInstance(Type type)
    {
        return typeToStateInstance[type];
    }

    public T2 GetStateInstance<T2>() where T2: T
    {
        return (T2)GetStateInstance(typeof(T2));
    }

    public void SwitchState<T2>(object[] args = null) where T2: T
    {
        SwitchState(typeof(T2), args);
    }

    private void OnEndStateHandler()
    {
        SwitchState(OnEndStateGetNextState(out object[] args), args);
    }

    public abstract Type OnEndStateGetNextState(out object[] args);

    public void SwitchState(Type type, object[] args = null)
    {
        if (CurrentState != null)
        {
            //Im pretty sure I don't need to unsub from these
            //When say when a tourist leaves because the 
            //stateMachine will be deleted first before the states
            CurrentState.OnChangeState -= SwitchState;
            CurrentState.OnEndState -= OnEndStateHandler;

            CurrentState.EndState();
        }

        T previousState = CurrentState;

        CurrentState = typeToStateInstance[type];

        CurrentState.OnChangeState += SwitchState;
        CurrentState.OnEndState += OnEndStateHandler;

        CurrentState.StartState(args);

        OnStateChanged?.Invoke(previousState, CurrentState);
    }

    public void EndCurrentState()
    {
        OnEndStateHandler();
    }
}
