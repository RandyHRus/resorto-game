using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject dropitemSlotPrefab = null;
    [SerializeField] private InventoryItemInformation[] starterItems = null;
    [SerializeField] private PlayerItemDrops playerItemDrops = null;
    [SerializeField] private Transform inventoryCanvas = null;
    [SerializeField] private Transform inventoryAboveCanvas = null;

    private readonly int slotCapacity = 5;
    private readonly int inventorySize_h = 10;
    private readonly int inventorySize_v = 3;
    private int totalSlotCount;

    bool isInventoryOpen;

    private Storage inventory;
    private ItemSlotUI[] inventorySlotsUI;

    private ItemSlotInformation mouseDraggingSlotInformation = null;
    private ItemSlotUI mouseDraggingSlotUI = null;

    private ItemSlotInformation selectedSlotInformation = null;
    private ItemSlotUI selectedSlotUI = null;
    private Color32 selectedSlotColor = new Color32(189, 189, 189, 255);
    public int SelectedSlotIndex { get; set; } = -1;

    private GameObject itemDropSlot;

    private readonly int slotStartY = -40;
    private readonly int slotStartX = 40;
    private readonly int slotPadding = 40;

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
        //Create inventory as storage
        {
            totalSlotCount = inventorySize_h * inventorySize_v;
            inventory = new Storage(inventorySize_h, inventorySize_v, slotCapacity);
        }
        //Create UI
        {
            inventorySlotsUI = new ItemSlotUI[totalSlotCount];

            for (int k = 0; k < inventorySize_v; k++)
            {
                int slotPosY = slotStartY - (k * slotPadding);

                for (int i = 0; i < inventorySize_h; i++)
                {
                    int index = i + (inventorySize_h * k);

                    ItemSlotUI slotUI = new ItemSlotUI(inventory.GetSlotInformation(index), inventoryCanvas);
                    inventorySlotsUI[index] = slotUI;
                    //Move slot to position
                    {
                        int slotPosX = slotStartX + i * slotPadding;
                        slotUI.RectTransform.anchoredPosition = (new Vector2(slotPosX, slotPosY));
                    }
                    //Button
                    {
                        InventorySlotClick script = slotUI.ObjectInScene.AddComponent<InventorySlotClick>();
                        script.slot = inventory.GetSlotInformation(index);
                    }
                }
            }

            //Create trashcan
            {
                int slotPosX = slotStartX + (inventorySize_h * slotPadding);
                int slotPosY = slotStartY - ((inventorySize_v-1) * slotPadding);

                itemDropSlot = Instantiate(dropitemSlotPrefab);
                itemDropSlot.transform.SetParent(inventoryCanvas, false);
                itemDropSlot.GetComponent<RectTransform>().anchoredPosition = new Vector2(slotPosX, slotPosY);
            }
            //Create mouse dragging slot
            {
                mouseDraggingSlotInformation = new ItemSlotInformation(slotCapacity);
                mouseDraggingSlotUI = new ItemSlotUI(mouseDraggingSlotInformation, inventoryAboveCanvas);
                mouseDraggingSlotUI.ObjectInScene.GetComponent<Image>().enabled = false;
            }
        }
    }

    private void Start()
    {
        ToggleInventory(false);
        GainStartItems();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            ToggleInventory(!isInventoryOpen);
        } 

        if (Input.GetButtonDown("Cancel"))
        {
            ToggleInventory(false);
            SelectItemSlot(-1);
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
                SelectedSlotIndex++;
                if (SelectedSlotIndex >= inventorySize_h)
                    SelectedSlotIndex = 0;

                SelectItemSlot(SelectedSlotIndex);
            }
            if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
            {
                SelectedSlotIndex--;
                if (SelectedSlotIndex < 0)
                    SelectedSlotIndex = inventorySize_h - 1;

                SelectItemSlot(SelectedSlotIndex);
            }

            //TODO REMOVE
            if (Input.GetButtonDown("Debug Items"))
            {
                GainStartItems();
            }
        }

        if (isInventoryOpen)
        {
            Vector2 mouseToWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseDraggingSlotUI.ObjectTransform.position = (new Vector2(mouseToWorld.x, mouseToWorld.y));
        }
    }

    private void GainStartItems()
    {
        foreach (InventoryItemInformation item in starterItems)
        {
            //TODO remove
            AddItem(item, 1);
        }
    }

    public void ClickItemSlot_primary(ItemSlotInformation slot)
    {
        if (isInventoryOpen)
        {
            InventoryItemInformation item = slot.Item;
            int count = slot.Count;

            //Just put mouse item in inventory slot
            if (item == null)
            {
                slot.SetSlot(mouseDraggingSlotInformation.Item, mouseDraggingSlotInformation.Count);
                mouseDraggingSlotInformation.SetSlot(null, 0);
            }
            //Fill slot with dragging item
            else if (item.Equals(mouseDraggingSlotInformation.Item))
            {
                FillSlotToCapacity(slot, item, mouseDraggingSlotInformation.Count, out int remains);
                mouseDraggingSlotInformation.SetSlot(item, remains);
            }
            //Switch slots items
            else
            {
                slot.SetSlot(mouseDraggingSlotInformation.Item, mouseDraggingSlotInformation.Count);
                mouseDraggingSlotInformation.SetSlot(item, count);
            }
        }
    }

    public void ClickItemSlot_secondary(ItemSlotInformation slot)
    {
        if (isInventoryOpen)
        {
            InventoryItemInformation item = slot.Item;
            int count = slot.Count;

            if (item == null || item == mouseDraggingSlotInformation.Item)
            {
                if (mouseDraggingSlotInformation.Item != null)
                {
                    FillSlotToCapacity(slot, mouseDraggingSlotInformation.Item, 1, out int remains);
                    if (remains == 0)
                        mouseDraggingSlotInformation.SetSlot(mouseDraggingSlotInformation.Item, mouseDraggingSlotInformation.Count - 1);
                }
            }
            else if (mouseDraggingSlotInformation.Item == null)
            {
                int half = count / 2;
                int otherHalf = count - half;

                slot.SetSlot(item, half);
                mouseDraggingSlotInformation.SetSlot(item, otherHalf);
            }
        }
    }

    //Select -1 to unselect slot
    public void SelectItemSlot(int i)
    {
        //unhighlight previously selected slot
        if (selectedSlotInformation != null)
        {
            selectedSlotUI.ObjectInScene.GetComponent<Image>().color = Color.white;
            selectedSlotInformation.OnItemChanged -= OnSelectedItemChanged;
        }

        //Unselect slots
        if (i == -1)
        {
            return;
        }

        if (i >= totalSlotCount || i < 0) {
            Debug.LogError("Invalid item slot");
            return;
        }

        selectedSlotInformation = inventory.GetSlotInformation(i);
        selectedSlotUI = inventorySlotsUI[i];
        selectedSlotUI.ObjectInScene.GetComponent<Image>().color = selectedSlotColor; //Highlight selected slot,
        SelectedSlotIndex = i;
        selectedSlotInformation.OnItemChanged += OnSelectedItemChanged;
        OnSelectedItemChanged();
    }

    public void OnSelectedItemChanged()
    {
        PlayerStateMachine.Instance.OnSelectedItemChanged(selectedSlotInformation.Item);
    }

    public void RemoveItem(int slotIndex, int count)
    {
        ItemSlotInformation slot = inventory.GetSlotInformation(SelectedSlotIndex);
        if (slot.Count - count < 0)
            Debug.Log("Tried to remove too many items");
        slot.SetSlot(slot.Item, slot.Count - count);
    }

    public void RemoveItem(InventoryItemInformation item, int count)
    {
        //TODO: Alternative remove method

    }

    public void AddItem(InventoryItemInformation item, int count) {

        OnItemGained?.Invoke(item, count);

        for (int i = 0; i < totalSlotCount; i++)
        {
            ItemSlotInformation slot = inventory.GetSlotInformation(i);
            if (slot.Item == null || (item == slot.Item))
            {
                //Add items
                FillSlotToCapacity(slot, item, count, out count);
            }
        }
        if (count > 0)
        {
            Debug.Log("Could not add all items");
            playerItemDrops.DropItem(item, count);
        }
    }

    private void FillSlotToCapacity(ItemSlotInformation slot, InventoryItemInformation item, int toAdd, out int remains)
    {
        if (slot.Count + toAdd <= slot.Capacity)
        {
            slot.SetSlot(item, slot.Count + toAdd);
            remains = 0;
            return;
        }
        else if (slot.Count < slot.Capacity)
        {
            toAdd -= slot.Capacity - slot.Count;
            slot.SetSlot(item, slot.Capacity);
            remains = toAdd;
            return;
        }

        remains = toAdd;
    }

    public void ToggleInventory(bool show)
    {
        isInventoryOpen = show;

        for (int i = 1; i < inventorySize_v; i++) {
            for (int j = 0; j < inventorySize_h; j++) {
                inventorySlotsUI[j + (inventorySize_h * i)].Show(show);
            }
        }

        mouseDraggingSlotUI.Show(show);
        itemDropSlot.SetActive(show);
    }

    public void DropDraggingItem()
    {
        if (mouseDraggingSlotInformation.Item != null)
        {
            playerItemDrops.DropItem(mouseDraggingSlotInformation.Item, mouseDraggingSlotInformation.Count);
            mouseDraggingSlotInformation.SetSlot(null, 0);
        }
    }
}