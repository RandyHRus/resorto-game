﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemGainMessage : MessageBox
{
    public ItemGainMessage(InventoryItemInformation item, int count) : base(MessageManager.Instance.ItemGainMessageBox)
    {
        //Find child components
        Transform t = ObjectInScene.transform;
        foreach (Transform tr in t)
        {
            if (tr.tag == "Count Field")
            {
                OutlinedText text = new OutlinedText(tr.gameObject);
                text.SetText("x" + count.ToString());
            }
            else if (tr.tag == "Name Field")
            {
                OutlinedText text = new OutlinedText(tr.gameObject);
                text.SetText(item.ItemName);
                text.SetColor(ResourceManager.Instance.ItemTagColors[(int)item.Tag]);
            }
            else if (tr.tag == "Icon Field")
            {
                tr.GetComponent<Image>().sprite = item.ItemIcon;
            }
        }
    }
}