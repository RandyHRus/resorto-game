using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouristLeftMessage : MessageBox
{
    public TouristLeftMessage(TouristInstance tourist) : base(MessageManager.Instance.TouristLeftMessage)
    {
        Transform t = ObjectInScene.transform;
        foreach (Transform tr in t)
        {
            if (tr.tag == "Name Field")
            {
                OutlinedText outlinedText = new OutlinedText(tr.gameObject);
                outlinedText.SetText(tourist.TouristInformation.NpcName);
            }
        }
    }
}
