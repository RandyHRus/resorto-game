using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouristDialogue : MonoBehaviour, INPCDialogue
{
    public string Name { get; set; }
    private TextAsset dialoguesFile;

    private TouristRelationship relationshipComponent;

    public void Initialize(string name, TextAsset dialoguesFile)
    {
        Name = name;
        this.dialoguesFile = dialoguesFile;

        relationshipComponent = GetComponent<TouristRelationship>();
    }

    public bool GetStandardDialogue(out Dialogue dialogue)
    {
        dialogue = null;
        try
        {
            JObject dialogues = JObject.Parse(dialoguesFile.text);
            if (dialogues.TryGetValue(relationshipComponent.relationshipLevel.ToString(), out JToken relationshipDialoguesJson))
            {
                List<Dialogue> relationshipDialogues = JsonConvert.DeserializeObject<List<Dialogue>>(relationshipDialoguesJson.ToString());
                dialogue = relationshipDialogues[Random.Range(0, relationshipDialogues.Count)];
                return true;
            }
            else
            {
                Debug.Log("NPC has no dialogue for relationship level: " + relationshipComponent.relationshipLevel.ToString());
                return false;
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.StackTrace);
            Debug.Log("Something went wrong parsing JSON!");
            return false;
        }
    }
}
