using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Inventory")]
public class InventoryState : PlayerState
{
    public override bool AllowMovement { get { return true; } }

    private bool firstExecute = false;

    public override void Initialize()
    {
        base.Initialize();

        PlayerMovement.PlayerMoved += (Vector2 pos, bool slow, Vector2 previousPos) => TryEndState();
    }

    public override void Execute()
    {
        if (firstExecute)
        {
            //We need to do this because first time its ran, inventory button is still pressed
            firstExecute = false;
            return;
        }

        if (Input.GetButtonDown("Inventory"))
        {
            PlayerStateMachine.Instance.TrySwitchState<DefaultState>();
        }
    }

    public override void StartState(object[] args)
    {
        InventoryManager.Instance.ToggleInventory(true);
        firstExecute = true;
    }

    public override bool TryEndState()
    {
        InventoryManager.Instance.ToggleInventory(false);
        return true;
    }
}
