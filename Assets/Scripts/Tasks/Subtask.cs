using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Subtask
{
    [SerializeField] private string description = "";
    public string Description => description;

    public abstract void Initialize();

    protected void SignalSubtaskCompleted()
    {
        OnSubtaskCompleted?.Invoke();
    }

    public delegate void SubtaskCompleted();
    public event SubtaskCompleted OnSubtaskCompleted;
}
