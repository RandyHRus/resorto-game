using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampPost : MonoBehaviour
{
    [SerializeField] private Sprite litSprite = null;
    [SerializeField] private Sprite unlitSprite = null;
    private SpriteRenderer sprRenderer;
    private LightEmitter lightEmitter;

    private void Awake()
    {
        lightEmitter = GetComponent<LightEmitter>();
    }

    private void Start()
    {
        TimeManager.Instance.OnTurnedMorning += UnLight;
        TimeManager.Instance.OnTurnedNight += LightUp;

        sprRenderer = GetComponent<SpriteRenderer>();
        if (sprRenderer == null)
            Debug.Log("No sprite renderer found in lamp post!");

        TimeOfDay now = TimeManager.Instance.GetTimeOfDay();
        if (now == TimeOfDay.Morning || now == TimeOfDay.MidDay)
            UnLight();
        else
            LightUp();
    }

    private void LightUp()
    {
        sprRenderer.sprite = litSprite;
        lightEmitter.StartEmit();
    }
    
    private void UnLight()
    {
        sprRenderer.sprite = unlitSprite;
        lightEmitter.StopEmit();
    }

    private void OnDestroy()
    {
        TimeManager.Instance.OnTurnedMorning -= UnLight;
        TimeManager.Instance.OnTurnedNight -= LightUp;
    }
}
