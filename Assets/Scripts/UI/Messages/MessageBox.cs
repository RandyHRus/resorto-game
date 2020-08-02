using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MessageBox : UIObject
{
    private float boxShowTime = 3;
    private CanvasGroup group;
    public float alpha
    {
        get
        {
            return group.alpha;
        }
        set
        {
            group.alpha = value;
        }
    }
    public float timeRemaining;

    public MessageBox(GameObject prefab) : base(prefab, MessageManager.Instance.Canvas.transform)
    {
        group = ObjectInScene.GetComponent<CanvasGroup>();
        timeRemaining = boxShowTime;

        MessageManager.Instance.ShowMessage(this);
    }
}