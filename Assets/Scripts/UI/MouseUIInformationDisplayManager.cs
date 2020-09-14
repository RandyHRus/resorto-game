using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseUIInformationDisplayManager
{
    private static UIObject currentShownUI;

    public static void SetShownUI(UIObject ui)
    {
        if (currentShownUI != ui)
        {
            currentShownUI?.Show(false);
            currentShownUI = ui;
        }
    }
}
