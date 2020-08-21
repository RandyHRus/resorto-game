using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MessageBox : UIObject
{
    private float boxShowTime = 3;
    private CanvasGroup group;

    private float overshootFrequency = 10f;
    private float overshootDecay = 0.03f;
    private float overshootHeight = 0.05f;

    private Coroutine overshootCoroutine;

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
        void OvershootMove(float value)
        {
            //MessageBox could have been destroyed
            if (RectTransform != null)
                RectTransform.localScale = new Vector2(1 + value, 1 + value);
        }

        group = ObjectInScene.GetComponent<CanvasGroup>();
        timeRemaining = boxShowTime;

        MessageManager.Instance.ShowMessage(this);

        overshootCoroutine = Coroutines.Instance.StartCoroutine(OvershootEffect.Overshoot(overshootHeight, overshootDecay, overshootFrequency, OvershootMove, OvershootMove));
    }
}