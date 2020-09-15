using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Tool")]
public class ToolItemInformation : InventoryItemInformation
{
    [SerializeField] private PlayerState stateWhenHeld = null;
    public PlayerState StateWhenHeld => stateWhenHeld;

    public override ItemTag Tag { get { return ItemTag.Tool; } }

    [SerializeField] private bool stackable = false;
    public override bool Stackable => stackable;

    public override void ItemSelected()
    {
        PlayerStateMachine.Instance.TrySwitchState(StateWhenHeld.GetType());
    }
}