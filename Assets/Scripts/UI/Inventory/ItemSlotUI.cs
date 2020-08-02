using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : UIObject
{
    private Image iconImage;
    private Text itemCountText;

    private ItemSlotInformation slotInfo;

    public ItemSlotUI(ItemSlotInformation slotInfo, Transform parent) : base(ResourceManager.Instance.ItemSlot, parent)
    {
        Transform t = ObjectInScene.transform;
        foreach (Transform tr in t)
        {
            if (tr.tag == "Count Field")
            {
                itemCountText = tr.GetComponent<Text>();
            }
            else if (tr.tag == "Icon Field")
            {
                iconImage = tr.GetComponent<Image>();
            }
        }

        iconImage.enabled = false; //To remove white square
        itemCountText.enabled = false;

        this.slotInfo = slotInfo;
        this.slotInfo.OnSlotChanged += RefreshUI;

        RefreshUI(); //Show UI
    }

    private void RefreshUI()
    {
        if (slotInfo.Item == null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;

            itemCountText.text = "";
            itemCountText.enabled = false;
        }
        else
        {
            iconImage.sprite = slotInfo.Item.ItemIcon;
            iconImage.enabled = true;

            itemCountText.text = slotInfo.Count.ToString();
            itemCountText.enabled = true;
        }
    }

    public new void Destroy()
    {
        this.slotInfo.OnSlotChanged -= RefreshUI;
        base.Destroy();
    }
}
