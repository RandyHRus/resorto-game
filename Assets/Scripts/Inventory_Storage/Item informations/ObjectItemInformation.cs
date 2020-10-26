using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Object")]
public class ObjectItemInformation : InventoryItemInformation
{
    [SerializeField] private ObjectInformation objectToPlace = null;
    public ObjectInformation ObjectToPlace => objectToPlace;

    [SerializeField] private ItemTag tag = 0;
    public override ItemTag Tag => tag;

    [SerializeField] private bool stackable = false;
    public override bool Stackable => stackable;

    public override void ItemSelected()
    {
        PlayerStateMachineManager.Instance.SwitchState<CreateObjectsState>(new object[] { objectToPlace });
    }
}
