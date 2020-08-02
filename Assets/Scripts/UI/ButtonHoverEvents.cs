using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool mouseOver;
    private Vector2 defaultPosition;
    private RectTransform rectTransform;

    private float waveHeight = 3f;
    private float waveSpeed = 5f;

    private void Awake()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
        defaultPosition = rectTransform.anchoredPosition;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
        StartCoroutine(HoverEffect());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
        rectTransform.anchoredPosition = defaultPosition;
    }

    IEnumerator HoverEffect()
    {
        while (mouseOver)
        {
            float offset = Mathf.Sin(waveSpeed * Time.time) * waveHeight;
            rectTransform.anchoredPosition = new Vector2(defaultPosition.x, defaultPosition.y + offset);

            yield return 0;
        }
    }
}
