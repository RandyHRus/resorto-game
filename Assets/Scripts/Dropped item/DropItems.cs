using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItems
{
    private static readonly float nearbyItemsScanRadius = 3f;

    public static void DropItem(Vector2 position, InventoryItemInformation itemInfo, int count)
    {
        //Try to group same items
        Collider2D[] results = Physics2D.OverlapCircleAll(position, nearbyItemsScanRadius, 1 << LayerMask.NameToLayer("DropItems"));

        bool foundSameItem = false;
        foreach (Collider2D col in results)
        {
            DroppedItem groundItem = col.GetComponent<DroppedItem>();
            if (groundItem.ItemInfo == itemInfo)
            {
                foundSameItem = true;
                groundItem.AddToStack(count);
            }
        }

        if (!foundSameItem)
        {
            GameObject droppedItemPrefab = ResourceManager.Instance.DroppedItem;
            GameObject dropItem = GameObject.Instantiate(droppedItemPrefab, position, Quaternion.identity);
            dropItem.GetComponent<DroppedItem>().Initialize(itemInfo, count);
        }

    }
}