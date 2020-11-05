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
        void ExpandValueChanged(float value)
        {
            //MessageBox could have been destroyed
            if (RectTransform != null)
                RectTransform.localScale = new Vector2(value, value);
        }

        void End()
        {

        }

        group = ObjectInScene.GetComponent<CanvasGroup>();
        timeRemaining = boxShowTime;

        MessageManager.Instance.ShowMessage(this);

        Coroutines.Instance.StartCoroutine(LerpEffect.LerpTime(0.5f, 1f, 0.3f, ExpandValueChanged, End, false));
        //overshootCoroutine = Coroutines.Instance.StartCoroutine(OvershootEffect.Overshoot(overshootHeight, overshootDecay, overshootFrequency, OvershootMove, OvershootMove));
    }
}