using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

[CreateAssetMenu(menuName = "Character/Tourist")]
public class TouristScriptableObject : CharacterScriptableObject
{
    [SerializeField] private TextAsset dialogueFile = null;

    public void CreateInScene(Vector2Int position)
    {
        Vector2 vec = position;
        float depth = DynamicZDepth.GetDynamicZDepth(vec, DynamicZDepth.NPC_OFFSET);
        GameObject instance = GameObject.Instantiate(ResourceManager.Instance.Character, new Vector3(position.x, position.y, depth), Quaternion.identity);

        instance.GetComponent<CharacterCustomizationLoader>().LoadCustomization(CharacterCustomization);
        instance.AddComponent<TouristBehaviour>();
        instance.AddComponent<TouristRelationship>();
        TouristDialogue dialogueComponent = instance.AddComponent<TouristDialogue>();
    
        dialogueComponent.Initialize(CharacterCustomization.CharacterName, dialogueFile);

        instance.layer = LayerMask.NameToLayer("Interactable");
        instance.tag = "NPC";
    }
}