using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StructureSelection : Selection
{
    private static SelectionPanel currentShownVariantSelection = null;

    private SelectionPanel structureVariantsPanel;
    private StructureInformation structureInfo;

    public StructureSelection(StructureInformation info, SelectionPanel structureVariantsPanel, SelectionPanel parentPanel) : base(ResourceManager.Instance.StructureSelection, parentPanel)
    {
        foreach (Transform t in ObjectTransform)
        {
            if (t.tag == "Name Field")
            {
                OutlinedText text = new OutlinedText(t.gameObject);
                text.SetText(info.StructureName);
            }
            else if (t.tag == "Icon Field")
            {
                t.GetComponent<Image>().sprite = info.Icon;
            }
        }

        this.structureVariantsPanel = structureVariantsPanel;
        this.structureInfo = info;
    }

    public override void OnClick()
    {
        base.OnClick();

        currentShownVariantSelection?.Show(false);

        currentShownVariantSelection = structureVariantsPanel;
        structureVariantsPanel.Show(true);
    }
}