using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Selection : UIObject
{
    private SelectionPanel selectionPanel;

    private Color32 highlightColor = new Color32(189, 189, 189, 255);
    private Image buttonImage;

    public Selection(GameObject prefab, SelectionPanel parent): base(prefab, parent.ObjectTransform)
    {
        selectionPanel = parent;
        Button button = ObjectInScene.GetComponent<Button>();
        buttonImage = button.GetComponent<Image>();
        AddListener(button);
    }

    private void AddListener(Button button)
    {
        button.onClick.AddListener(OnClick);
    }

    public virtual void OnClick()
    {
        selectionPanel.ChangeSelectedSelection(this);
    }

    public void Highlight(bool highlight)
    {
        buttonImage.color = highlight ? highlightColor : (Color32)Color.white;
    }
}