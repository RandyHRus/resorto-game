using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisitorInformation : NPCInformation
{
    protected override GameObject ObjectToInitialize => ResourceManager.Instance.VisitorNpc;

    public VisitorInformation(string name, CharacterCustomization customization) : base(name, customization)
    {
        throw new System.NotImplementedException();
    }
}
