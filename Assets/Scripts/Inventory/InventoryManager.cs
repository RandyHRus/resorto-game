using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    private int slotCapacity = 5;
    [SerializeField] private GameObject canvas = null;
    [SerializeField] private GameObject inventorySlotPrefab = null;
    [SerializeField] private GameObject draggingSlotPrefab = null;
    private int inventorySize_h = 10;
    private int inventorySize_v = 3;
    private int totalSlotCount;

    bool isInventoryOpen;

    private InventorySlotInformation[] inventorySlots;
    private InventorySlotInformation mouseDraggingSlotInformation = null;
    private Transform mouseDraggingSlotTransform;

    private Color32 selectedSlotColor = new Color32(189, 189, 189, 255);

    private int selectedSlotIndex = -1;
    private InventorySlotInformation selectedSlot = null;

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

    private class InventorySlotInformation
    {
        public InventoryItem item { get; private set; }
        public int count { get; private set; }
        public Image iconImage;
        public Text itemCountText;
        public GameObject slotObject;


        public void SetSlot(InventoryItem item, int count)
        {
            this.item = item;
            this.count = count;

            if (count <= 0)
            {
                this.item = null;
                this.count = 0;
            }
        }

        public void ClearSlot()
        {
            item = null;
            count = 0;
        }

        public InventorySlotInformation(GameObject slotObject) {
            item = null;
            count = 0;
            this.slotObject = slotObject;

            {
                //Find child components
                Transform t = slotObject.transform;
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
            }

            iconImage.enabled = false; //To remove white space
            itemCountText.enabled = false;
        }
    }

    public delegate void ItemGained(InventoryItem item, int count);
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

            inventorySlots = new InventorySlotInformation[totalSlotCount];

            for (int k = 0; k < inventorySize_v; k++)
            {
                int slotPosY = slotStartY - (k * slotPadding);

                for (int i = 0; i < inventorySize_h; i++)
                {
                    GameObject obj = Instantiate(inventorySlotPrefab);
                    int index = i + (inventorySize_h * k);

                    //Set parent and transform position
                    {
                        obj.transform.SetParent(canvas.transform, false);

                        int slotPosX = slotStartX + i * slotPadding;
                        obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(slotPosX, slotPosY);
                    }
                    //Create and set up slot information
                    {
                        inventorySlots[index] = new InventorySlotInformation(obj);
                    }
                    //Button
                    {
                        InventorySlotClick script = obj.AddComponent<InventorySlotClick>();
                        script.slotIndex = index;
                    }
                }
            }
        }
        //Craete mouse dragging slot
        {
            GameObject obj = Instantiate(draggingSlotPrefab);

            obj.transform.SetParent(canvas.transform, false);

            mouseDraggingSlotInformation = new InventorySlotInformation(obj);
            mouseDraggingSlotTransform = obj.transform;
        }
    }

    private void Start()
    {
        ToggleInventory(false);

        //TODO remove
        AddItem(new ToolItem(0), 1);
        AddItem(new ToolItem(1), 1);
        AddItem(new ToolItem(2), 1);
        AddItem(new ObjectItem(0), 3);
        AddItem(new ObjectItem(1), 3);
        AddItem(new ObjectItem(2), 3);
        AddItem(new ObjectItem(3), 3);
        AddItem(new ObjectItem(4), 3);
        AddItem(new ObjectItem(5), 3);
        AddItem(new ObjectItem(6), 3);
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
            mouseDraggingSlotTransform.position = new Vector2(mouseToWorld.x, mouseToWorld.y);
        }
    }

    public void ClickItemSlot_primary(int i)
    {
        if (isInventoryOpen)
        {
            InventoryItem inventoryItem = inventorySlots[i].item;
            int inventoryCount = inventorySlots[i].count;

            //Just put mouse item in inventory slot
            if (inventoryItem == null)
            {
                inventorySlots[i].SetSlot(mouseDraggingSlotInformation.item, mouseDraggingSlotInformation.count);
                mouseDraggingSlotInformation.ClearSlot();
            }
            //Fill slot with dragging item
            else if (inventoryItem.Equals(mouseDraggingSlotInformation.item))
            {
                FillSlotToCapacity(inventorySlots[i], inventoryItem, mouseDraggingSlotInformation.count, out int remains);
                mouseDraggingSlotInformation.SetSlot(inventoryItem, remains);
            }
            //Switch slots items
            else
            {
                inventorySlots[i].SetSlot(mouseDraggingSlotInformation.item, mouseDraggingSlotInformation.count);
                mouseDraggingSlotInformation.SetSlot(inventoryItem, inventoryCount);
            }

            RefreshSlotUI(inventorySlots[i]);
            RefreshSlotUI(mouseDraggingSlotInformation);
        }
    }

    public void ClickItemSlot_secondary(int i)
    {
        if (isInventoryOpen)
        {
            //TODO split
        }
    }

    private void RefreshSlotUI(InventorySlotInformation info)
    {
        if (info.item == null)
        {
            info.iconImage.sprite = null;
            info.iconImage.enabled = false;

            info.itemCountText.text = "";
            info.itemCountText.enabled = false;
        }
        else
        {
            info.iconImage.sprite = info.item.itemIcon;
            info.iconImage.enabled = true;

            info.itemCountText.text = info.count.ToString();
            info.itemCountText.enabled = true;
        }
    }

    public void SelectItemSlot(int i)
    {
        //Highlight selected slot, unhighlight previously selected slot
        if (selectedSlot != null)
            selectedSlot.slotObject.GetComponent<Image>().color = Color.white;
        inventorySlots[i].slotObject.GetComponent<Image>().color = selectedSlotColor;

        if (i >= totalSlotCount) {
            Debug.Log("Invalid item slot");
            return;
        }

        selectedSlot = inventorySlots[i];
        selectedSlotIndex = i;
        PlayerStatesManager.Instance.OnSelectedItemChanged(selectedSlot.item);
    }

    public void RemoveItem(int itemId, int count)
    {
        //TODO
    }

    public void AddItem(InventoryItem item, int count) {

        OnItemGained?.Invoke(item, count);

        foreach (InventorySlotInformation slotInfo in inventorySlots)
        {
            if (slotInfo.item == null || (item.id == slotInfo.item.id && item.GetType() == slotInfo.item.GetType()))
            {
                //Add items
                {
                    FillSlotToCapacity(slotInfo, item, count, out count);
                }
                //Refresh info
                {
                    RefreshSlotUI(slotInfo);
                }
            }
        }
        if (count > 0)
        {
            //TOTO do something with remained items
            Debug.Log("Could not add all items");
        }
    }

    private void FillSlotToCapacity(InventorySlotInformation slotInfo, InventoryItem item, int count, out int remains)
    {
        if (slotInfo.count + count <= slotCapacity)
        {
            slotInfo.SetSlot(item, slotInfo.count + count);
            remains = 0;
            return;
        }
        else if (slotInfo.count < slotCapacity)
        {
            count -= slotCapacity - slotInfo.count;
            slotInfo.SetSlot(item, slotCapacity);
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
                inventorySlots[j + (inventorySize_h * i)].slotObject.SetActive(show);
            }
        }

        mouseDraggingSlotInformation.slotObject.SetActive(show);
    }

    public InventoryItem GetSelectedItem()
    {
        if (selectedSlot != null)
            return selectedSlot.item;
        else
            return null;
    }
}

#region Inventory items

public interface InventoryItem: System.IEquatable<InventoryItem>
{
    int id { get; }
    Sprite itemIcon { get; }
    string itemName { get; }
    IPlayerState onSelectStateInstance { get; }
}

public class ObjectItem: InventoryItem
{
    private int id_;
    public int id {
        get {
            return id_;
        }
        private set {
            id_ = value;
        }
    }
    private Sprite itemIcon_;
    public Sprite itemIcon {
        get {
            return itemIcon_;
        }
        private set {
            itemIcon_ = value;
        }
    }
    private string itemName_;
    public string itemName {
        get {
            return itemName_;
        }
        private set {
            itemName_ = value;
        }
    }

    public IPlayerState onSelectStateInstance {
        get {
            return CreateObjectsState.Instance;
        }
    }

    public ObjectItem(int id)
    {
        this.id = id;
        if (ObjectInformationManager.Instance.objectInformationMap.TryGetValue(id, out ObjectInformation info))
        {
            itemIcon = info.iconSprite;
            itemName = info.objectName;
        } 
        else
        {
            Debug.Log("Could not find object with id: " + id);
        }
    }

    public override bool Equals(object obj)
    {
        return this.Equals(obj as InventoryItem);
    }

    public bool Equals(InventoryItem item)
    {
        if (Object.ReferenceEquals(item, null))
        {
            return false;
        }

        if (Object.ReferenceEquals(this, item))
        {
            return true;
        }

        if (this.GetType() != item.GetType())
        {
            return false;
        }

        return (id == item.id);
    }

    public override int GetHashCode()
    {
        return 0001000000 + id;
    }
}

public class ToolItem: InventoryItem
{
    private int id_;
    public int id {
        get {
            return id_;
        }
        private set {
            id_ = value;
        }
    }
    private Sprite itemIcon_;
    public Sprite itemIcon {
        get {
            return itemIcon_;
        }
        private set {
            itemIcon_ = value;
        }
    }
    private string itemName_;
    public string itemName {
        get {
            return itemName_;
        }
        private set {
            itemName_ = value;
        }
    }

    private IPlayerState onSelectStateInstance_;
    public IPlayerState onSelectStateInstance {
        get {
            return onSelectStateInstance_;
        }
    }

    public ToolItem(int id)
    {
        this.id = id;
        if (ToolInformationManager.Instance.informationMap.TryGetValue(id, out ToolInformation info))
        {
            itemIcon = info.icon;
            itemName = info.name;

            onSelectStateInstance_ = ToolInformationManager.Instance.GetToolState(info.toolState);
        }
        else
        {
            Debug.Log("Could not find tool with id: " + id);
        }
    }

    public override bool Equals(object obj)
    {
        return this.Equals(obj as InventoryItem);
    }

    public bool Equals(InventoryItem item)
    {
        if (Object.ReferenceEquals(item, null))
        {
            return false;
        }

        if (Object.ReferenceEquals(this, item))
        {
            return true;
        }

        if (this.GetType() != item.GetType())
        {
            return false;
        }

        return (id == item.id);
    }

    public override int GetHashCode()
    {
        return 0002000000 + id;
    }
}

#endregion