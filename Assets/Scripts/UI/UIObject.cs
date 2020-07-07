using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIObject
{
    public GameObject ObjectInScene { get; }
    public RectTransform RectTransform { get; }
    public Transform ObjectTransform { get; }

    public UIObject(GameObject prefab, Transform parent)
    {
        ObjectInScene = GameObject.Instantiate(prefab);
        RectTransform = ObjectInScene.GetComponent<RectTransform>();
        ObjectTransform = ObjectInScene.transform;
        ObjectTransform.SetParent(parent, false);
    }

    public void Show(bool show)
    {
        ObjectInScene.SetActive(show);
    }
}