using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SidebarTouristsPanel : SidebarPanel
{
    private SelectionPanel<TouristInformationComponentUI> touristsComponentsPanel;
    private Dictionary<TouristComponents, TouristInformationComponentUI> touristToComponent = new Dictionary<TouristComponents, TouristInformationComponentUI>();

    protected override void Awake()
    {
        base.Awake();

        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            if (t.tag == "List Field")
                touristsComponentsPanel = new SelectionPanel<TouristInformationComponentUI>(t.gameObject);
        }
    }

    private void Start()
    {
        TouristsManager.Instance.OnTouristAdded += AddTouristComponent;
        TouristsManager.Instance.OnTouristRemoved += RemoveTouristComponent;
    }

    private void OnDestroy()
    {
        TouristsManager.Instance.OnTouristAdded -= AddTouristComponent;
        TouristsManager.Instance.OnTouristRemoved -= RemoveTouristComponent;
    }

    public void SelectTourist(TouristComponents touristComponents)
    {
        TouristInformationComponentUI component = touristToComponent[touristComponents];
        touristsComponentsPanel.SetScrollToComponent(component);

        component.OnClick();
    }
    
    private void AddTouristComponent(TouristMonoBehaviour touristMono)
    {
        TouristInformationComponentUI component = new TouristInformationComponentUI(touristMono, touristsComponentsPanel.ObjectTransform);
        touristsComponentsPanel.InsertListComponent(component);
        touristToComponent.Add(touristMono.TouristComponents, component);
    }

    private void RemoveTouristComponent(TouristMonoBehaviour touristMono)
    {
        TouristInformationComponentUI component = touristToComponent[touristMono.TouristComponents];
        touristsComponentsPanel.RemoveListComponent(component);
        touristToComponent.Remove(touristMono.TouristComponents);
    }
}
