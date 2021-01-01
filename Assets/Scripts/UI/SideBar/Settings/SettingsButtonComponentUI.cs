using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SettingsButtonComponentUI : ListComponentUI
{
    private Action onButtonPressCallback;

    public SettingsButtonComponentUI(Transform parent, string text, Action onButtonPressCallback): base(ResourceManager.Instance.SettingsButtonListComponentUI, parent)
    {
        this.onButtonPressCallback = onButtonPressCallback;

        foreach (Transform t in ObjectTransform)
        {
            if (t.tag == "Text Field")
            {
                OutlinedText outlinedText = new OutlinedText(t.gameObject);
                outlinedText.SetText(text);
            }
        }
    }

    public override void OnClick()
    {
        base.OnClick();
        onButtonPressCallback?.Invoke();
    }

    public override void Destroy()
    {
        base.Destroy();
    }
}
