using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouristMonoBehaviour : NPCMonoBehaviour
{
    public TouristInstance TouristInstance => (TouristInstance)NpcInstance;
    private TouristHappinessManager happinessManager;

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
}
