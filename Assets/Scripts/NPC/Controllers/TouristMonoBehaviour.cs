using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouristMonoBehaviour : NPCMonoBehaviour
{
    public TouristInstance TouristInstance => (TouristInstance)NpcInstance;
    private TouristHappinessManager happinessManager;
    private TouristHappinessChangeDisplay happinessDiaplay;

    public delegate void TouristDeleting(TouristMonoBehaviour mono);
    public event TouristDeleting OnTouristDeleting;

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
        void OnGoBackToUnloadingDockHandler()
        {
            ((TouristSchedule)Schedule).OnGoBackToUnloadingDockTime -= OnGoBackToUnloadingDockHandler;
            ((TouristSchedule)Schedule).OnGoToBedLocationTime -= GoToBedLocation; //We don't want the tourist going to bed while going back to unloading dock!
            GoBackToUnloadingDock();
        }

        base.Initialize(npcInfo);

        ((TouristSchedule)Schedule).OnGoBackToUnloadingDockTime += OnGoBackToUnloadingDockHandler;
        ((TouristSchedule)Schedule).OnGoToBedLocationTime += GoToBedLocation;

        happinessManager = new TouristHappinessManager(TouristInstance, stateMachine);
        happinessDiaplay = new TouristHappinessChangeDisplay(TouristInstance);

        stateMachine.GetStateInstance<NPCLeaveAndDeleteState>().OnNPCDeleting += InvokeOnTouristDeleting;
    }

    private void InvokeOnTouristDeleting()
    {
        stateMachine.GetStateInstance<NPCLeaveAndDeleteState>().OnNPCDeleting -= InvokeOnTouristDeleting;
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

    private void GoBackToUnloadingDock()
    {
        Debug.Log("Going back to unloading dock");
        Vector2Int unloadingPosition = RegionManager.GetRandomRegionInstanceOfType(ResourceManager.Instance.BoatUnloadingRegion).GetRegionPositionsAsList()[0];
        SwitchState<NPCWalkToPositionState>(new object[] { unloadingPosition, typeof(NPCLeaveAndDeleteState), null, "Leaving island" });
    }

    private void GoToBedLocation(Vector2Int bedAccessLocation)
    {
        Debug.Log("Going to bed");
        SwitchState<NPCWalkToPositionState>(new object[] { bedAccessLocation, typeof(NPCSleepingState), null, "Going to bed" });
    }

    public override void OnDelete()
    {
        base.OnDelete();
        ((TouristSchedule)Schedule).OnGoToBedLocationTime -= GoToBedLocation;
    }
}
