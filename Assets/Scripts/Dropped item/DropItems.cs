using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItems
{
    private static readonly float dropNearbyItemsScanRadius = 1f;

    public static DroppedItem DropItem(Vector2 position, InventoryItemInformation itemInfo, int count, bool pickupableInstantly)
    {
        //Try to group same items
        Collider2D[] results = Physics2D.OverlapCircleAll(position, dropNearbyItemsScanRadius, 1 << LayerMask.NameToLayer("DropItems"));

        foreach (Collider2D col in results)
        {
            DroppedItem groundItem = col.GetComponent<DroppedItem>();
            if (groundItem.ItemInfo == itemInfo)
            {
                groundItem.AddToStack(count);
                return groundItem;
            }
        }

        //If no same item found
        {
            GameObject droppedItemPrefab = ResourceManager.Instance.DroppedItem;
            GameObject obj = GameObject.Instantiate(droppedItemPrefab, position, Quaternion.identity);
            DroppedItem droppedItem = obj.GetComponent<DroppedItem>();
            droppedItem.Initialize(itemInfo, count, pickupableInstantly);
            return droppedItem;
        }
    }
}