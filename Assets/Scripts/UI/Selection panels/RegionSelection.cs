using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegionSelection : Selection
{
    public RegionSelection(RegionInformation info, Transform parentPanel): base(ResourceManager.Instance.RegionSelection, parentPanel)
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

        AddListener(ObjectInScene.GetComponent<Button>(), info);
    }

    private void AddListener(Button button, RegionInformation info)
    {
        button.onClick.AddListener(delegate { PlayerStateMachine.Instance.TrySwitchState<CreateRegionState>(new object[] { info }); } );
    }
}
