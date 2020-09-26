using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskInstance
{
    public delegate void TaskCompleted();
    public event TaskCompleted OnTaskCompleted;

    public delegate void SubtaskCompleted(int subTaskIndex);
    public event SubtaskCompleted OnSubtaskCompleted;

    public delegate void SubtaskDelegate(Subtask subtask);
    public event SubtaskDelegate OnNewSubtask;

    public Task TaskInformation { get; }
    private int currentSubtaskIndex = -1;
    public Subtask CurrentSubtask => TaskInformation.SubTasks[currentSubtaskIndex];

    public TaskInstance(Task taskInformation)
    {
        this.TaskInformation = taskInformation;
        NextSubTask();
    }

    public void NextSubTask()
    {
        if (currentSubtaskIndex != -1)
        {
            TaskInformation.SubTasks[currentSubtaskIndex].OnSubtaskCompleted -= NextSubTask;
            OnSubtaskCompleted?.Invoke(currentSubtaskIndex);
        }

        currentSubtaskIndex++;

        if (currentSubtaskIndex <= TaskInformation.SubTasks.Count)
        {
            TaskInformation.SubTasks[currentSubtaskIndex].OnSubtaskCompleted += NextSubTask;
            CurrentSubtask.Initialize();
            OnNewSubtask?.Invoke(CurrentSubtask);
        }
        else
        {
            OnTaskCompleted?.Invoke();
        }
    }
}
