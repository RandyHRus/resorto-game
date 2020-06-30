using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightingManager : MonoBehaviour
{
    private Light2D mainLight;
    Coroutine currentCoroutine = null;

    private void Awake()
    {
        mainLight = GetComponent<Light2D>();
    }

    private void Start()
    {
        TimeManager.OnTurnedMorning += () => StartChangeLight(new Color32(187, 227, 255, 255), 0.8f,  20);
        TimeManager.OnTurnedMidDay  += () => StartChangeLight(new Color32(255, 255, 255, 255), 1f,    20);
        TimeManager.OnTurnedEvening += () => StartChangeLight(new Color32(255, 174, 144, 255), 0.9f,  20);
        TimeManager.OnTurnedNight   += () => StartChangeLight(new Color32(55,  136, 255, 255), 0.4f,  20);
    }

    private void StartChangeLight(Color32 targetColor, float targetIntensity, float durationSeconds)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            Debug.Log("Stopping color changing coroutine that was already running previously");
        }
        currentCoroutine = StartCoroutine(ChangeLight(targetColor, targetIntensity, durationSeconds));
    }

    IEnumerator ChangeLight(Color32 targetColor, float targetIntensity, float durationSeconds)
    {
        float timer = 0;
        float startingIntensity = mainLight.intensity;
        Color32 startingColor = mainLight.color;

        while (timer < durationSeconds)
        {
            timer += Time.deltaTime;
            if (timer > durationSeconds)
                timer = durationSeconds;

            float fraction = timer / durationSeconds;

            mainLight.intensity = Mathf.Lerp(startingIntensity, targetIntensity, fraction);
            mainLight.color = Color.Lerp(startingColor, targetColor, fraction);

            yield return 0;
        }

        currentCoroutine = null;
    }
}
