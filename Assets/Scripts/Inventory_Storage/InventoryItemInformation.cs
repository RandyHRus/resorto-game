using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryItemInformation : ScriptableObject
{
    [SerializeField] private string itemName = "";
    public string ItemName => itemName;

    [SerializeField] private Sprite itemIcon = null;
    public Sprite ItemIcon => itemIcon;

    public abstract ItemTag Tag { get; }

    public abstract bool Stackable { get; }

    public abstract void ItemSelected();
}

public enum ItemTag
{
    Furniture,
    Tool,
    Seeds,
    Resource,
    Cosmetic,
    Food
}
