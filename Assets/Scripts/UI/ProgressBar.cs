using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar: UIObject
{
    private Transform fillBarTransform;

    public ProgressBar(Transform parent): base(ResourceManager.Instance.Progressbar, parent)
    {
        Initialize();
    }

    public ProgressBar(GameObject instance): base(instance)
    {
        Initialize();
    }

    private void Initialize()
    {
        foreach (Transform t in ObjectTransform.GetComponentsInChildren<Transform>())
        {
            if (t.tag == "Image component")
            {
                fillBarTransform = t;
            }
        }
    }

    //XScale 0-1
    public void SetFill(float xScale)
    {
        fillBarTransform.localScale = new Vector3(xScale, 1, 1);
    }
}
