using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
    AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
[Obsolete("Not used any more", true)]
public class TwoColorsCosmeticColorHideAttribute : PropertyAttribute
{
    public bool primary;
    public string cosmeticItemInformationSourceField;

    public TwoColorsCosmeticColorHideAttribute(bool primary, string cosmeticItemInformationSourceField)
    {
        this.primary = primary;
        this.cosmeticItemInformationSourceField = cosmeticItemInformationSourceField;
    }
}
