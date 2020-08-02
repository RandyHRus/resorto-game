using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StructureSelection : Selection
{
    private static SelectionPanel currentShownVariantSelection = null;

    public StructureSelection(StructureInformation info, SelectionPanel structureVariantsPanel,Transform parentPanel): base(ResourceManager.Instance.StructureSelection, parentPanel)
    {
        foreach (Transform t in ObjectTransform)
        {
            if (t.tag == "Name Field")
            {
                t.GetComponent<Text>().text = info.name;
            }
            else if (t.tag == "Icon Field")
            {
                t.GetComponent<Image>().sprite = info.Icon;
            }
        }

        AddListener(ObjectInScene.GetComponent<Button>(), info, structureVariantsPanel);
    }

    private void AddListener(Button button, StructureInformation info, SelectionPanel structureVariantsPanel)
    {
        button.onClick.AddListener(delegate {

            if (currentShownVariantSelection != null)
                currentShownVariantSelection.Show(false);

            currentShownVariantSelection = structureVariantsPanel;
            structureVariantsPanel.Show(true);
        });
    }
}
