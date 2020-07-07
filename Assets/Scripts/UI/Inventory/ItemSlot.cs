using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : UIObject
{
    public int capacity = 5;

    public InventoryItemInformation Item { get; private set; } = null;
    public int Count { get; private set; } = 0;
    private Image iconImage;
    private Text itemCountText;

    public ItemSlot(): base(ResourceManager.Instance.ItemSlot, ResourceManager.Instance.InventoryCanvas.transform)
    {
        Transform t = ObjectInScene.transform;
        foreach (Transform tr in t)
        {
            if (tr.tag == "Item Count Field")
            {
                itemCountText = tr.GetComponent<Text>();
            }
            else if (tr.tag == "Item Icon Field")
            {
                iconImage = tr.GetComponent<Image>();
            }
        }

        iconImage.enabled = false; //To remove white space
        itemCountText.enabled = false;
    }

    public void ClearSlot()
    {
        Item = null;
        Count = 0;
    }


    public void SetSlot(InventoryItemInformation item, int count)
    {
        this.Item = item;
        this.Count = count;

        if (count <= 0)
        {
            this.Item = null;
            this.Count = 0;
        }
    }

    public void RefreshUI()
    {
        if (Item == null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;

            itemCountText.text = "";
            itemCountText.enabled = false;
        }
        else
        {
            iconImage.sprite = Item.ItemIcon;
            iconImage.enabled = true;

            itemCountText.text = Count.ToString();
            itemCountText.enabled = true;
        }
    }

}
