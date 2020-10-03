using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActivityState
{
    public abstract void StartState(object[] args);

    public abstract void Execute();

    public abstract void EndState();
}
