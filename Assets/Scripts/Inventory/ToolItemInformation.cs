using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ToolItemInformation : InventoryItemInformation
{
    [SerializeField] private ToolState stateWhenHeld = ToolState.breakMode;
    public ToolState StateWhenHeld => stateWhenHeld;
}

//Add IPlayerInstance states in ToolInformationManager
public enum ToolState
{
    breakMode,
    fishing
}