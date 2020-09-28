using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SidebarRegionsPanel : SidebarPanel
{
    [SerializeField] private RegionInformation[] regions = null;

    private SelectionPanel<RegionSelection> regionsPanel;

    void Start()
    {
        foreach (Transform t in transform.GetComponentsInChildren<Transform>())
        {
            if (t.tag == "List Field")
                regionsPanel = new SelectionPanel<RegionSelection>(t.gameObject);
        }

        foreach (RegionInformation i in regions)
        {
            regionsPanel.InsertListComponent(new RegionSelection(i, regionsPanel));
        }
    }
}
