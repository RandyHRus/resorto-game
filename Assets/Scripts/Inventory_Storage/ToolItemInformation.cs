using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ToolItemInformation : InventoryItemInformation
{
    [SerializeField] private PlayerState stateWhenHeld = null;
    public PlayerState StateWhenHeld => stateWhenHeld;
}