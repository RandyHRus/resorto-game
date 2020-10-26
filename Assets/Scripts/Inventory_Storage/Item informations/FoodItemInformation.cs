using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Food")]
public class FoodItemInformation : InventoryItemInformation
{
    public override ItemTag Tag { get { return ItemTag.Food; }}

    public override bool Stackable { get { return true; } }

    [Range(1, 10), SerializeField] private int restoration = 1;
    public int Restoration => restoration;

    public override void ItemSelected()
    {
        PlayerStateMachineManager.Instance.SwitchState<PlayerEatState>(new object[] { this });
    }
}
