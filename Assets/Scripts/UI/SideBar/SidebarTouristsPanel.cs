using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SidebarTouristsPanel : SidebarPanel
{
    private SelectionPanel<TouristInformationComponentUI> touristsComponentsPanel;
    private Dictionary<TouristInstance, TouristInformationComponentUI> touristToComponent = new Dictionary<TouristInstance, TouristInformationComponentUI>();

    protected override void Awake()
    {
        base.Awake();

        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            if (t.tag == "List Field")
                touristsComponentsPanel = new SelectionPanel<TouristInformationComponentUI>(t.gameObject);
        }

        TouristsManager.OnTouristAdded += AddTouristComponent;
        TouristsManager.OnTouristRemoved += RemoveTouristComponent;
    }

    public void SelectTourist(TouristInstance instance)
    {
        TouristInformationComponentUI component = touristToComponent[instance];
        touristsComponentsPanel.SetScrollToComponent(component);

        component.OnClick();
    }
    
    private void AddTouristComponent(TouristMonoBehaviour touristMono)
    {
        TouristInformationComponentUI component = new TouristInformationComponentUI(touristMono, touristsComponentsPanel.ObjectTransform);
        touristsComponentsPanel.InsertListComponent(component);
        touristToComponent.Add(touristMono.TouristInstance, component);
    }

    private void RemoveTouristComponent(TouristMonoBehaviour touristMono)
    {
        TouristInformationComponentUI component = touristToComponent[touristMono.TouristInstance];
        touristsComponentsPanel.RemoveListComponent(component);
        touristToComponent.Remove(touristMono.TouristInstance);
    }
}
