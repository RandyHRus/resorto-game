using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class TouristMonoBehaviour : NPCMonoBehaviour
{
    public TouristInstance TouristInstance => (TouristInstance)NpcInstance;
    private TouristHappinessManager happinessManager;
    private TouristHappinessChangeDisplay happinessDiaplay;

    public delegate void TouristDeleting(TouristMonoBehaviour mono);
    public event TouristDeleting OnTouristDeleting;

    public override Type DefaultStateType => typeof(TouristIdleState);

    public override NPCInstance CreateNPCInstance(NPCInformation npcInfo, Transform npcTransform)
    {
        TouristInformation touristInformation = (TouristInformation)npcInfo;

        //Random dialogue
        TouristDialogue dialogue = TouristsGenerator.Instance.GenerateRandomDialogue(touristInformation);
        TouristInterest[] interests = TouristsGenerator.Instance.GetRandomInterestsList();
        TouristHappiness happiness = new TouristHappiness();

        TouristInstance instance = new TouristInstance(touristInformation, npcTransform, dialogue, interests, happiness);
        return instance;
    }

    public override void Initialize(NPCInformation npcInfo)
    {
        base.Initialize(npcInfo);

        happinessManager = new TouristHappinessManager(TouristInstance, stateMachine);
        happinessDiaplay = new TouristHappinessChangeDisplay(TouristInstance);

        NpcInstance.OnNPCDelete += InvokeOnTouristDeleting;
    }

    private void InvokeOnTouristDeleting()
    {
        NpcInstance.OnNPCDelete -= InvokeOnTouristDeleting;
        OnTouristDeleting?.Invoke(this);
    }

    public override Dialogue GetDialogue()
    {
        //Gets dialogue either from state or happiness
        TouristDialogueType type = (CurrentState.GetType() is ITouristStateDialogue overrider) ?
            overrider.GetTouristDialogueType() :
            TouristDialogue.GetDialogueTypeFromHappiness(TouristInstance.happiness.GetTouristHappinessEnum());

        return TouristInstance.dialogue.GetDialogue(type);
    }

    public override void FollowNPC()
    {
        base.FollowNPC();
        Sidebar.Instance.OpenSidebar(SidebarTab.Tourists);
        ((SidebarTouristsPanel)Sidebar.Instance.GetPanel(SidebarTab.Tourists)).SelectTourist(TouristInstance);
    }

    public override void OnDelete()
    {
        base.OnDelete();
    }

    public override NPCScheduleManager CreateNPCScheduleManager(NPCSchedule[] schedules, NPCInstance instance)
    {
        int wakeTime = UnityEngine.Random.Range(5, 10);
        int sleepHours = UnityEngine.Random.Range(6, 9);
        int sleepTime = (int)MathFunctions.Mod(wakeTime - sleepHours, 24);
        int leaveDay = TimeManager.Instance.GetCurrentTime().day + UnityEngine.Random.Range(1, 8);

        return new TouristScheduleManager(new InGameTime(sleepTime, 0, 0), sleepHours, leaveDay, schedules, TouristInstance);
    }

    public override NPCState[] CreateNPCStates(NPCInstance npcInstance)
    {
        NPCState[] additionalStates = {
               new TouristIdleState(npcInstance),
        };

        return base.CreateNPCStates(npcInstance).Union(additionalStates).ToArray();
    }

    public override NPCSchedule[] CreateNPCSchedules(NPCInstance npcInstance)
    {
        NPCSchedule[] additionalSchedules = {
            new TouristCheckInSchedule((TouristInstance)npcInstance)
        };

        return base.CreateNPCSchedules(npcInstance).Union(additionalSchedules).ToArray();
    }
}
