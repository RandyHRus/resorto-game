using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CosmeticGainMessage : ItemGainMessage
{
    public CosmeticGainMessage(CosmeticItemInstance item, int count): base(item, count, MessageManager.Instance.CosmeticGainMessageBox)
    {
        Transform t = ObjectInScene.transform;
        foreach (Transform tr in t)
        {
            if (tr.tag == "Color Field 1")
            {
                Image[] components = tr.GetComponentsInChildren<Image>();
                Image outlineImage = components[0];
                Image colorImage = components[1];

                if (item.PrimaryColor == null)
                {
                    outlineImage.enabled = false;
                    colorImage.enabled = false;
                }
                else
                {
                    colorImage.color = (Color32)item.PrimaryColor;
                }
            }

            if (tr.tag == "Color Field 2")
            {
                Image[] components = tr.GetComponentsInChildren<Image>();
                Image outlineImage = components[0];
                Image colorImage = components[1];

                if (item.SecondaryColor == null)
                {
                    outlineImage.enabled = false;
                    colorImage.enabled = false;
                }
                else
                {
                    colorImage.color = (Color32)item.SecondaryColor;
                }
            }
        }
    }
}
