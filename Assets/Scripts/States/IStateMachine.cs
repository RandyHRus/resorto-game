using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStateMachine
{
    void TrySwitchState<T>(object[] args = null);

    void TrySwitchState(Type type, object[] args = null);
}
