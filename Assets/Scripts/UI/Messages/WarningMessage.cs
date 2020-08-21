using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningMessage : MessageBox
{
    public WarningMessage(string text) : base(MessageManager.Instance.WarningMessageBox)
    {
        Transform t = ObjectInScene.transform;
        foreach (Transform tr in t)
        {
            if (tr.tag == "Text Field")
            {
                OutlinedText outlinedText = new OutlinedText(tr.gameObject);
                outlinedText.SetText(text);
            }
        }
    }
}