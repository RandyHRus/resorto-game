using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ListComponentUI : UIObject
{
    public delegate void Selected(ListComponentUI selection);
    public event Selected OnSelect;

    /* This is required because we can't get the content height of the object in script, without a 1 frame delay
     * It will become messy if we have to wait 1 frame to get the height of the object, so we will just manually set it here
     */
    public virtual int ObjectHeight => 40;

    private Image buttonImage;

    private void AddListener(Button button)
    {
        button.onClick.AddListener(OnClick);
    }

    public virtual void OnClick()
    {
        OnSelect?.Invoke(this);
    }

    public ListComponentUI(GameObject prefab, Transform parent) : base(prefab, parent)
    {
        foreach (Transform t in ObjectTransform)
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
        Coroutines.Instance.StartCoroutine(LerpEffect.LerpSpeed(startPos.y, startPos.y + change, speed, ShiftProgress, null));
    }
}
