using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampPost : MonoBehaviour
{
    [SerializeField] private Sprite litSprite = null;
    [SerializeField] private Sprite unlitSprite = null;
    private SpriteRenderer sprRenderer;


    private void Start()
    {
        TimeManager.onTurnedDay += ChangeToUnlitSprite;
        TimeManager.onTurnedNight += ChangeToLitSprite;

        sprRenderer = GetComponent<SpriteRenderer>();
        if (sprRenderer == null)
            Debug.Log("No sprite renderer found in lamp post!");

        if (TimeManager.Instance.IsDay())
            ChangeToUnlitSprite();
        else
            ChangeToLitSprite();
    }

    private void ChangeToLitSprite()
    {
        sprRenderer.sprite = litSprite;
    }
    
    private void ChangeToUnlitSprite()
    {
        sprRenderer.sprite = unlitSprite;
    }

    private void OnDestroy()
    {
        TimeManager.onTurnedDay -= ChangeToUnlitSprite;
        TimeManager.onTurnedNight -= ChangeToLitSprite;
    }
}
