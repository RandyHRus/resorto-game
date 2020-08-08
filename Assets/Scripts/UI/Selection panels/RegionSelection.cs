using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegionSelection : Selection
{
    RegionInformation info;

    public RegionSelection(RegionInformation info, SelectionPanel parentPanel): base(ResourceManager.Instance.RegionSelection, parentPanel)
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

        this.info = info;
    }

    public override void OnClick()
    {
        base.OnClick();
        PlayerStateMachine.Instance.TrySwitchState<CreateRegionState>(new object[] { info });
    }
}
