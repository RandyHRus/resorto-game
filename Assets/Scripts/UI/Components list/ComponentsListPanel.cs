using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ComponentsListPanel : UIObject
{
    public ComponentsListPanel(Transform parent): base(ResourceManager.Instance.ComponentsListPanel, parent)
    {

    }

    public ComponentsListPanel(GameObject instance): base(instance)
    {

    }

    public abstract void OnContentSizeChange(ListComponentUI comp, float change, float speed);
}

public class ComponentsListPanel<T> : ComponentsListPanel where T: ListComponentUI
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
    public ComponentsListPanel(Transform parent, Vector2 position, float xSize, bool isFixed): base (parent)
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

        //Debug.Log(-((FIXED_PADDING * components.Count) + (objectHeight * (components.Count - 1))));

        Vector2 pos = new Vector2(0, -panelContentHeight);

        panelContentHeight += objectHeight + FIXED_PADDING;
        UpdateContentRectTransformSize();

        comp.RectTransform.anchoredPosition = pos;
        comp.ObjectTransform.SetParent(contentTransform, false);

        comp.OnSelected += InvokeSelected;
        comp.OnDestroy += OnComponentDestroyed;

        if (comp is CollapsibleComponentUI collapsible)
        {
            collapsible.OnCallapseStart += OnContentSizeChange;
        }

        comp.OnSelected += OnComponentSelectedHandler;
    }

    public void RemoveListComponent(T comp)
    {
        //This will trigger OnComponentDestroyed through events
        comp.Destroy(); 
    }

    public virtual void OnComponentDestroyed(UIObject sender)
    {
        RemoveListComponentOnDestroy((T)sender);
    }

    private void RemoveListComponentOnDestroy(T comp)
    {
        //Resize panel content
        float objectHeight = comp.RectTransform.sizeDelta.y;
        float contentHeightToRemove = objectHeight + FIXED_PADDING;
        {
            if (comp is CollapsibleComponentUI collapsible)
                if (!collapsible.Collapsed)
                    contentHeightToRemove += collapsible.CollapsibleTargetSize;
        }

        panelContentHeight -= contentHeightToRemove;

        UpdateContentRectTransformSize();

        //We need to change the position of every component that comes after this component
        for (int i = components.IndexOf(comp) + 1; i < components.Count; i++)
        {
            components[i].RectTransform.anchoredPosition += new Vector2(0, contentHeightToRemove);
        }

        comp.OnSelected -= InvokeSelected;
        {
            if (comp is CollapsibleComponentUI collapsible)
            {
                collapsible.OnCallapseStart -= OnContentSizeChange;
            }
        }
        comp.OnDestroy -= OnComponentDestroyed;
        comp.OnSelected -= OnComponentSelectedHandler;

        components.Remove(comp);
    }

    public override void OnContentSizeChange(ListComponentUI comp, float change, float speed)
    {
        int componentIndex = components.IndexOf((T)comp);

        //Change content size;
        panelContentHeight += change;
        UpdateContentRectTransformSize();

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

    private void UpdateContentRectTransformSize()
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

    private void OnDestroyHandler(UIObject sender)
    {
        //To make sure everything is destroyed and unsubbed
        ClearComponents();

        //Unsub
        OnDestroy -= OnDestroyHandler;
    }

    public void ClearComponents()
    {
        while (components.Count > 0)
        {
            RemoveListComponent(components[components.Count - 1]);
        }
    }

    private void InvokeSelected(ListComponentUI component)
    {
        OnSelected?.Invoke(component);
    }

    public virtual void OnComponentSelectedHandler(ListComponentUI selection)
    {
        if (selection is CollapsibleComponentUI collapsible)
            collapsible.Collapse(!collapsible.Collapsed);
    }
}
