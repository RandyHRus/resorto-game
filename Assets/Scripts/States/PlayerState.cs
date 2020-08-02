using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState : ScriptableObject
{
    public virtual void Initialize()
    {

    }

    public abstract bool AllowMovement { get; }

    public abstract void StartState(object[] args);

    public abstract bool TryEndState();

    public abstract void Execute();
}
