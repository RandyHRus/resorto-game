using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionPanel: UIObject
{
    private Transform contentTransform;

    private List<Selection> selections;
    private float panelContentHeight = 0;

    private int PADDING = 3;

    public SelectionPanel(Canvas canvas, Vector2 position) : base(ResourceManager.Instance.selectionPanel, canvas.transform)
    {
        ObjectInScene.GetComponent<RectTransform>().anchoredPosition = position;
        contentTransform = ObjectTransform.Find("Viewport/Content");
        selections = new List<Selection>();
        panelContentHeight = PADDING;
    }

    public void InsertSelection(Selection selection)
    {
        selections.Add(selection);

        float objectHeight = selection.RectTransform.sizeDelta.y;
        panelContentHeight += objectHeight + PADDING;
        contentTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(RectTransform.sizeDelta.x, panelContentHeight);

        
        Vector2 pos = new Vector2(0, -((PADDING * selections.Count) + (objectHeight * (selections.Count - 1)) + (objectHeight / 2)));
        selection.RectTransform.anchoredPosition = pos;
        selection.ObjectTransform.SetParent(contentTransform, false);
    }
}
