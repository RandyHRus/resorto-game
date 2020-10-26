using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private CharacterCustomizationLoader customizationLoader = null;

    [SerializeField] private InventoryItemInformation[] starterItems = null;

    [SerializeField] private Transform inventoryCanvas = null;
    [SerializeField] private Transform inventoryAboveCanvas = null;
    [SerializeField] private Transform playerTransform = null;

    [SerializeField] private Sprite purpleSlot = null;
    [SerializeField] private Sprite greenSlot = null;

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
    private CosmeticInventorySlot<CharacterHatItemInformation> hatSlot;
    private CosmeticInventorySlot<CharacterShirtItemInformation> shirtSlot;
    private CosmeticInventorySlot<CharacterPantsItemInformation> pantsSlot;

    private List<Tuple<InventorySlotUI, bool>> uiToClose = new List<Tuple<InventorySlotUI, bool>>();

    private ItemInformationDisplayUI itemInformationDisplayUI;

    private readonly int slotStartY = -40;
    private readonly int slotStartX = 40;
    private readonly int slotPadding = 44;

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

    public delegate void ItemGained(InventoryItemInstance item, int count);
    public static event ItemGained OnItemGained;

    public delegate void InventoryClosed();
    public static event InventoryClosed OnInventoryClosed;

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

        Sidebar.OnSidebarOpened += ShowInventory;
    }

    private void Start()
    {
        //Create inventory as storage
        {
            totalSlotCount = inventorySize_h * inventorySize_v;
            inventory = new Storage(inventorySize_h, inventorySize_v);
        }
        //Create UI
        {
            //Create slots
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
                        slotUI.RectTransform.anchoredPosition = new Vector2(slotPosX, slotPosY);
                    }

                    if (k >= 1)
                        uiToClose.Add(Tuple.Create((InventorySlotUI)slotUI, true));
                    else
                        uiToClose.Add(Tuple.Create((InventorySlotUI)slotUI, false));
                }
            }
            //Create cosmetic slots
            hatSlot = new CosmeticInventorySlot<CharacterHatItemInformation>();
            ItemSlotUI hatSlotUI = new ItemSlotUI(hatSlot, inventoryCanvas, purpleSlot);
            hatSlotUI.RectTransform.anchoredPosition = new Vector2(slotStartX + (slotPadding * 0), slotStartY - (inventorySize_v * slotPadding));
            uiToClose.Add(Tuple.Create((InventorySlotUI)hatSlotUI, true));
            hatSlot.ItemChanged += () => customizationLoader.LoadHatInstance((HatItemInstance)hatSlot.Item);

            hatSlot.SetSlot(PlayerCustomization.Character.Hat);

            shirtSlot = new CosmeticInventorySlot<CharacterShirtItemInformation>();
            ItemSlotUI shirtSlotUI = new ItemSlotUI(shirtSlot, inventoryCanvas, greenSlot);
            shirtSlotUI.RectTransform.anchoredPosition = new Vector2(slotStartX +(slotPadding * 1), slotStartY - (inventorySize_v * slotPadding));
            uiToClose.Add(Tuple.Create((InventorySlotUI)shirtSlotUI, true));
            shirtSlot.ItemChanged += () => customizationLoader.LoadShirtInstance((ShirtItemInstance)shirtSlot.Item);

            shirtSlot.SetSlot(PlayerCustomization.Character.Shirt);

            pantsSlot = new CosmeticInventorySlot<CharacterPantsItemInformation>();
            ItemSlotUI pantsSlotUI = new ItemSlotUI(pantsSlot, inventoryCanvas, greenSlot);
            pantsSlotUI.RectTransform.anchoredPosition = new Vector2(slotStartX + (slotPadding * 2), slotStartY - (inventorySize_v * slotPadding));
            uiToClose.Add(Tuple.Create((InventorySlotUI)pantsSlotUI, true));
            pantsSlot.ItemChanged += () => customizationLoader.LoadPantsInstance((PantsItemInstance)pantsSlot.Item); ;

            pantsSlot.SetSlot(PlayerCustomization.Character.Pants);

            //Create trashcan
            {
                int slotPosX = slotStartX + (inventorySize_h * slotPadding);
                int slotPosY = slotStartY - ((inventorySize_v - 1) * slotPadding);

                DropItemInventorySlot itemDropSlot = new DropItemInventorySlot();
                DropItemInventorySlotUI itemDropSlotUI = new DropItemInventorySlotUI(itemDropSlot, inventoryCanvas);
                itemDropSlotUI.RectTransform.anchoredPosition = new Vector2(slotPosX, slotPosY);
                uiToClose.Add(Tuple.Create((InventorySlotUI)itemDropSlotUI, true));
            }
            //Create mouse dragging slot
            {
                MouseDraggingSlot = new DraggingItemInventorySlot();
                draggingSlotUI = new DraggingItemSlotUI(MouseDraggingSlot, inventoryAboveCanvas);
                draggingSlotUI.ObjectInScene.GetComponent<Image>().enabled = false;
                uiToClose.Add(Tuple.Create((InventorySlotUI)draggingSlotUI, true));
            }
            //Item information display
            {
                itemInformationDisplayUI = new ItemInformationDisplayUI();
                itemInformationDisplayUI.Show(false);
            }
        }

        TryHideInventory();
        GainStartItems();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
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
                int proposedSlotIndex = selectedSlotIndex + 1;
                if (proposedSlotIndex >= inventorySize_h)
                    proposedSlotIndex = 0;

                SelectItemSlot(proposedSlotIndex);
            }
            if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
            {
                int proposedSlotIndex = selectedSlotIndex - 1;
                if (proposedSlotIndex < 0)
                    proposedSlotIndex = inventorySize_h - 1;

                SelectItemSlot(proposedSlotIndex);
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
            //TODO remove, we dont want starter items maybe
            InventoryItemInstance iteminstance = new InventoryItemInstance(item);
            AddItem(iteminstance, 1);
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

        selectedSlotIndex = i;
        OnSelectedItemChanged();

        if (i == -1)
        {
            return;
        }

        if (i >= totalSlotCount || i < 0)
        {
            throw new System.Exception("Invalid item slot");
        }

        StorageItemInventorySlot slotInfo = inventory.GetStorageSlotInformation(i);
        inventorySlotToUI[slotInfo].StartEnlarge();
        slotInfo.ItemChanged += OnSelectedItemChanged;
    }

    public void OnSelectedItemChanged()
    {
        if (SelectedSlot == null)
            return;

        if (SelectedSlot.Item == null)
            PlayerStateMachineManager.Instance.SwitchState<DefaultState>();
        else
            SelectedSlot.Item.ItemInformation.ItemSelected();
    }

    public void AddItem(InventoryItemInstance item, int count) {

        OnItemGained?.Invoke(item, count);
        for (int i = 0; i < totalSlotCount; i++)
        {
            if (count <= 0)
            {
                break;
            }
            StorageItemInventorySlot slot = inventory.GetStorageSlotInformation(i);
            if (slot.Item == null || (item.Equals(slot.Item)))
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

    public void ShowInventory()
    {
        IsInventoryOpen = true;

        if (SelectedSlot != null)
            inventorySlotToUI[SelectedSlot].StartShrink();

        SelectItemSlot(-1);

        foreach (Tuple<InventorySlotUI, bool> pair in uiToClose)
        {
            if (pair.Item2)
                pair.Item1.Show(true);
        }
    }

    public void TryHideInventory()
    {
        if (Sidebar.Instance.CurrentPanel != null)
            return;

        IsInventoryOpen = false;

        foreach (Tuple<InventorySlotUI, bool> pair in uiToClose)
        {
            pair.Item1.StartShrink();
        }

        foreach (Tuple<InventorySlotUI, bool> pair in uiToClose)
        {
            if (pair.Item2)
                pair.Item1.Show(false);
        }

        itemInformationDisplayUI.Show(false);

        OnInventoryClosed?.Invoke();
    }

    public void DropDraggingItem()
    {
        if (MouseDraggingSlot.Item != null)
        {
            DropItem(MouseDraggingSlot.Item, MouseDraggingSlot.Count);
            MouseDraggingSlot.SetSlot(null, 0);
        }
    }

    private void DropItem(InventoryItemInstance item, int count)
    {
        float xSpeed = PlayerDirection.Instance.VisualDirection.DirectionVector.x * UnityEngine.Random.Range(minDropXSpeed, maxDropXSpeed);
        DropItems.DropItem(playerTransform.position, itemDropHeight, item, count, xSpeed);
    }

    public void DisplaySlotInformation(InventorySlotUI slotUI)
    {
        if (slotUI.Slot is ItemInventorySlot itemSlot && itemSlot.Item != null)
        {
            itemInformationDisplayUI.Show(true);
            itemInformationDisplayUI.SetItem(itemSlot.Item);
            itemInformationDisplayUI.ObjectTransform.position = slotUI.ObjectTransform.position + new Vector3(1.5f, -0.5f);
            MouseUIInformationDisplayManager.SetShownUI(itemInformationDisplayUI);
        }
    }

    public void HideSlotInformation(InventorySlotUI slotUI)
    {
        itemInformationDisplayUI.Show(false);
    }
}