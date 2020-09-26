using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Task : ScriptableObject
{
    [SerializeField] private string taskName = "";
    public string TaskName => taskName;

    [SerializeField] private string description = "";
    public string Description => description;

    public abstract List<Subtask> SubTasks { get; }
}
