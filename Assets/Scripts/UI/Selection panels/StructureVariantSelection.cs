using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StructureVariantSelection : Selection
{
    public StructureVariantSelection(StructureInformation structureInfo, StructureVariantInformation variantInfo, Transform parentPanel) : base(ResourceManager.Instance.StructureSelection, parentPanel)
    {
        foreach (Transform t in ObjectTransform)
        {
            if (t.tag == "Name Field")
            {
                t.GetComponent<Text>().text = variantInfo.name;
            }
            else if (t.tag == "Icon Field")
            {
                t.GetComponent<Image>().sprite = variantInfo.Icon;
            }
        }

        AddListener(ObjectInScene.GetComponent<Button>(), structureInfo, variantInfo);
    }

    private void AddListener(Button button, StructureInformation structureInfo, StructureVariantInformation info)
    {
        PlayerState state = structureInfo.OnSelectState;
        Type stateType = state.GetType();
        button.onClick.AddListener(delegate { PlayerStateMachine.Instance.TrySwitchState(stateType, new object[] { info }); });
    }
}
