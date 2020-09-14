using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestStorage : MonoBehaviour, ITileObjectFunctions
{
    private Storage storage;
    private StorageUI ui;

    [SerializeField] private int numOfSlotsX = 5;
    [SerializeField] private int numOfSlotsY = 5;

    private void Awake()
    {
        storage = new Storage(numOfSlotsX, numOfSlotsY);
    }

    public void ClickInteract()
    {
        OpenUI();
    }

    public void Initialize(BuildOnTile objectData)
    {
        
    }

    public void StepOff()
    {
        
    }

    public void StepOn()
    {
       
    }

    public void InsertItemInRandomEmptySlot(InventoryItemInstance item, int count)
    {
        List<StorageItemInventorySlot> emptySlots = new List<StorageItemInventorySlot>();

        for (int i = 0; i < storage.SlotCount; i++)
        {
            StorageItemInventorySlot slot = storage.GetStorageSlotInformation(i);
            if (slot.Item == null)
                emptySlots.Add(slot);
        }

        if (emptySlots.Count <= 0)
        {
            throw new System.Exception("No empty slots!");
        }

        StorageItemInventorySlot randomSlot = emptySlots[Random.Range(0, emptySlots.Count)];
        randomSlot.SetSlot(item, count);
    }

    private void OpenUI()
    {
        if (InventoryManager.Instance.IsInventoryOpen)
            return;

        ui = new StorageUI(storage);
        PlayerStateMachine.Instance.TrySwitchState<InventoryState>(new object[] { ui });
    }
}
