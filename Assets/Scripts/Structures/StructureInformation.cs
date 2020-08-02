using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class StructureInformation : ScriptableObject
{
    [SerializeField] private string structureName = null;
    public string StructureName => structureName;

    [SerializeField] private Sprite icon = null;
    public Sprite Icon => icon;

    [SerializeField] private StructureVariantInformation[] variants = null;
    public StructureVariantInformation[] Variants => variants;

    [SerializeField] private PlayerState onSelectState = null;
    public PlayerState OnSelectState => onSelectState;
}
