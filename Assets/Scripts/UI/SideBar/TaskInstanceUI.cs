using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskInstanceUI : ListComponentUI
{
    private int distanceBetweenSubtasks = 30;
    private Dictionary<Subtask, SubtaskInstanceUI> subtaskUIs = new Dictionary<Subtask, SubtaskInstanceUI>();

    public override int ObjectHeight => 56 + (subtaskUIs.Count * distanceBetweenSubtasks);

    public TaskInstanceUI(TaskInstance task, ComponentsListPanel<TaskInstanceUI> parent): base(ResourceManager.Instance.TaskInstanceUI, parent.ObjectTransform)
    {
        foreach (Transform t in ObjectTransform)
        {
            if (t.tag == "Name Field")
            {
                OutlinedText text = new OutlinedText(t.gameObject);
                text.SetText(task.TaskInformation.TaskName);
            }
            else if (t.tag == "Description Field")
            {
                OutlinedText text = new OutlinedText(t.gameObject);
                text.SetText(task.TaskInformation.Description);
            }
        }

        ShowSubtask(task.CurrentSubtask);
        task.OnNewSubtask += ShowSubtask;
    }

    public void ShowSubtask(Subtask subtask)
    {
        SubtaskInstanceUI ui = new SubtaskInstanceUI(subtask, this);
        subtaskUIs.Add(subtask, ui);
        RectTransform.sizeDelta += new Vector2(0, distanceBetweenSubtasks);
    }

    public void MarkSubtaskComplete(Subtask subtask)
    {
        if (subtaskUIs.TryGetValue(subtask, out SubtaskInstanceUI ui))
        {
            ui.MarkSubtaskComplete();
        }
        else
        {
            throw new System.Exception("Subtask not shown!");
        }
    }

    private class SubtaskInstanceUI: UIObject
    {
        private Image subtaskStatusImage;
        private Subtask subtask;

        public SubtaskInstanceUI(Subtask subTask, TaskInstanceUI taskInstanceUI): base(ResourceManager.Instance.SubtaskInstanceUI, taskInstanceUI.ObjectTransform)
        {
            this.subtask = subTask;

            foreach (Transform t in ObjectTransform)
            {
                if (t.tag == "Description Field 2")
                {
                    OutlinedText text = new OutlinedText(t.gameObject);
                    text.SetText(subTask.Description);
                }
                else if (t.tag == "Icon Field")
                {
                    subtaskStatusImage = t.gameObject.GetComponent<Image>();
                    subtaskStatusImage.sprite = ResourceManager.Instance.SubtaskActiveSprite;
                }
            }

            RectTransform.anchoredPosition = new Vector2(0, -(taskInstanceUI.subtaskUIs.Count * taskInstanceUI.distanceBetweenSubtasks) - 67);

            subTask.OnSubtaskCompleted += MarkSubtaskComplete;
        }

        public void MarkSubtaskComplete()
        {
            subtaskStatusImage.sprite = ResourceManager.Instance.SubtaskCompleteSprite;
        }

        public override void Destroy()
        {
            base.Destroy();
            subtask.OnSubtaskCompleted -= MarkSubtaskComplete;
        }
    }
}
