using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Resource")]
public class ResourceItemInformation : InventoryItemInformation
{
    public override ItemTag Tag => ItemTag.Resource;

    public override bool Stackable => true;

    public override void ItemSelected()
    {
        
    }
}
