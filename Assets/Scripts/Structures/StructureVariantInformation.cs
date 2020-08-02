using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StructureVariantInformation : ScriptableObject
{
    [SerializeField] private string variantName = "";
    public string VariantName => variantName;

    [SerializeField] private Sprite icon = null;
    public Sprite Icon => icon;
}
