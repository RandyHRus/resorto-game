using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OutlinedText: UIObject
{
    private Text outline;
    private Text innerText;
    CanvasGroup group;

    //Existing instance
    public OutlinedText(GameObject objectInScene): base (objectInScene)
    {
        Initialize();
    }

    //Creates new instance
    public OutlinedText(Transform parent): base (ResourceManager.Instance.OutlinedText, parent)
    {
        Initialize();
    }

    private void Initialize()
    {
        outline = ObjectInScene.GetComponent<Text>();
        innerText = ObjectInScene.GetComponentsInChildren<Text>()[1];
        group = ObjectInScene.GetComponent<CanvasGroup>();
    }

    public void SetText(string text)
    {
        outline.text = text;
        innerText.text = text;
    }

    public void SetColor(Color32 color)
    {
        innerText.color = color;
    }

    public void SetAlpha(float frac)
    {
        group.alpha = frac;
    }
}
