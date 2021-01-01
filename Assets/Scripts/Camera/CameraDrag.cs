using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDrag: MonoBehaviour
{
    private Vector3 dragOrigin;

    private Transform cameraTransform;

    private static CameraDrag _instance;
    public static CameraDrag Instance { get { return _instance; } }
    private void Awake()
    {
        //Singleton
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }

        cameraTransform = Camera.main.transform;
    }

    public void ExecuteLateUpdate()
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
