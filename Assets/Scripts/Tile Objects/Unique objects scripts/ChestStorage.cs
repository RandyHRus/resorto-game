using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestStorage : MonoBehaviour, ITileObjectFunctions
{
    private int capacity = 5;
    private Storage storage;
    private StorageUI ui;

    [SerializeField] private int numOfSlotsX = 5;
    [SerializeField] private int numOfSlotsY = 5;

    private void Start()
    {
        storage = new Storage(numOfSlotsX, numOfSlotsY, capacity);
    }

    public void ClickInteract()
    {
        OpenUI();
    }

    public void Initialize(ObjectOnTile objectData)
    {
        
    }

    public void StepOff()
    {
        
    }

    public void StepOn()
    {
       
    }

    private void OpenUI()
    {
        if (ui != null)
            return;

        ui = new StorageUI(storage);
        ui.OnDestroy += OnUIDestroy;

        UIPanelsManager.Instance.SetCurrentPanel(ui, true);
        InventoryManager.Instance.ToggleInventory(true);
    }

    private void OnUIDestroy()
    {
        ui.OnDestroy -= OnUIDestroy;
        ui = null;
    }
}
