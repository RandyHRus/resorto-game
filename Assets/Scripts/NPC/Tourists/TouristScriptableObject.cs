using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[CreateAssetMenu]
public class TouristScriptableObject : ScriptableObject
{
    [SerializeField] private string touristName = null;
    [SerializeField] private TextAsset dialoguesFile = null;
    [SerializeField] private CharacterInformation character = null;

    public void CreateInScene(Vector2Int position)
    {
        Vector2 vec = position;
        float depth = DynamicZDepth.GetDynamicZDepth(vec, DynamicZDepth.NPC_OFFSET);
        GameObject instance = GameObject.Instantiate(ResourceManager.Instance.character, new Vector3(position.x, position.y, depth), Quaternion.identity);

        instance.GetComponent<CharacterCustomizationLoader>().LoadCharacter(character);
        instance.AddComponent<TouristBehaviour>();
        instance.AddComponent<TouristRelationship>();
        TouristDialogue dialogueComponent = instance.AddComponent<TouristDialogue>();

        dialogueComponent.Initialize(touristName, dialoguesFile);

        instance.layer = LayerMask.NameToLayer("NPC");
    }
}