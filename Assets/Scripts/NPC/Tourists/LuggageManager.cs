using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuggageManager
{
    private TouristComponents touristComponents;
    private Transform luggageInstance;

    public LuggageManager(TouristComponents touristComponents)
    {
        this.touristComponents = touristComponents;

        touristComponents.SubscribeToEvent(NPCInstanceEvent.DropLuggage, OnDropLuggageHandler);
        touristComponents.SubscribeToEvent(NPCInstanceEvent.Delete, OnDeleteHandler);

        luggageInstance = GameObject.Instantiate(ResourceManager.Instance.Prefab_luggage).transform;
        Transform pivot = null;

        foreach (Transform t in touristComponents.npcTransform)
        {
            if (t.tag == "Pivot")
                pivot = t;
        }

        luggageInstance.SetParent(pivot);
        luggageInstance.SetAsLastSibling();
        luggageInstance.transform.localPosition = new Vector2(-0.25f, -0.21875f);
    }

    private void OnDropLuggageHandler(object[] args)
    {
        luggageInstance.SetParent(null);
        luggageInstance.transform.position = new Vector3(touristComponents.npcTransform.position.x,
                                                         touristComponents.npcTransform.position.y,
                                                         DynamicZDepth.GetDynamicZDepth(touristComponents.npcTransform.position.y, DynamicZDepth.LUGGAGE_OFFSET));
    }

    private void OnDeleteHandler(object[] args)
    {
        touristComponents.UnsubscribeToEvent(NPCInstanceEvent.DropLuggage, OnDropLuggageHandler);
        touristComponents.UnsubscribeToEvent(NPCInstanceEvent.Delete, OnDeleteHandler);

        GameObject.Destroy(luggageInstance.gameObject);
    }
}
