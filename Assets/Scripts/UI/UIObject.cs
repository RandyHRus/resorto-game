using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIObject
{
    public GameObject ObjectInScene { get; }
    public RectTransform RectTransform { get; }
    public Transform ObjectTransform { get; }

    //Instantiates a object
    public UIObject(GameObject prefab, Transform parent)
    {
        ObjectInScene = GameObject.Instantiate(prefab);
        RectTransform = ObjectInScene.GetComponent<RectTransform>();
        ObjectTransform = ObjectInScene.transform;
        ObjectTransform.SetParent(parent, false);
    }

    //Existing object
    public UIObject(GameObject instance)
    {
        ObjectInScene = instance;
        RectTransform = ObjectInScene.GetComponent<RectTransform>();
        ObjectTransform = ObjectInScene.transform;
    }

    public virtual void Show(bool show)
    {
        ObjectInScene.SetActive(show);
    }

    public virtual void Destroy()
    {
        GameObject.Destroy(ObjectInScene);
    }
}