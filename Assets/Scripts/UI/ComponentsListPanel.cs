using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentsListPanel<T> : UIObject where T: ListComponentUI
{
    private RectTransform contentTransform;

    private List<T> components;
    private float panelContentHeight = 0;

    private readonly float PADDING = 3f;

    public ComponentsListPanel(Transform parent, Vector2 position): base (ResourceManager.Instance.ComponentsListPanel, parent)
    {
        RectTransform.anchoredPosition = position;
        Initialize();
    }

    public ComponentsListPanel(GameObject instance): base (instance)
    {
        Initialize();
    }

    private void Initialize()
    {
        Transform[] allChildren = ObjectTransform.GetComponentsInChildren<Transform>();

        foreach (Transform t in allChildren)
        {
            if (t.tag == "Content Field")
            {
                contentTransform = t.GetComponent<RectTransform>();
            }
        }

        components = new List<T>();
        panelContentHeight = PADDING;
    }

    public virtual void InsertListComponent(T comp)
    {
        components.Add(comp);

        float objectHeight = comp.RectTransform.sizeDelta.y;

        panelContentHeight += objectHeight + PADDING;
        contentTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, panelContentHeight);

        Vector2 pos = new Vector2(0, -((PADDING * components.Count) + (objectHeight * (components.Count - 1))));

        comp.RectTransform.anchoredPosition = pos;
        comp.ObjectTransform.SetParent(contentTransform, false);
    }

    public virtual void RemoveListComponent(T comp)
    {
        //Resize panel content
        float objectHeight = comp.RectTransform.sizeDelta.y;
        float contentHeightToRemove = objectHeight + PADDING;
        panelContentHeight -= contentHeightToRemove;
        contentTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, panelContentHeight);

        //We need to change the position of every component that comes after this component
        for (int i = components.IndexOf(comp) + 1; i < components.Count; i++)
        {
            components[i].RectTransform.anchoredPosition -= new Vector2(0, contentHeightToRemove);
        }

        //Remove actual component
        comp.Destroy();
        components.Remove(comp);
    }


    public void SignalContentSizeChange(T comp, float change, float speed)
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
}
