using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar: UIObject
{
    private Transform fillBarTransform;

    public ProgressBar(Canvas canvas): base(ResourceManager.Instance.Progressbar, canvas.transform)
    {
        foreach (Transform t in ObjectTransform)
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
