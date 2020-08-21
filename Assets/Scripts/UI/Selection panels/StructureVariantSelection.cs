using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StructureVariantSelection : Selection
{
    private StructureInformation structureInformation;
    private StructureVariantInformation variantInformation;

    public StructureVariantSelection(StructureInformation structureInfo, StructureVariantInformation variantInfo, SelectionPanel parentPanel) : base(ResourceManager.Instance.StructureSelection, parentPanel)
    {
        foreach (Transform t in ObjectTransform)
        {
            if (t.tag == "Name Field")
            {
                OutlinedText text = new OutlinedText(t.gameObject);
                text.SetText(variantInfo.name);
            }
            else if (t.tag == "Icon Field")
            {
                t.GetComponent<Image>().sprite = variantInfo.Icon;
            }
        }

        this.structureInformation = structureInfo;
        this.variantInformation = variantInfo;
    }

    public override void OnClick()
    {
        base.OnClick();

        PlayerStateMachine.Instance.TrySwitchState(structureInformation.OnSelectState.GetType(), new object[] { variantInformation });
    }
}
