using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class RightClickOptionUI : ListComponentUI
{
    public RightClickOptionUI(string text, Transform parent): base(ResourceManager.Instance.RightClickOptionUIPrefab, parent)
    {
        foreach (Transform t in ObjectTransform.GetComponentsInChildren<Transform>())
        {
            if (t.tag == "Text Field")
            {
                OutlinedText outliendText = new OutlinedText(t.gameObject);
                outliendText.SetText(text);
            }
        }
    }
}
