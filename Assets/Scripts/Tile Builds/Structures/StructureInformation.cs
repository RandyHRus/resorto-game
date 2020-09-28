using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StructureInformation: ScriptableObject
{
    [SerializeField] private string _name = "";
    public string Name => _name;

    [SerializeField] private Sprite icon = null;
    public Sprite Icon => icon;
}