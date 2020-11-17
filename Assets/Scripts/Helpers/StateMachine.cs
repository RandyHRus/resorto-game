using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StateMachine<T> where T: IStateMachineState
{
    public T CurrentState { get; private set; }
    protected Dictionary<Type, T> typeToStateInstance = new Dictionary<Type, T>();

    public delegate void StateChanged(T previousState, T newState);
    public event StateChanged OnStateChanged;

    public Type defaultStateType;

    public StateMachine(Type defaultStateType, T[] stateInstances)
    {
        foreach (T s in stateInstances)
        {
            Type stateType = s.GetType();
            typeToStateInstance.Add(stateType, s);
        }

        this.defaultStateType = defaultStateType;
    }

    public void RunExecute()
    {
        CurrentState.Execute();
    }

    public void RunLateExecute()
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

    public void SwitchDefaultState()
    {
        SwitchState(defaultStateType);
    }

    public void SwitchState(Type type, object[] args = null)
    {
        if (CurrentState != null)
        {
            CurrentState.OnChangeState -= SwitchState;
            CurrentState.OnEndState -= SwitchDefaultState;

            CurrentState.EndState();
        }

        T previousState = CurrentState;

        CurrentState = typeToStateInstance[type];

        CurrentState.OnChangeState += SwitchState;
        CurrentState.OnEndState += SwitchDefaultState;

        CurrentState.StartState(args);

        OnStateChanged?.Invoke(previousState, CurrentState);
    }
}
