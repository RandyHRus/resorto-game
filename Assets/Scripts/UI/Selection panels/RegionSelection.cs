﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegionSelection : ListComponentUI
{
    private readonly RegionInformation info;

    public RegionSelection(RegionInformation info, SelectionPanel<RegionSelection> parentPanel): base(ResourceManager.Instance.RegionComponent, parentPanel.ObjectTransform)
    {
        this.info = info;

        foreach (Transform t in ObjectTransform)
        {
            if (t.tag == "Name Field")
            {
                OutlinedText text = new OutlinedText(t.gameObject);
                text.SetText(info.RegionName);
            }
            else if (t.tag == "Icon Field")
            {
                t.GetComponent<Image>().sprite = info.Icon;
            }
        }
    }


    public override void OnClick()
    {
        base.OnClick();
        PlayerStateMachineManager.Instance.SwitchState<CreateRegionState>(new object[] { info });
    }
}
