using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ObjectItemInformation : InventoryItemInformation
{
    [SerializeField] private ObjectInformation objectToPlace = null;
    public ObjectInformation ObjectToPlace => objectToPlace;

    //public override string itemName => throw new System.NotImplementedException();
}
