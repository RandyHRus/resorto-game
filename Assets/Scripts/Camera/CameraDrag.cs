using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDrag
{
    private static Vector3 dragOrigin;

    private static readonly Transform cameraTransform;

    static CameraDrag() {
        cameraTransform = Camera.main.transform;
    }

    public static void ExecuteLateUpdate()
    {
        if (CheckMouseOverUI.GetButtonDownAndNotOnUI("Secondary"))
        {
            dragOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return;
        }

        if (!Input.GetButton("Secondary")) return;

        cameraTransform.position = cameraTransform.position - (Camera.main.ScreenToWorldPoint(Input.mousePosition) - dragOrigin);
    }


}
