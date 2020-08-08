using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightEmitter : MonoBehaviour
{
    [SerializeField] private Light2D emitLight = null;
    [SerializeField] private float flickerSize = 1;
    [SerializeField] private float flickerSpeed = 1;
    private float defaultRange;
    private bool emitting;

    private void Awake()
    {
        defaultRange = emitLight.pointLightOuterRadius;
    }

    public void StartEmit()
    {
        emitLight.enabled = true;
        emitting = true;
        StartCoroutine(FlickerEffect());
    }

    public void StopEmit()
    {
        emitLight.enabled = false;
        emitting = false;
    }

    IEnumerator FlickerEffect()
    {
        while (emitting)
        {
            float offset = Mathf.Sin(flickerSpeed * Time.time) * flickerSize;
            emitLight.pointLightOuterRadius = defaultRange + offset;
            yield return 0;
        }
    }
}
