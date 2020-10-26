using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class NPCInformation
{
    [SerializeField] private string npcName = "";
    public string NpcName => npcName;

    [SerializeField] public CharacterCustomization characterCustomization = null;
    public CharacterCustomization CharacterCustomization => characterCustomization;

    public NPCInformation(string name, CharacterCustomization customization)
    {
        this.npcName = name;
        this.characterCustomization = customization;
    }

    //Should not be overridden because it returns
    public NPCMonoBehaviour CreateInScene(Vector2 position)
    {
        float depth = DynamicZDepth.GetDynamicZDepth(position, DynamicZDepth.NPC_OFFSET);

        GameObject obj = GameObject.Instantiate(ObjectToInitialize, new Vector3(position.x, position.y, depth), Quaternion.identity);

        obj.GetComponent<CharacterCustomizationLoader>().LoadCustomization(characterCustomization);

        obj.layer = LayerMask.NameToLayer("Interactable");
        obj.tag = "NPC";

        NPCMonoBehaviour mono = obj.GetComponent<NPCMonoBehaviour>();
        mono.Initialize(this);

        return mono;
    }

    protected abstract GameObject ObjectToInitialize { get; }
}
