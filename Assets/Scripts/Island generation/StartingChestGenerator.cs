using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class StartingChestGenerator : MonoBehaviour
{
    [SerializeField] private ObjectInformation chestObject = null;

    [SerializeField] private InventoryItemAssetToCount[] chestContent = null;

    [System.Serializable]
    public class InventoryItemAssetToCount
    {
        [SerializeField] private AssetReference item = null;
        public AssetReference Item => item;

        [SerializeField] private int count = 0;
        public int Count => count;
    }

    private static StartingChestGenerator _instance;
    public static StartingChestGenerator Instance { get { return _instance; } }
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
    }

    public void CreateStartingChest(IslandStartingPosition position)
    {
        if (!TileObjectsManager.Instance.TryCreateObject(chestObject, position.StartingChestPosition, out BuildOnTile buildOnTile))
        {
            throw new IslandGenerationException("Failed to create starting chest.");
        }

        ChestStorage chestStorage = buildOnTile.GameObjectOnTile.GetComponent<ChestStorage>();

        foreach (InventoryItemAssetToCount i in chestContent)
        {
            chestStorage.InsertItemInRandomEmptySlot(new InventoryItemInstance(i.Item), i.Count);
        }
    }
}
