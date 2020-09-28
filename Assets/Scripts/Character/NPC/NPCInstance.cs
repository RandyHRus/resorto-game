using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInstance
{
    public CharacterScriptableObject CharacterInformation { get; }
    public GameObject ObjectInScene { get; }
    public Transform ObjectTransform { get; }

    public NPCInstance(CharacterScriptableObject scriptable, Vector2 position)
    {
        this.CharacterInformation = scriptable;

        Vector2 vec = position;
        float depth = DynamicZDepth.GetDynamicZDepth(vec, DynamicZDepth.NPC_OFFSET);

        ObjectInScene = GameObject.Instantiate(ResourceManager.Instance.Character, new Vector3(position.x, position.y, depth), Quaternion.identity);
        ObjectTransform = ObjectInScene.transform;

        ObjectInScene.GetComponent<CharacterCustomizationLoader>().LoadCustomization(scriptable.CharacterCustomization);

        ObjectInScene.layer = LayerMask.NameToLayer("Interactable");
        ObjectInScene.tag = "NPC";
    }
}
