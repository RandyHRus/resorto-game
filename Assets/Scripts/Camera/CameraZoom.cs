using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    Camera cam;
    int currentIndex = 1;
    float[] cameraSizes = new float[]
    {
        4f, 8f, 12, 16f, 20f, 24f, 28f, 32f  
    };

    void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographicSize = cameraSizes[currentIndex];
    }

    void Update()
    {
        if (Input.GetButtonDown("Zoom in"))
        {
            currentIndex--;
            if (currentIndex < 0)
                currentIndex = 0;
            else
                cam.orthographicSize = cameraSizes[currentIndex];
        }
        else if (Input.GetButtonDown("Zoom out"))
        {
            currentIndex++;
            if (currentIndex > cameraSizes.Length - 1) 
                currentIndex = cameraSizes.Length - 1;
            else
                cam.orthographicSize = cameraSizes[currentIndex];
        }
    }
}
