using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryItemInformation : ScriptableObject
{
    [SerializeField] private string itemName = "";
    public string ItemName => itemName;

    [SerializeField] private Sprite itemIcon = null;
    public Sprite ItemIcon => itemIcon;

    [SerializeField] private ItemTag tag = 0;
    public ItemTag Tag => tag;
}

public enum ItemTag
{
    Furniture,
    Tool,
    Seeds,
    Resource
}
