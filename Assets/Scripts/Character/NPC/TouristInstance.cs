using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouristInstance: NPCInstance
{
    public TouristInstance(TouristScriptableObject scriptable, Vector2 position): base(scriptable, position)
    {
        ObjectInScene.AddComponent<TouristBehaviour>();
        ObjectInScene.AddComponent<TouristRelationship>();
        TouristDialogue dialogueComponent = ObjectInScene.AddComponent<TouristDialogue>();

        dialogueComponent.Initialize(scriptable.CharacterCustomization.CharacterName, scriptable.DialogueFile);
    }
}
