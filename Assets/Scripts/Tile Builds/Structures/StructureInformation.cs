using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Need this to be able to create a list of structure informations
public abstract class StructureInformation
{
    [SerializeField] private string structureName = null;
    public string StructureName => structureName;

    [SerializeField] private Sprite icon = null;
    public Sprite Icon => icon;

    [SerializeField] private PlayerState onSelectState = null;
    public PlayerState OnSelectState => onSelectState;

    public abstract StructureVariantInformation[] Variants { get; }
}

[System.Serializable]
public abstract class StructureInformation<T> : StructureInformation where T: StructureVariantInformation
{
    [SerializeField] private T[] variants = null;
    public override StructureVariantInformation[] Variants => variants;
}
