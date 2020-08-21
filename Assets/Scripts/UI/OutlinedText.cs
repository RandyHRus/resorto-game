using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OutlinedText
{
    private Text outline;
    private Text innerText;

    public OutlinedText(GameObject objectInScene)
    {
        outline = objectInScene.GetComponent<Text>();
        innerText = objectInScene.GetComponentsInChildren<Text>()[1];
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
}
