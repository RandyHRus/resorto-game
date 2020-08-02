using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

[CreateAssetMenu]
public class TouristScriptableObject : ScriptableObject
{
    [SerializeField] private string touristName = null;
    [SerializeField] private TextAsset dialogueFile = null;
    [SerializeField] private CharacterInformation character = null;

    public void CreateInScene(Vector2Int position)
    {
        Vector2 vec = position;
        float depth = DynamicZDepth.GetDynamicZDepth(vec, DynamicZDepth.NPC_OFFSET);
        GameObject instance = GameObject.Instantiate(ResourceManager.Instance.Character, new Vector3(position.x, position.y, depth), Quaternion.identity);

        instance.GetComponent<CharacterCustomizationLoader>().LoadCharacter(character);
        instance.AddComponent<TouristBehaviour>();
        instance.AddComponent<TouristRelationship>();
        TouristDialogue dialogueComponent = instance.AddComponent<TouristDialogue>();
    
        dialogueComponent.Initialize(touristName, dialogueFile);

        instance.layer = LayerMask.NameToLayer("Interactable");
        instance.tag = "NPC";
    }
}