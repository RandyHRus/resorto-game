using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SidebarRegionsPanel : SidebarPanel
{
    [SerializeField] private RegionInformation[] regions = null;

    private SelectionPanel<CreateRegionSelection> createRegionsPanel;
    private SelectionPanel<ManageRegionSelection> manageRegionsPanel;
    private UIObject currentActiveTabPanel;

    private Dictionary<RegionInstance, ManageRegionSelection> instanceToManageComponent = new Dictionary<RegionInstance, ManageRegionSelection>();

    protected override void Awake()
    {
        base.Awake();
        RegionManager.OnRegionCreated += OnRegionCreatedHandler;
        RegionManager.OnRegionRemoved += OnRegionRemovedHandler;
    }

    void Start()
    {
        foreach (Transform t in transform.GetComponentsInChildren<Transform>())
        {
            if (t.tag == "List Field")
            {
                createRegionsPanel = new SelectionPanel<CreateRegionSelection>(t.gameObject);
                t.gameObject.SetActive(false);
            }
            else if (t.tag == "List Field 2")
            {
                manageRegionsPanel = new SelectionPanel<ManageRegionSelection>(t.gameObject);
                t.gameObject.SetActive(false);
            }
        }

        foreach (RegionInformation i in regions)
        {
            createRegionsPanel.InsertListComponent(new CreateRegionSelection(i, createRegionsPanel));
        }

        OnPanelOpen += OnPanelOpenHandler;
        OnPanelClosed += OnPanelClosedHandler;

        ShowTab(0);
    }

    void OnPanelOpenHandler()
    {
        PlayerStateMachineManager.Instance.SwitchState<SelectRegionState>();
    }

    void OnPanelClosedHandler()
    {
        PlayerStateMachineManager.Instance.SwitchDefaultState();
    }

    public void ShowTab(int tabIndex)
    {
        currentActiveTabPanel?.ObjectInScene.SetActive(false);

        UIObject toActivate;

        switch (tabIndex)
        {
            case (0):
                toActivate = createRegionsPanel;
                break;
            case (1):
                toActivate = manageRegionsPanel;
                break;
            default:
                throw new System.NotImplementedException();
        }

        toActivate.ObjectInScene.SetActive(true);
        currentActiveTabPanel = toActivate;
    }

    private void OnRegionCreatedHandler(RegionInstance instance)
    {
        ManageRegionSelection newSelection = new ManageRegionSelection(instance, manageRegionsPanel.ObjectTransform);
        manageRegionsPanel.InsertListComponent(newSelection);
        instanceToManageComponent.Add(instance, newSelection);
    }

    private void OnRegionRemovedHandler(RegionInstance instance)
    {
        manageRegionsPanel.RemoveListComponent(instanceToManageComponent[instance]);
        instanceToManageComponent.Remove(instance);
    }

    public void SelectRegionManageComponent(RegionInstance region)
    {
        ManageRegionSelection sel = instanceToManageComponent[region];
        manageRegionsPanel.SetScrollToComponent(sel);

        sel.OnClick();
    }
}
