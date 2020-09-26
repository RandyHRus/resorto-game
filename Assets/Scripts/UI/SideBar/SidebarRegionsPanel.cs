using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SidebarRegionsPanel : SidebarPanel
{
    [SerializeField] private RegionInformation[] regions = null;

    private SelectionPanel<RegionSelection> regionsPanel;

    void Start()
    {
        regionsPanel = new SelectionPanel<RegionSelection>(transform.Find("RegionSelection").gameObject);
    }

    void Update()
    {
        foreach (RegionInformation i in regions)
        {
            regionsPanel.InsertListComponent(new RegionSelection(i, regionsPanel));
        }
    }
}
