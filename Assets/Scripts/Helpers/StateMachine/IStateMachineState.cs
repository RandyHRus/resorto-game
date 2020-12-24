using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IStateMachineState
{
    event ChangeState OnChangeState;
    event EndState OnEndState;

    void StartState(object[] args);

    void Execute();

    void LateExecute();

    void EndState();
}

public delegate void ChangeState(Type stateType, object[] args);
public delegate void EndState();
