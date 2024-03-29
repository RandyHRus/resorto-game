﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvershootEffect
{
    static readonly float hStop = 0.01f;

    public delegate void Progress(float value);
    public delegate void End();

    public static IEnumerator Overshoot(float startHeight, float hDecaySpeed, float frequency, Progress moveCallback, End endCallback, bool stopOnPause)
    {
        float timer = 0;
        float h = startHeight;

        while (h > hStop)
        {
            yield return 0;

            timer += (stopOnPause ? Time.deltaTime : Time.unscaledDeltaTime);
            h -= hDecaySpeed * (stopOnPause ? Time.deltaTime : Time.unscaledDeltaTime);

            float value = Mathf.Sin(frequency * timer) * h;

            moveCallback(value);
        }
       
        endCallback?.Invoke();
    }
}
