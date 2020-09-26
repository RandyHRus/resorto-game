using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TasksManager : MonoBehaviour
{
    private static TasksManager _instance;
    public static TasksManager Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public delegate void EntryDelegate(TaskInstance task);
    public event EntryDelegate OnTaskAdded;
    public event EntryDelegate OnTaskCompleted;

    private HashSet<TaskInstance> activeTasks = new HashSet<TaskInstance>();
    private HashSet<TaskInstance> completedTasks = new HashSet<TaskInstance>();

    public void AddTask(TaskInstance task)
    {
        activeTasks.Add(task);
        OnTaskAdded?.Invoke(task);
    }

    public void CompleteTask(TaskInstance task)
    {
        activeTasks.Remove(task);
        completedTasks.Add(task);
        OnTaskCompleted?.Invoke(task);
    }
}
