using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemDrops : MonoBehaviour
{
    private static readonly float pickupNearbyItemsScanRadius = 1.5f;
    private static readonly float distanceToUnfreshItem = 2.5f;

    Dictionary<Vector2Int, HashSet<DroppedItem>> locationToFreshDropItemList = new Dictionary<Vector2Int, HashSet<DroppedItem>>();
    Transform _transform;

    private void Awake()
    {
        PlayerMovement.PlayerMoved += () => PickUpNearby();
        PlayerMovement.PlayerMoved += () => CheckFreshItems();
        _transform = transform;
    }

    public void PickUpNearby()
    {
        Collider2D[] results = Physics2D.OverlapCircleAll(transform.position, pickupNearbyItemsScanRadius, 1 << LayerMask.NameToLayer("DropItems"));
        foreach (Collider2D result in results)
        {
            DroppedItem item = result.GetComponent<DroppedItem>();
            if (!item.IsFresh)
            {
                InventoryManager.Instance.AddItem(item.ItemInfo, item.Count);
                Destroy(result.gameObject);
            }
        }
    }

    private void CheckFreshItems()
    {
        List<Vector2Int> keysToRemove = new List<Vector2Int>();
        foreach (KeyValuePair<Vector2Int, HashSet<DroppedItem>> pair in locationToFreshDropItemList)
        {
            if (Vector2.Distance(pair.Key, _transform.position) >= distanceToUnfreshItem)
            {
                foreach (DroppedItem item in pair.Value)
                {
                    item.UnfreshItem();
                }
                keysToRemove.Add(pair.Key);
            }
        }

        foreach (Vector2Int key in keysToRemove)
        {
            locationToFreshDropItemList.Remove(key);
        }
    }


    public void DropItem(InventoryItemInformation item, int count)
    {
        DroppedItem newDroppedItem = DropItems.DropItem(_transform.position, item, count, false);
        Vector2Int tilePos = new Vector2Int(Mathf.RoundToInt(_transform.position.x), Mathf.RoundToInt(_transform.position.y));
        if (locationToFreshDropItemList.TryGetValue(tilePos, out HashSet<DroppedItem> set))
        {
            set.Add(newDroppedItem);
        }
        else
        {
            HashSet<DroppedItem> newSet = new HashSet<DroppedItem>();
            newSet.Add(newDroppedItem);
            locationToFreshDropItemList[tilePos] = newSet;
        }
    }
}
