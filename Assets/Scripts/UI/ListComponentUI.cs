using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ListComponentUI : UIObject
{
    public delegate void Selected(ListComponentUI selection);
    public event Selected OnSelect;

    private Image buttonImage;
    protected Button button;

    private void AddListener(Button button)
    {
        this.button = button;
        button.onClick.AddListener(OnClick);
        OnDestroy += UnSub;
    }

    public virtual void OnClick()
    {
        OnSelect?.Invoke(this);
    }

    public ListComponentUI(GameObject prefab, Transform parent) : base(prefab, parent)
    {
        foreach (Transform t in ObjectTransform.GetComponentsInChildren<Transform>())
        {
            if (t.tag == "Background Field")
            {
                Button button = t.gameObject.GetComponent<Button>();
                buttonImage = t.gameObject.GetComponent<Image>();
                AddListener(button);
                break;
            }
        }
    }

    public void Highlight(bool highlight)
    {
        buttonImage.color = highlight ? ResourceManager.Instance.UIHighlightColor : (Color32)Color.white;
    }

    public void ShiftComponent(float change, float speed)
    {
        Vector2 startPos;

        void ShiftProgress(float value)
        {
            RectTransform.anchoredPosition = new Vector2(0, value); 
        }

        startPos = RectTransform.anchoredPosition;
        Coroutines.Instance.StartCoroutine(LerpEffect.LerpSpeed(startPos.y, startPos.y + change, speed, ShiftProgress, null, false));
    }

    /*
    public void GreyOut()
    {
        button.onClick.RemoveListener(OnClick);
        buttonImage.color = new Color32(255, 255, 255, 100);
    }
    */

    private void UnSub()
    {
        OnDestroy -= UnSub;
        button.onClick.RemoveListener(OnClick);
    }
}
