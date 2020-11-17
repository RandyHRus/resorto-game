using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollapsibleComponentUI : ListComponentUI
{
    public bool Collapsed { get; private set; }

    Coroutine collapseCoroutine = null;
    private bool collapseRunning = false;

    private static float collapseSpeed = 2000;

    public abstract int CollapsibleTargetSize { get; }

    public delegate void CollapseStartDelegate(CollapsibleComponentUI caller, float targetY, float speed);
    public event CollapseStartDelegate OnCallapseStart;

    public delegate void CollapseEndDelegate(CollapsibleComponentUI caller);
    public event CollapseEndDelegate OnCollapseEnd;

    private UIObject collapsible;

    public CollapsibleComponentUI(GameObject prefab, Transform parent) : base(prefab, parent)
    {
        foreach (Transform t in ObjectTransform.GetComponentsInChildren<Transform>())
        {
            if (t.tag == "Collapsible Field")
            {
                collapsible = new UIObject(t.gameObject);
                Coroutines.Instance.StartCoroutine(DelayedInitialize());
            }
        }

        Collapsed = true;
    }

    IEnumerator DelayedInitialize()
    {
        yield return 0;
        collapsible.ObjectInScene.SetActive(false);
    }

    public void Collapse(bool bCollapse)
    {
        void OnCollapseProgress(float value)
        {
            collapsible.RectTransform.sizeDelta = new Vector2(collapsible.RectTransform.sizeDelta.x, value);
        }

        void OnCollapseEnded()
        {
            if (Collapsed)
                collapsible.ObjectInScene.SetActive(false); //To fix weird button raycast collision bug

            collapseRunning = false;

            OnCollapseEnd?.Invoke(this);
        }

        if (Collapsed == bCollapse)
            return;

        Collapsed = bCollapse;

        if (collapseRunning == true)
            Coroutines.Instance.StopCoroutine(collapseCoroutine);

        collapseRunning = true;

        if (!Collapsed)
            collapsible.ObjectInScene.SetActive(true);

        int targetYSize = Collapsed ? 0 : CollapsibleTargetSize;
        Vector2 currentSize = collapsible.RectTransform.sizeDelta;

        float startValue = currentSize.y;

        collapseCoroutine = Coroutines.Instance.StartCoroutine(LerpEffect.LerpSpeed(startValue, targetYSize, collapseSpeed, OnCollapseProgress, OnCollapseEnded, false));
        OnCallapseStart?.Invoke(this, targetYSize - startValue, collapseSpeed);
    }
}
