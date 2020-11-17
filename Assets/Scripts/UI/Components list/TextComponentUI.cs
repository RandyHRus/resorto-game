using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextComponentUI : ListComponentUI
{
    public TextComponentUI(string text, Color32 color, int width, Transform parent): base(ResourceManager.Instance.TextListComponentUI, parent)
    {
        //Change width
        RectTransform.sizeDelta = new Vector2(width, RectTransform.sizeDelta.y);

        foreach (Transform t in ObjectTransform.GetComponentsInChildren<Transform>())
        {
            if (t.tag == "Text Field")
            {
                OutlinedText oText = new OutlinedText(t.gameObject);
                oText.SetText(text);
            }
            else if (t.tag == "Color Field 1")
            {
                Text textComponent = t.GetComponent<Text>();
                textComponent.color = color;
            }

        }
    }
}
