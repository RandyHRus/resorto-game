using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRate : MonoBehaviour
{
    int[] cycleFrameRates = new int[6] { -1, 5, 10, 30, 60, 100 };
    int index = 0;

    private void Update()
    {
        if(Input.GetButtonDown("FrameRate"))
        {
            index++;

            if (index >= cycleFrameRates.Length)
                index = 0;
            Application.targetFrameRate = cycleFrameRates[index];
        }
    }
}
