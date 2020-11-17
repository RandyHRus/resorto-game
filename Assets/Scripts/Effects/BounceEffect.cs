using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceEffect
{
    static readonly float g = 10f;         // m/s/s
    static readonly float rho = 0.75f;     // coefficient of restitution
    static readonly float tau = 0.10f;     // contact time for bounce
    static readonly float hStop = 0.05f;   // stop when bounce is less than

    public delegate void Progress(float h, float xChange);
    public delegate void End();

    public static IEnumerator Bounce(float startHeight, float xSpeed, Progress moveCallback, End endCallback, bool stopOnPause)
    {
        float maxYSpeed = Mathf.Sqrt(Mathf.Abs(2 * startHeight * g));

        float h0 = startHeight;     // m/s
        float v = 0f;          // m/s, current velocity
        float t = 0f;          // starting time
        float hmax = h0;       // keep track of the maximum height
        float h = h0;
        bool freefall = true; // state: freefall or in contact
        float t_last = -Mathf.Sqrt(2 * h0 / g); // time we would have launched to get to h0 at t=0
        float vmax = Mathf.Sqrt(2 * hmax * g);

        float timeScale = stopOnPause ? Time.deltaTime : Time.unscaledDeltaTime;

        while (hmax > hStop) {
            yield return 0;

            if (freefall) {
                float hnew = (float)(h + (v * timeScale) - (0.5 * g * timeScale * timeScale));
                if (hnew < 0) {
                    t = t_last + 2 * Mathf.Sqrt(2 * hmax / g);
                    freefall = false;
                    t_last = t + tau;
                    h = 0;
                }
                else {
                    t = t + timeScale;
                    v = v - g * timeScale;
                    h = hnew;
                }
            }
            else {
                t = t + tau;
                vmax = vmax * rho;
                v = vmax;
                freefall = true;
                h = 0;
            }
            hmax = 0.5f * vmax * vmax / g;

            moveCallback(h, xSpeed * timeScale);
        }

        endCallback?.Invoke();
    }
}
