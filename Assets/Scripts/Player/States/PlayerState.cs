using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState : ScriptableObject
{
    public virtual void Initialize()
    {

    }

    public abstract bool AllowMovement { get; }

    public abstract bool AllowMouseDirectionChange { get; }

    public abstract void StartState(object[] args);

    public abstract void EndState();

    public abstract void Execute();

    public virtual void LateExecute() { }
}
