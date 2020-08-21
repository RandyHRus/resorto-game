using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItems : MonoBehaviour
{
    private static readonly float pickupNearbyItemsScanRadius = 1f;

    private void Awake()
    {
        PlayerMovement.PlayerMoved += (Vector2 pos, bool slow, Vector2 previousPos) => PickUpNearby(pos);
    }

    public void PickUpNearby(Vector2 pos)
    {
        Collider2D[] results = Physics2D.OverlapCircleAll(pos, pickupNearbyItemsScanRadius, 1 << LayerMask.NameToLayer("DropItems"));
        foreach (Collider2D result in results)
        {
            DroppedItem item = result.GetComponent<DroppedItem>();
            if (!item.Moving)
            {
                item.StartFlyToTarget(transform, OnFlyEnd);
            }
        }

        void OnFlyEnd(DroppedItem item)
        {
            //Gain item here
            InventoryManager.Instance.AddItem(item.ItemInfo, item.Count);
            Destroy(item.gameObject);
        }
    }
}
