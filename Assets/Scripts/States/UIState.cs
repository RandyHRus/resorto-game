using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/UI")]
public class UIState : PlayerState
{
    public override bool AllowMovement { get { return true; } }

    private bool firstExecute = false;

    public override void Initialize()
    {
        base.Initialize();
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
            PlayerStateMachine.Instance.SwitchState<DefaultState>();
        }
    }

    public override void StartState(object[] args)
    {
        PlayerMovement.PlayerMoved += SwitchDefaultState;

        if (args == null || args.Length == 0)
            InventoryManager.Instance.ShowInventory(null);
        else if (args[0] is BuildingStructureVariant v)
            InventoryManager.Instance.ShowInventory(v.uiToShowOnSelect);
        else if (args[0] is UIObject u)
            InventoryManager.Instance.ShowInventory(u);
        else
            throw new System.NotImplementedException();

        firstExecute = true;
    }

    public override void EndState()
    {
        PlayerMovement.PlayerMoved -= SwitchDefaultState;
        InventoryManager.Instance.HideInventory();
    }

    private void SwitchDefaultState(Vector2 pos, bool slow, Vector2 directionVector)
    {
        PlayerStateMachine.Instance.SwitchState<DefaultState>();
    }
}
