using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComponentsListPanel<T> : UIObject where T: ListComponentUI
{
    private RectTransform contentTransform;
    private ScrollRect scrollRect;

    private List<T> components;
    private float panelContentHeight = 0;

    private readonly float FIXED_PADDING = 3f;
    private readonly float NON_FIXED_PADDING = 24f;

    private readonly bool isFixed;

    public delegate void Selected(ListComponentUI component);
    public event Selected OnSelected;

    // If IsFixed is set to true, you get a scroll bar when content gets too large
    // If IsFixed is set to false, the whole thing gets larger
    public ComponentsListPanel(Transform parent, Vector2 position, float xSize, bool isFixed): base (ResourceManager.Instance.ComponentsListPanel, parent)
    {
        this.isFixed = isFixed;

        RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, xSize);
        RectTransform.ForceUpdateRectTransforms();

        ObjectTransform.position = position;
        Initialize();
    }

    public ComponentsListPanel(GameObject instance): base (instance)
    {
        this.isFixed = true;
        Initialize();
    }

    private void Initialize()
    {
        Transform[] allChildren = ObjectTransform.GetComponentsInChildren<Transform>();
        scrollRect = ObjectTransform.GetComponent<ScrollRect>();

        foreach (Transform t in allChildren)
        {
            if (t.tag == "Content Field")
            {
                contentTransform = t.GetComponent<RectTransform>();
            }
        }

        components = new List<T>();
        panelContentHeight = FIXED_PADDING;

        OnDestroy += OnDestroyHandler;
    }

    public virtual void InsertListComponent(T comp)
    {
        components.Add(comp);

        float objectHeight = comp.RectTransform.sizeDelta.y;

        panelContentHeight += objectHeight + FIXED_PADDING;

        UpdateRectTransformSize();

        Vector2 pos = new Vector2(0, -((FIXED_PADDING * components.Count) + (objectHeight * (components.Count - 1))));
        comp.RectTransform.anchoredPosition = pos;
        comp.ObjectTransform.SetParent(contentTransform, false);

        comp.OnSelect += InvokeSelected;
    }

    public virtual void RemoveListComponent(T comp)
    {
        //Resize panel content
        float objectHeight = comp.RectTransform.sizeDelta.y;
        float contentHeightToRemove = objectHeight + FIXED_PADDING;
        panelContentHeight -= contentHeightToRemove;

        UpdateRectTransformSize();

        //We need to change the position of every component that comes after this component
        for (int i = components.IndexOf(comp) + 1; i < components.Count; i++)
        {
            components[i].RectTransform.anchoredPosition -= new Vector2(0, contentHeightToRemove);
        }

        comp.OnSelect -= InvokeSelected;

        //Remove actual component
        comp.Destroy();
        components.Remove(comp);
    }


    public void OnContentSizeChange(T comp, float change, float speed)
    {
        int componentIndex = components.IndexOf(comp);

        //Change content size;
        contentTransform.sizeDelta += new Vector2(0, change);

        //Shift below components
        for (int i = componentIndex + 1; i < components.Count; i++)
        {
            T thisComp = components[i];
            thisComp.ShiftComponent(-change, speed);
        }
    }

    public void SetScrollToComponent(T listComponent)
    {
        //Thanks https://stackoverflow.com/questions/30766020/how-to-scroll-to-a-specific-element-in-scrollrect-with-unity-ui
        contentTransform.anchoredPosition =
            (Vector2)scrollRect.transform.InverseTransformPoint(contentTransform.position)
            - (Vector2)scrollRect.transform.InverseTransformPoint(listComponent.ObjectTransform.position);
    }

    private void UpdateRectTransformSize()
    {
        Vector2 contentSize = new Vector2(RectTransform.sizeDelta.x, panelContentHeight);
        contentTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, contentSize.y);
        contentTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, contentSize.x);
        contentTransform.ForceUpdateRectTransforms();

        if (!isFixed)
        {
            Vector2 rectSize = new Vector2(RectTransform.sizeDelta.x, panelContentHeight + NON_FIXED_PADDING);
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rectSize.y);
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectSize.x);
            RectTransform.ForceUpdateRectTransforms();
        }
    }

    private void OnDestroyHandler()
    {
        //To make sure everything is destroyed and unsubbed
        while(components.Count > 0)
        {
            RemoveListComponent(components[components.Count - 1]);
        }

        //Unsub
        OnDestroy -= OnDestroyHandler;

    }

    private void InvokeSelected(ListComponentUI component)
    {
        OnSelected?.Invoke(component);
    }
}
