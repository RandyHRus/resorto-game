using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidebarTouristsPanel : SidebarPanel
{
    private SelectionPanel<TouristInformationComponentUI> touristsComponentsPanel;

    public class TouristInformationComponentUI: ListComponentUI
    {
        TouristInstance instance;

        public TouristInformationComponentUI(TouristInstance instance, Transform parent): base(ResourceManager.Instance.TouristInformationComponentUI, parent)
        {
            this.instance = instance;

            foreach (Transform t in ObjectTransform.GetComponentsInChildren<Transform>())
            {
                if (t.tag == "Character Field")
                {
                    CharacterCustomizationLoader loader = t.GetComponent<CharacterCustomizationLoader>();
                    loader.LoadCustomization(instance.CharacterInformation.CharacterCustomization);
                }
                else if (t.tag == "Name Field")
                {
                    OutlinedText text = new OutlinedText(t.gameObject);
                    text.SetText(instance.CharacterInformation.CharacterCustomization.CharacterName);
                }
            }
        }

        public override void OnClick()
        {
            base.OnClick();
            PlayerStateMachine.Instance.SwitchState<FollowNPCState>(new object[] { instance });
        }
    }

    protected override void Awake()
    {
        base.Awake();

        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            if (t.tag == "List Field")
                touristsComponentsPanel = new SelectionPanel<TouristInformationComponentUI>(t.gameObject);
        }

        NPCManager.OnTouristAdded += AddTouristComponent;
    }
    
    private void AddTouristComponent(TouristInstance instance)
    {
        TouristInformationComponentUI component = new TouristInformationComponentUI(instance, touristsComponentsPanel.ObjectTransform);
        touristsComponentsPanel.InsertListComponent(component);
    }
}
