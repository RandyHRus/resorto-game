using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouristInstance: NPCInstance
{
    public TouristInstance(TouristScriptableObject scriptable, Vector2 position): base(scriptable, position)
    {
        TouristBehaviour b = ObjectInScene.AddComponent<TouristBehaviour>();
        b.OnClick += FollowTourist; //Remember to unsubscribe if I ever implement destroying

        ObjectInScene.AddComponent<TouristRelationship>();
        TouristDialogue dialogueComponent = ObjectInScene.AddComponent<TouristDialogue>();

        dialogueComponent.Initialize(scriptable.CharacterCustomization.CharacterName, scriptable.DialogueFile);
    }

    private void FollowTourist()
    {
        Sidebar.Instance.OpenSidebar(SidebarTab.Tourists);
        ((SidebarTouristsPanel)Sidebar.Instance.GetPanel(SidebarTab.Tourists)).SelectTourist(this);
        PlayerStateMachine.Instance.SwitchState<FollowNPCState>(new object[] { this });
    }
}
