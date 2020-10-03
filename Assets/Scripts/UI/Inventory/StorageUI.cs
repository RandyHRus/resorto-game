using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageUI : InventoryExternalUI
{
    private static int edgesPadding = 10;
    private static int slotPadding = 44;

    private ItemSlotUI[] slots;

    public StorageUI(Storage storage) : base(ResourceManager.Instance.ResizablePanel, ResourceManager.Instance.InventoryCanvas.transform)
    {
        //Set up background
        float panelSizeX = ((slotPadding * storage.SlotCountX) + (2 * edgesPadding));
        float panelSizeY = ((slotPadding * storage.SlotCountY) + (2 * edgesPadding));
        RectTransform.sizeDelta = new Vector2(panelSizeX, panelSizeY);

        int posX = Screen.width / 2;
        int posY = Screen.height / 3;
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(posX, posY, 0));
        ObjectTransform.position = worldPos;

        slots = new ItemSlotUI[storage.SlotCountX * storage.SlotCountY];

        //Create slotsUI
        for (int x = 0; x < storage.SlotCountX; x++)
        {
            for (int y = 0; y < storage.SlotCountY; y++)
            {
                //Create slot UI
                int index = x + (storage.SlotCountX * y);
                ItemSlotUI slotUI = new ItemSlotUI(storage.GetStorageSlotInformation(index), ObjectTransform);
                slots[index] = slotUI;

                //Set position
                {
                    int slotPosX = edgesPadding + (slotPadding * x) + (slotPadding / 2);
                    int slotPosY = -(edgesPadding + (slotPadding * y) + (slotPadding / 2));
                    slotUI.RectTransform.anchoredPosition = new Vector2(slotPosX, slotPosY);
                }
            }
        }
    }

    public override void Show(bool show)
    {
        if (!show)
        {
            foreach (ItemSlotUI slot in slots)
            {
                //To make sure event unsubscribed
                slot.Destroy();
            }
            Destroy();
        }
        else
        {
            base.Show(show);
        }
    }
}
