using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class TouristMonoBehaviour : NPCMonoBehaviour
{
    public TouristInformation TouristInformation => (TouristInformation)NpcInformation;
    public TouristComponents TouristComponents => (TouristComponents)NPCComponents;
    private TouristHappinessManager happinessManager;
    private TouristHappinessChangeDisplay happinessDisplay;
    private LuggageManager luggageManager;

    public delegate void TouristDeleting(TouristMonoBehaviour mono);
    public event TouristDeleting OnTouristDeleting;

    public override Type DefaultStateType => typeof(TouristIdleState);

    public override void Initialize(NPCInformation npcInfo, NPCComponents npcComponents)
    {
        base.Initialize(npcInfo, npcComponents);

        happinessManager = new TouristHappinessManager(TouristComponents, stateMachine);
        happinessDisplay = new TouristHappinessChangeDisplay(TouristComponents);
        luggageManager = new LuggageManager(TouristComponents);

        NPCComponents.SubscribeToEvent(NPCInstanceEvent.Delete, InvokeOnTouristDeleting);
    }

    private void InvokeOnTouristDeleting(object[] args)
    {
        NPCComponents.UnsubscribeToEvent(NPCInstanceEvent.Delete, InvokeOnTouristDeleting);
        OnTouristDeleting?.Invoke(this);
    }

    public override Dialogue GetDialogue()
    {
        //Gets dialogue either from state or happiness
        TouristDialogueType type = (CurrentState.GetType() is ITouristStateDialogue overrider) ?
            overrider.GetTouristDialogueType() :
            TouristDialogue.GetDialogueTypeFromHappiness(TouristComponents.happiness.GetTouristHappinessEnum());

        return TouristComponents.dialogue.GetDialogue(type);
    }

    public override void FollowNPC()
    {
        base.FollowNPC();
        Sidebar.Instance.OpenSidebar(SidebarTab.Tourists);
        ((SidebarTouristsPanel)Sidebar.Instance.GetPanel(SidebarTab.Tourists)).SelectTourist(TouristComponents);
    }

    public override NPCScheduleManager CreateNPCScheduleManager(NPCSchedule[] schedules, NPCComponents npcComponents)
    {
        int wakeTime = UnityEngine.Random.Range(5, 10);
        int sleepHours = UnityEngine.Random.Range(6, 9);
        int sleepTime = (int)MathFunctions.Mod(wakeTime - sleepHours, 24);
        int leaveDay = TimeManager.Instance.GetCurrentTime().day + UnityEngine.Random.Range(1, 8);

        return new TouristScheduleManager(new InGameTime(sleepTime, 0, 0), sleepHours, leaveDay, schedules, TouristComponents);
    }

    public override NPCState[] GetNPCStates(NPCComponents npcComponents)
    {
        NPCState[] additionalStates = {
               new TouristIdleState(npcComponents),
        };

        return base.GetNPCStates(npcComponents).Union(additionalStates).ToArray();
    }

    public override NPCSchedule[] GetNPCSchedules(NPCComponents npcComponents)
    {
        NPCSchedule[] additionalSchedules = {
            new TouristCheckInSchedule((TouristComponents)npcComponents)
        };

        return base.GetNPCSchedules(NPCComponents).Union(additionalSchedules).ToArray();
    }
}
