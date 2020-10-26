using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TouristDialogue
{
    private Dictionary<TouristDialogueType, Dialogue[]> dialogueChoices = new Dictionary<TouristDialogueType, Dialogue[]>();
    private int targetChoicesCountForEachType = 3;

    //Creates random tourist dialogue
    public TouristDialogue(TextAsset dialoguesFile)
    {
        JObject allDialoguesForPersonality = JObject.Parse(dialoguesFile.text);

        for (int i = 0; i < System.Enum.GetNames(typeof(TouristDialogueType)).Length; i++)
        {
            TouristDialogueType dType = (TouristDialogueType)i;

            try
            {
                if (allDialoguesForPersonality.TryGetValue(dType.ToString(), out JToken typeDialogueJson))
                {
                    List<Dialogue> typeDialogues = JsonConvert.DeserializeObject<List<Dialogue>>(typeDialogueJson.ToString());
                    //Dialogue dialogue = typeDialogues[Random.Range(0, typeDialogues.Count)];

                    //Number of choices for each type, need to make it smaller if not enough choices are defined in the dialogue file.
                    int choicesCount = (typeDialogues.Count < targetChoicesCountForEachType) ? typeDialogues.Count : targetChoicesCountForEachType;

                    System.Random rnd = new System.Random();
                    int[] myRndNos = Enumerable.Range(0, typeDialogues.Count).OrderBy(j => rnd.Next()).Take(choicesCount).ToArray();
                    Dialogue[] dialoguesForType = new Dialogue[myRndNos.Length];
                    foreach (int j in myRndNos)
                    {
                        dialoguesForType[j] = typeDialogues[j];
                    }
                    dialogueChoices.Add(dType, dialoguesForType);
                }
                else
                {
                    Debug.LogError("File has no dialogue for type " + dType.ToString());
                }
            }
            catch (System.Exception e)
            {
                Debug.Log(e.StackTrace);
                Debug.Log("Something went wrong parsing JSON!");
            }
        }
    }

    public Dialogue GetDialogue(TouristDialogueType type)
    {
        return dialogueChoices[type][UnityEngine.Random.Range(0, dialogueChoices[type].Length)];
    }

    public static TouristDialogueType GetDialogueTypeFromHappiness(TouristHappinessEnum happiness)
    {
        switch (happiness)
        {
            case (TouristHappinessEnum.VeryHappy):
                return TouristDialogueType.VeryHappy;
            case (TouristHappinessEnum.Happy):
                return TouristDialogueType.Happy;
            case (TouristHappinessEnum.Neutral):
                return TouristDialogueType.NeutralHappy;
            case (TouristHappinessEnum.Unhappy):
                return TouristDialogueType.Unhappy;
            case (TouristHappinessEnum.VeryUnhappy):
                return TouristDialogueType.VeryUnhappy;
            default:
                throw new System.Exception("Unknown happiness enum");
        }
    }
}

public enum TouristDialogueType
{
    Greeting,
    VeryHappy,
    Happy,
    NeutralHappy,
    Unhappy,
    VeryUnhappy,
    Fishing
}
