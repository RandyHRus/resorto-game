using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlotUI : UIObject
{
    private Coroutine changeSizeCoroutine;
    private float enlargeSize = 1.2f;
    private float enlargeSpeed = 2f;

    public InventorySlot Slot { get; private set; }

    public InventorySlotUI(InventorySlot slot, GameObject prefab, Transform parent): base (prefab, parent)
    {
        this.Slot = slot;
        ObjectInScene.AddComponent<InventorySlotInteraction>().slotUI = this;
    }

    public virtual void StartShrink()
    {
        if (changeSizeCoroutine != null)
            Coroutines.Instance.StopCoroutine(changeSizeCoroutine);

        changeSizeCoroutine = Coroutines.Instance.StartCoroutine(Shrink());
    }

    private IEnumerator Shrink()
    {
        while (RectTransform.localScale.x > 1)
        {
            float scale = RectTransform.localScale.x - (enlargeSpeed * Time.deltaTime);
            if (scale < 1)
                scale = 1;
            RectTransform.localScale = new Vector2(scale, scale);
            yield return 0;
        }
    }

    public virtual void StartEnlarge()
    {
        if (changeSizeCoroutine != null)
            Coroutines.Instance.StopCoroutine(changeSizeCoroutine);

        RectTransform.SetAsLastSibling();
        changeSizeCoroutine = Coroutines.Instance.StartCoroutine(Enlarge());
    }

    private IEnumerator Enlarge()
    {
        while (RectTransform.localScale.x < enlargeSize)
        {
            float scale = RectTransform.localScale.x + (enlargeSpeed * Time.deltaTime);
            if (scale > enlargeSize)
                scale = enlargeSize;
            RectTransform.localScale = new Vector2(scale, scale);
            yield return 0;
        }
    }
}
