using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StateMachineWithDefaultState<T> : StateMachine<T> where T: IStateMachineState
{
    public Type defaultStateType;

    public StateMachineWithDefaultState(Type defaultStateType, T[] stateInstances): base(stateInstances)
    {
        this.defaultStateType = defaultStateType;
    }

    public override Type OnEndStateGetNextState(out object[] args)
    {
        args = null;
        return defaultStateType;
    }
}
