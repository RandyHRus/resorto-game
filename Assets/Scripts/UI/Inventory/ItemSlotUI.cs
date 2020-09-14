using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : InventorySlotUI
{
    private Image iconImage;
    private OutlinedText itemCountText;

    public ItemSlotUI(ItemInventorySlot slotInfo, Transform parent, Sprite overrideSprite = null) : base(slotInfo, ResourceManager.Instance.ItemSlot, parent)
    {
        Transform t = ObjectInScene.transform;
        foreach (Transform tr in t)
        {
            if (tr.tag == "Count Field")
            {
                itemCountText = new OutlinedText(tr.gameObject);
            }
            else if (tr.tag == "Icon Field")
            {
                iconImage = tr.GetComponent<Image>();
            }
        }

        if (overrideSprite != null)
            ObjectInScene.GetComponent<Image>().sprite = overrideSprite;

        iconImage.enabled = false; //To remove white square
        itemCountText.SetText("");

        ((ItemInventorySlot)this.Slot).SlotChanged += RefreshUI;

        RefreshUI(); //Show UI
    }

    private void RefreshUI()
    {
        ItemInventorySlot itemInventorySlot = ((ItemInventorySlot)this.Slot);

        if (itemInventorySlot.Item == null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;

            itemCountText.SetText("");
        }
        else
        {
            iconImage.sprite = itemInventorySlot.Item.ItemInformation.ItemIcon;
            iconImage.enabled = true;

            itemCountText.SetText(itemInventorySlot.Count > 1 ? itemInventorySlot.Count.ToString() : "");
        }
    }

    public new void Destroy()
    {
        ((ItemInventorySlot)this.Slot).SlotChanged -= RefreshUI;
        base.Destroy();
    }
}
