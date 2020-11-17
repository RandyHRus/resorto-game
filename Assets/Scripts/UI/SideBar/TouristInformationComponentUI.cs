using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouristInformationComponentUI : CollapsibleComponentUI
{
    TouristMonoBehaviour touristMono;
    private OutlinedText stateText;
    private OutlinedText whenLeavingText;
    private Image happinessIcon;
    private ProgressBar happinessBar;

    public override int CollapsibleTargetSize => 100;

    public TouristInformationComponentUI(TouristMonoBehaviour touristMono, Transform parent) : base(ResourceManager.Instance.TouristInformationComponentUI, parent)
    {
        this.touristMono = touristMono;
        TouristInstance instance = touristMono.TouristInstance;

        foreach (Transform t in ObjectTransform.GetComponentsInChildren<Transform>())
        {
            switch (t.tag)
            {
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
                        touristMono.OnStateChanged += OnStateChangedHandler;
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
                        TimeManager.Instance.SubscribeToTime(new InGameTime(0, 0), UpdateWhenLeaving); //Refreshes every start of day.

                        break;
                    }
                case ("Progress Field"):
                    {
                        happinessBar = new ProgressBar(ObjectInScene);
                        ChangeHappinessBarFill(touristMono.TouristInstance.happiness.Value);
                        break;
                    }
                case ("Description Field"):
                    {
                        whenLeavingText = new OutlinedText(t.gameObject);
                        UpdateWhenLeaving(null);
                        break;
                    }
            }
        }

        touristMono.TouristInstance.happiness.OnHappinessChanged += OnHappinessChangedHandler;
        OnDestroy += OnDestroyed;
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

    private void UpdateWhenLeaving(object[] args)
    {
        int daysUntilLeave = ((TouristSchedule)touristMono.Schedule).leaveDay - TimeManager.Instance.GetCurrentTime().day;
        string text = "Leaving in: " + daysUntilLeave + (daysUntilLeave == 1 ? " Day" : " Days");
        whenLeavingText.SetText(text);
    }

    private void OnDestroyed(UIObject sender)
    {
        OnDestroy -= OnDestroyed;

        touristMono.OnStateChanged -= OnStateChangedHandler;
        TimeManager.Instance.UnsubscribeFromTime(new InGameTime(0, 0), UpdateWhenLeaving);
        touristMono.TouristInstance.happiness.OnHappinessChanged -= OnHappinessChangedHandler;
    }
}