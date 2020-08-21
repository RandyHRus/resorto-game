using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private InventoryItemInformation[] starterItems = null;
    [SerializeField] private Transform inventoryCanvas = null;
    [SerializeField] private Transform inventoryAboveCanvas = null;
    [SerializeField] private Transform playerTransform = null;

    private readonly int inventorySize_h = 10;
    private readonly int inventorySize_v = 3;
    private int totalSlotCount;

    private readonly float itemDropHeight = 0.5f;

    public bool IsInventoryOpen { get; private set; }

    private Storage inventory;
    public DraggingItemInventorySlot MouseDraggingSlot { get; private set; } = null;

    private int selectedSlotIndex = -1;
    public StorageItemInventorySlot SelectedSlot => inventory.GetStorageSlotInformation(selectedSlotIndex);

    private Dictionary<StorageItemInventorySlot, ItemSlotUI> inventorySlotToUI = new Dictionary<StorageItemInventorySlot, ItemSlotUI>();
    private ItemSlotUI draggingSlotUI;

    private DropItemInventorySlot itemDropSlot;
    private DropItemInventorySlotUI itemDropSlotUI;

    private readonly int slotStartY = -40;
    private readonly int slotStartX = 40;
    private readonly int slotPadding = 36;

    private float minDropXSpeed = 0.8f;
    private float maxDropXSpeed = 1.2f;

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
            inventory = new Storage(inventorySize_h, inventorySize_v);
        }
        //Create UI
        {
            for (int k = 0; k < inventorySize_v; k++)
            {
                int slotPosY = slotStartY - (k * slotPadding);

                for (int i = 0; i < inventorySize_h; i++)
                {
                    int index = i + (inventorySize_h * k);

                    StorageItemInventorySlot slot = inventory.GetStorageSlotInformation(index);
                    ItemSlotUI slotUI = new ItemSlotUI(slot, inventoryCanvas);
                    inventorySlotToUI.Add(slot, slotUI);
                    //Move slot to position
                    {
                        int slotPosX = slotStartX + i * slotPadding;
                        slotUI.RectTransform.anchoredPosition = (new Vector2(slotPosX, slotPosY));
                    }
                }
            }

            //Create trashcan
            {
                int slotPosX = slotStartX + (inventorySize_h * slotPadding);
                int slotPosY = slotStartY - ((inventorySize_v-1) * slotPadding);

                itemDropSlot = new DropItemInventorySlot();
                itemDropSlotUI = new DropItemInventorySlotUI(itemDropSlot, inventoryCanvas);
                itemDropSlotUI.RectTransform.anchoredPosition = new Vector2(slotPosX, slotPosY);
            }
            //Create mouse dragging slot
            {
                MouseDraggingSlot = new DraggingItemInventorySlot();
                draggingSlotUI = new DraggingItemSlotUI(MouseDraggingSlot, inventoryAboveCanvas);
                draggingSlotUI.ObjectInScene.GetComponent<Image>().enabled = false;
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
        /*
        if (Input.GetButtonDown("Cancel"))
        {
            ToggleInventory(false);
            SelectItemSlot(-1);
        }
        */

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

            //TODO REMOVE
            if (Input.GetButtonDown("Debug Items"))
            {
                GainStartItems();
            }
        }

        //Move dragging slot
        if (IsInventoryOpen)
        {
            Vector2 mouseToWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            draggingSlotUI.ObjectTransform.position = (new Vector2(mouseToWorld.x, mouseToWorld.y));
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

    //Select -1 to unselect slot
    public void SelectItemSlot(int i)
    {
        //unhighlight previously selected slot
        if (selectedSlotIndex != -1)
        {
            inventorySlotToUI[SelectedSlot].StartShrink();
            ((StorageItemInventorySlot)inventorySlotToUI[SelectedSlot].Slot).ItemChanged -= OnSelectedItemChanged;
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

        StorageItemInventorySlot slotInfo = inventory.GetStorageSlotInformation(i);
        inventorySlotToUI[slotInfo].StartEnlarge();
        selectedSlotIndex = i;
        slotInfo.ItemChanged += OnSelectedItemChanged;
        OnSelectedItemChanged();
    }

    public void OnSelectedItemChanged()
    {
        PlayerStateMachine.Instance.OnSelectedItemChanged(inventory.GetStorageSlotInformation(selectedSlotIndex).Item);
    }

    public void RemoveItem(InventoryItemInformation item, int count)
    {
        //TODO: Alternative remove method

    }

    public void AddItem(InventoryItemInformation item, int count) {

        OnItemGained?.Invoke(item, count);

        for (int i = 0; i < totalSlotCount; i++)
        {
            StorageItemInventorySlot slot = inventory.GetStorageSlotInformation(i);
            if (slot.Item == null || (item == slot.Item))
            {
                //Add items
                slot.FillSlotToCapacity(item, count, out count);
            }
        }
        if (count > 0)
        {
            Debug.Log("Could not add all items");
            DropItem(item, count);
        }
    }

    public void ToggleInventory(bool show)
    {
        IsInventoryOpen = show;

        if (show) {
            if (SelectedSlot != null)
                inventorySlotToUI[SelectedSlot].StartShrink();

            selectedSlotIndex = -1;
        }
        else
        {
            //Shrink previously hoverred over slot
            for (int i = 1; i < inventorySize_v; i++)
            {
                for (int j = 0; j < inventorySize_h; j++)
                {
                    inventorySlotToUI[inventory.GetStorageSlotInformation(j + (inventorySize_h * i))].StartShrink();
                }
            }
        }


        for (int i = 1; i < inventorySize_v; i++) {
            for (int j = 0; j < inventorySize_h; j++) {
                inventorySlotToUI[inventory.GetStorageSlotInformation(j + (inventorySize_h * i))].Show(show);
            }
        }

        //slotToUI[SelectedSlot].Show(show);

        itemDropSlotUI.Show(show);
    }

    public void DropDraggingItem()
    {
        if (MouseDraggingSlot.Item != null)
        {
            DropItem(MouseDraggingSlot.Item, MouseDraggingSlot.Count);
            MouseDraggingSlot.SetSlot(null, 0);
        }
    }

    private void DropItem(InventoryItemInformation item, int count)
    {
        float xSpeed = PlayerDirection.Instance.VisualDirection.DirectionVector.x * Random.Range(minDropXSpeed, maxDropXSpeed);
        DropItems.DropItem(playerTransform.position, itemDropHeight, item, count, xSpeed);
    }
}