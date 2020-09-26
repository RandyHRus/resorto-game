using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegionSelection : ListComponentUI
{
    private readonly RegionInformation info;

    public RegionSelection(RegionInformation info, SelectionPanel<RegionSelection> parentPanel): base(ResourceManager.Instance.RegionComponent, parentPanel.ObjectTransform)
    {
        foreach (Transform t in ObjectTransform)
        {
            if (t.tag == "Name Field")
            {
                OutlinedText text = new OutlinedText(t.gameObject);
                text.SetText(info.name);
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
