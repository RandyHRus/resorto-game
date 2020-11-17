using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidebarTasksPanel : SidebarPanel
{
    [SerializeField] private Task testTask = null;

    private ComponentsListPanel<TaskInstanceUI> activeTasksPanel;
    private ComponentsListPanel<TaskInstanceUI> completedTasksPanel;
    private UIObject currentActiveTabPanel;

    private Dictionary<TaskInstance, TaskInstanceUI> activeTaskUIs = new Dictionary<TaskInstance, TaskInstanceUI>();

    private void Start()
    {
        foreach (Transform t in transform.GetComponentsInChildren<Transform>())
        {
            if (t.tag == "List Field")
            {
                activeTasksPanel = new ComponentsListPanel<TaskInstanceUI>(t.gameObject);
                t.gameObject.SetActive(false);
            }
            else if (t.tag == "List Field 2")
            {
                completedTasksPanel = new ComponentsListPanel<TaskInstanceUI>(t.gameObject);
                t.gameObject.SetActive(false);
            }
        }

        TasksManager.Instance.OnTaskAdded += ShowTaskActive;
        TasksManager.Instance.OnTaskCompleted += ShowTaskCompleted;

        for (int i = 0; i < 10; i++)
        {
            TasksManager.Instance.AddTask(new TaskInstance(testTask));
        }

        ShowTab(0);
    }

    public void ShowTab(int tabIndex)
    {
        currentActiveTabPanel?.ObjectInScene.SetActive(false);

        UIObject toActivate;

        switch (tabIndex)
        {
            case (0):
                toActivate = activeTasksPanel;
                break;
            case (1):
                toActivate = completedTasksPanel;
                break;
            default:
                throw new System.NotImplementedException();
        }

        toActivate.ObjectInScene.SetActive(true);
        currentActiveTabPanel = toActivate;
    }

    private void ShowTaskActive(TaskInstance task)
    {
        TaskInstanceUI newUI = new TaskInstanceUI(task, activeTasksPanel);
        activeTasksPanel.InsertListComponent(newUI);
    }

    private void ShowTaskCompleted(TaskInstance task)
    {
        if (activeTaskUIs.TryGetValue(task, out TaskInstanceUI ui))
        {
            activeTasksPanel.RemoveListComponent(ui);
            completedTasksPanel.InsertListComponent(ui);
            activeTaskUIs.Remove(task);
        }
        else
        {
            throw new System.Exception("Task not active!");
        }
    }
}
