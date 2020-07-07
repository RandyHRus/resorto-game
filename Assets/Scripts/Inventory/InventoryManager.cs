using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject dropitemSlotPrefab = null;
    [SerializeField] private InventoryItemInformation[] starterItems = null;
    [SerializeField] private Transform playerTransform = null;
    private int inventorySize_h = 10;
    private int inventorySize_v = 3;
    private int totalSlotCount;

    bool isInventoryOpen;

    private Storage inventory;
    private ItemSlot mouseDraggingSlotInformation = null;
    private GameObject itemDropSlot;

    private Color32 selectedSlotColor = new Color32(189, 189, 189, 255);

    private int selectedSlotIndex = -1;
    private ItemSlot selectedSlot = null;

    private KeyCode[] numberKeys = {
         KeyCode.Alpha1,
         KeyCode.Alpha2,
         KeyCode.Alpha3,
         KeyCode.Alpha4,
         KeyCode.Alpha5,
         KeyCode.Alpha6,
         KeyCode.Alpha7,
         KeyCode.Alpha8,
         KeyCode.Alpha9,
         KeyCode.Alpha0,
     };

    public delegate void ItemGained(InventoryItemInformation item, int count);
    public static event ItemGained OnItemGained;

    private static InventoryManager _instance;
    public static InventoryManager Instance { get { return _instance; } }
    private void Awake()
    {
        //Singleton
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }
        {
            totalSlotCount = inventorySize_h * inventorySize_v;
        }
        //Create UI
        {
            int slotStartY = -60;
            int slotStartX = 60;
            int slotPadding = 60;

            inventory = new Storage(totalSlotCount);

            for (int k = 0; k < inventorySize_v; k++)
            {
                int slotPosY = slotStartY - (k * slotPadding);

                for (int i = 0; i < inventorySize_h; i++)
                {
                    int index = i + (inventorySize_h * k);

                    //Create
                    {
                        int slotPosX = slotStartX + i * slotPadding;
                        inventory.GetItemSlot(index).RectTransform.anchoredPosition = (new Vector2(slotPosX, slotPosY));
                    }
                    //Button
                    {
                        InventorySlotClick script = inventory.GetItemSlot(index).ObjectInScene.AddComponent<InventorySlotClick>();
                        script.slotIndex = index;
                    }
                }
            }
            //Create trashcan
            {
                int slotPosX = slotStartX + (inventorySize_h * slotPadding);
                int slotPosY = slotStartY - ((inventorySize_v-1) * slotPadding);

                itemDropSlot = Instantiate(dropitemSlotPrefab);
                itemDropSlot.transform.SetParent(ResourceManager.Instance.InventoryCanvas.transform, false);
                itemDropSlot.GetComponent<RectTransform>().anchoredPosition = new Vector2(slotPosX, slotPosY);
            }
            //Craete mouse dragging slot
            {
                mouseDraggingSlotInformation = new ItemSlot();
                mouseDraggingSlotInformation.ObjectInScene.GetComponent<Image>().enabled = false;
            }
        }
    }

    private void Start()
    {
        ToggleInventory(false);

        foreach (InventoryItemInformation item in starterItems)
        {
            AddItem(item, 1);
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            ToggleInventory(!isInventoryOpen);
        }

        //Set selected slot with number keys
        for (int i = 0; i < numberKeys.Length; i++)
        {
            if (Input.GetKeyDown(numberKeys[i]))
            {
                SelectItemSlot(i);
                break;
            }
        }

        //Set selected slot with scroll wheel
        {
            if (Input.GetAxis("Mouse ScrollWheel") < 0f) // forward
            {
                selectedSlotIndex++;
                if (selectedSlotIndex >= inventorySize_h)
                    selectedSlotIndex = 0;

                SelectItemSlot(selectedSlotIndex);
        }
            if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
            {
                selectedSlotIndex--;
                if (selectedSlotIndex < 0)
                    selectedSlotIndex = inventorySize_h - 1;

                SelectItemSlot(selectedSlotIndex);
            }
        }

        if (isInventoryOpen)
        {
            Vector2 mouseToWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseDraggingSlotInformation.ObjectTransform.position = (new Vector2(mouseToWorld.x, mouseToWorld.y));
        }
    }

    public void ClickItemSlot_primary(int i)
    {
        if (isInventoryOpen)
        {
            ItemSlot inventorySlot = inventory.GetItemSlot(i);
            InventoryItemInformation inventoryItem = inventorySlot.Item;
            int inventoryCount = inventorySlot.Count;

            //Just put mouse item in inventory slot
            if (inventoryItem == null)
            {
                inventorySlot.SetSlot(mouseDraggingSlotInformation.Item, mouseDraggingSlotInformation.Count);
                mouseDraggingSlotInformation.ClearSlot();
            }
            //Fill slot with dragging item
            else if (inventoryItem.Equals(mouseDraggingSlotInformation.Item))
            {
                FillSlotToCapacity(inventorySlot, inventoryItem, mouseDraggingSlotInformation.Count, out int remains);
                mouseDraggingSlotInformation.SetSlot(inventoryItem, remains);
            }
            //Switch slots items
            else
            {
                inventorySlot.SetSlot(mouseDraggingSlotInformation.Item, mouseDraggingSlotInformation.Count);
                mouseDraggingSlotInformation.SetSlot(inventoryItem, inventoryCount);
            }

            inventorySlot.RefreshUI();
            mouseDraggingSlotInformation.RefreshUI();
        }
    }

    public void ClickItemSlot_secondary(int i)
    {
        if (isInventoryOpen)
        {
            //TODO split
        }
    }

    public void SelectItemSlot(int i)
    {
        //Highlight selected slot, unhighlight previously selected slot
        if (selectedSlot != null)
            selectedSlot.ObjectInScene.GetComponent<Image>().color = Color.white;

        if (i >= totalSlotCount) {
            Debug.LogError("Invalid item slot");
            return;
        }

        selectedSlot = inventory.GetItemSlot(i);
        selectedSlot.ObjectInScene.GetComponent<Image>().color = selectedSlotColor;
        selectedSlotIndex = i;
        PlayerStatesManager.Instance.OnSelectedItemChanged(selectedSlot.Item);
    }

    public void RemoveItem(int itemId, int count)
    {
        //TODO
    }

    public void AddItem(InventoryItemInformation item, int count) {

        OnItemGained?.Invoke(item, count);

        for (int i = 0; i < totalSlotCount; i++)
        {
            ItemSlot slot = inventory.GetItemSlot(i);
            if (slot.Item == null || (item == slot.Item))
            {
                //Add items
                {
                    FillSlotToCapacity(slot, item, count, out count);
                }
                //Refresh info
                {
                    slot.RefreshUI();
                }
            }
        }
        if (count > 0)
        {
            //TOTO do something with remained items
            Debug.Log("Could not add all items");
        }
    }

    private void FillSlotToCapacity(ItemSlot slot, InventoryItemInformation item, int count, out int remains)
    {
        if (slot.Count + count <= slot.capacity)
        {
            slot.SetSlot(item, slot.Count + count);
            remains = 0;
            return;
        }
        else if (slot.Count < slot.capacity)
        {
            count -= slot.capacity - slot.Count;
            slot.SetSlot(item, slot.capacity);
            remains = count;
            return;
        }

        remains = count;
    }

    private void ToggleInventory(bool show)
    {
        isInventoryOpen = show;

        for (int i = 1; i < inventorySize_v; i++) {
            for (int j = 0; j < inventorySize_h; j++) {
                inventory.GetItemSlot(j + (inventorySize_h * i)).Show(show);
            }
        }

        mouseDraggingSlotInformation.Show(show);
        itemDropSlot.SetActive(show);
    }

    public InventoryItemInformation GetSelectedItem()
    {
        if (selectedSlot != null)
            return selectedSlot.Item;
        else
            return null;
    }

    public void DropDraggingItem()
    {
        if (mouseDraggingSlotInformation.Item != null)
            DropItems.DropItem(playerTransform.position, mouseDraggingSlotInformation.Item, mouseDraggingSlotInformation.Count);
    }
}