using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishGainMessage : ItemGainMessage
{
    public FishGainMessage(FishItemInstance item, int count) : base(item, count, MessageManager.Instance.FishGainMessageBox)
    {
        Transform t = ObjectInScene.transform;
        foreach (Transform tr in t)
        {
            if (tr.tag == "Length Field")
            {
                OutlinedText text = new OutlinedText(tr.gameObject);
                text.SetText((item.Millimetres / 10f).ToString() + "cm");
            }
        }
    }
}
