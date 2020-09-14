using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItems
{
    //private static readonly float dropNearbyItemsScanRadius = 1f;

    public static DroppedItem DropItem(Vector2 positionOnGround, float dropHeight, InventoryItemInstance itemInstance, int count, float xSpeed)
    {
        /*
        //Try to group same items
        Collider2D[] results = Physics2D.OverlapCircleAll(positionOnGround, dropNearbyItemsScanRadius, 1 << LayerMask.NameToLayer("DropItems"));

        foreach (Collider2D col in results)
        {
            DroppedItem groundItem = col.GetComponent<DroppedItem>();
            if (groundItem.ItemInfo == itemInfo)
            {
                groundItem.AddToStack(count);
                return groundItem;
            }
        }
        */

        //If no same item found
        {
            Vector3 posWithDepth = new Vector3(positionOnGround.x, positionOnGround.y, DynamicZDepth.GetDynamicZDepth(positionOnGround.y, 0));
            GameObject droppedItemPrefab = ResourceManager.Instance.DroppedItem;
            GameObject obj = GameObject.Instantiate(droppedItemPrefab, posWithDepth, Quaternion.identity);
            DroppedItem droppedItem = obj.GetComponent<DroppedItem>();
            droppedItem.Initialize(itemInstance, count, positionOnGround, dropHeight, xSpeed);
            return droppedItem;
        }

    }
}