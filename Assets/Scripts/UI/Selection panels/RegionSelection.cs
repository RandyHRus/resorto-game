using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegionSelection : Selection
{
    public RegionSelection(RegionInformation info, Transform parent): base(ResourceManager.Instance.RegionSelection, parent)
    {
        ObjectTransform.Find("Name").GetComponent<Text>().text = info.name;
        ObjectTransform.Find("RegionIcon").GetComponent<Image>().sprite = info.icon;

        AddListener(ObjectInScene.GetComponent<Button>(), info.id);
    }

    private void AddListener(Button button, int regionId)
    {
        button.onClick.AddListener(delegate { RegionsManager.Instance.SetSelectedRegion(regionId); });
    }
}
