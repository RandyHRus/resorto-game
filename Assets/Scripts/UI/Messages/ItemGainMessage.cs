using System.Collections;
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
                tr.GetComponent<Text>().text = "x" + count.ToString();
            }
            else if (tr.tag == "Name Field")
            {
                tr.GetComponent<Text>().text = item.ItemName;
            }
            else if (tr.tag == "Icon Field")
            {
                tr.GetComponent<Image>().sprite = item.ItemIcon;
            }
        }
    }
}