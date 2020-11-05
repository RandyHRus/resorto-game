using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SidebarTouristsPanel : SidebarPanel
{
    private SelectionPanel<TouristInformationComponentUI> touristsComponentsPanel;
    private Dictionary<TouristInstance, TouristInformationComponentUI> touristToComponent = new Dictionary<TouristInstance, TouristInformationComponentUI>();

    public class TouristInformationComponentUI: ListComponentUI
    {
        TouristMonoBehaviour touristMono;
        private OutlinedText stateText;

        private Image happinessIcon;
        private ProgressBar happinessBar;

        public TouristInformationComponentUI(TouristMonoBehaviour touristMono, Transform parent): base(ResourceManager.Instance.TouristInformationComponentUI, parent)
        {
            this.touristMono = touristMono;
            TouristInstance instance = touristMono.TouristInstance;

            foreach (Transform t in ObjectTransform.GetComponentsInChildren<Transform>())
            {
                switch (t.tag) {
                    case ("Character Field"):
                        {
                            CharacterCustomizationLoader loader = t.GetComponent<CharacterCustomizationLoader>();
                            loader.LoadCustomization(instance.TouristInformation.CharacterCustomization);
                            break;
                        }

                    case ("Name Field"):
                        {
                            OutlinedText text = new OutlinedText(t.gameObject);
                            text.SetText(instance.TouristInformation.NpcName);
                            break;
                        }
                    case ("State Field"):
                        {
                            stateText = new OutlinedText(t.gameObject);
                            stateText.SetText(touristMono.CurrentState.DisplayMessage);
                            touristMono.OnStateChanged += OnStateChangedHandler;//Remember to unsubscribe if I ever implement destroying  
                            break;
                        }
                    case ("Icon Field"):
                        {
                            Image img = t.GetComponent<Image>();
                            if (instance.interests.Length > 0)
                                img.sprite = instance.interests[0].Icon;
                            else
                                img.enabled = false;

                            break;
                        }
                    case ("Icon Field 2"):
                        {
                            Image img = t.GetComponent<Image>();
                            if (instance.interests.Length > 1)
                                img.sprite = instance.interests[1].Icon;
                            else
                                img.enabled = false;

                            break;
                        }
                    case ("Icon Field 3"):
                        {
                            Image img = t.GetComponent<Image>();
                            if (instance.interests.Length > 2)
                                img.sprite = instance.interests[2].Icon;
                            else
                                img.enabled = false;

                            break;
                        }
                    case ("Icon Field 4"):
                        {
                            Image img = t.GetComponent<Image>();
                            if (instance.interests.Length > 3)
                                img.sprite = instance.interests[3].Icon;
                            else
                                img.enabled = false;

                            break;
                        }
                    case ("Icon Field 5"):
                        {
                            happinessIcon = t.GetComponent<Image>();
                            ChangeHappinessIcon(touristMono.TouristInstance.happiness.GetTouristHappinessEnum());

                            break;
                        }
                    case ("Progress Field"):
                        {
                            happinessBar = new ProgressBar(ObjectInScene);
                            ChangeHappinessBarFill(touristMono.TouristInstance.happiness.Value);
                            break;
                        }
                    case ("List Field"):
                        {

                        }
                }
            }

            //TODO: remember to unsubscribe on tourist deletion;
            touristMono.TouristInstance.happiness.OnHappinessChanged += OnHappinessChangedHandler;
        }

        public override void OnClick()
        {
            base.OnClick();
            PlayerStateMachineManager.Instance.SwitchState<FollowNPCState>(new object[] { touristMono.TouristInstance });
        }

        private void OnStateChangedHandler(NPCState previousState, NPCState newState)
        {
            //Change state display message
            stateText.SetText(newState.DisplayMessage);
        }

        private void OnHappinessChangedHandler(TouristHappinessFactor changeFactor, int newValue, TouristHappinessEnum newHappinessEnum)
        {
            ChangeHappinessIcon(newHappinessEnum);
            ChangeHappinessBarFill(newValue);
        }

        private void ChangeHappinessIcon(TouristHappinessEnum happinessEnum)
        {
            happinessIcon.sprite = ResourceManager.Instance.GetTouristHappinessIcon(happinessEnum);
        }

        private void ChangeHappinessBarFill(int happinessValue)
        {
            happinessBar.SetFill(happinessValue / 100f);
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
}
