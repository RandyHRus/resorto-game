using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpEffect
{
    public delegate void ProgressFloat(float value);
    public delegate void ProgressVector(Vector2 value);

    public static IEnumerator LerpTime(float startValue, float endValue, float time, ProgressFloat valueChangedCallback, ProgressFloat endCallback)
    {
        float timer = 0;

        while (timer <= time)
        {
            timer += Time.deltaTime;
            if (timer > time)
                timer = time;

            valueChangedCallback(Mathf.Lerp(startValue, endValue, timer / time));

            yield return 0;
        }

        endCallback(endValue);
    }

    public static IEnumerator LerpVectorSpeed(Vector2 startPos, Vector2 targetPos, float speed, ProgressVector valueChangedCallback, ProgressVector endCallBack)
    {
        float distanceTravelled = 0;
        float distanceToTravel = Vector2.Distance(startPos, targetPos);

        while (distanceTravelled < distanceToTravel)
        {
            distanceTravelled += Time.deltaTime * speed;
            if (distanceTravelled >= distanceToTravel)
                distanceTravelled = distanceToTravel;

            Vector2 proposedPos = Vector2.Lerp(startPos, targetPos, distanceTravelled / distanceToTravel);
            valueChangedCallback(proposedPos);
            yield return 0;
        }

        endCallBack(targetPos);
    }
}
