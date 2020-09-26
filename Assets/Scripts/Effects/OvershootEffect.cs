using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvershootEffect
{
    static readonly float hStop = 0.01f;

    public delegate void Progress(float value);
    public delegate void End();

    public static IEnumerator Overshoot(float startHeight, float hDecaySpeed, float frequency, Progress moveCallback, End endCallback)
    {
        float timer = 0;
        float h = startHeight;

        while (h > hStop)
        {
            timer += Time.deltaTime;
            h -= hDecaySpeed * Time.deltaTime;

            float value = Mathf.Sin(frequency * timer) * h;

            moveCallback(value);

            yield return 0;
        }
       
        endCallback?.Invoke();
    }
}
