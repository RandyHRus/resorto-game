using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouristHappinessChangeDisplay
{
    private static readonly int initialPoolCount = 3;
    private static readonly float floatUpSpeed = 1f;
    private static readonly float fadeSpeed = 1f;

    private Vector3 startingOffset = new Vector3(0, 2);

    private readonly TouristComponents touristComponents;

    private Queue<OutlinedText> displayPool;

    public TouristHappinessChangeDisplay(TouristComponents touristComponents)
    {
        this.touristComponents = touristComponents;

        touristComponents.happiness.OnHappinessChanged += OnHappinessChangedHandler;
        touristComponents.SubscribeToEvent(NPCInstanceEvent.Delete, OnDeleteHandler);

        displayPool = new Queue<OutlinedText>();
        for (int i = 0; i < initialPoolCount; i++)
        {
            displayPool.Enqueue(CreateNewOutlinedText());
        }
    }

    private void OnDeleteHandler(object[] args)
    {
        touristComponents.happiness.OnHappinessChanged -= OnHappinessChangedHandler;
        touristComponents.UnsubscribeToEvent(NPCInstanceEvent.Delete, OnDeleteHandler);

        while (displayPool.Count > 0)
        {
            OutlinedText text = displayPool.Dequeue();
            text.Destroy();
        }
        displayPool = null;
    }

    private void OnHappinessChangedHandler(TouristHappinessFactor changeFactor, int newHappinessValue, TouristHappinessEnum newHappinessEnum)
    {
        OutlinedText textToShow;

        if (displayPool.Count == 0)
            textToShow = CreateNewOutlinedText();
        else
            textToShow = displayPool.Dequeue();


        textToShow.SetColor(changeFactor.value > 0 ? ResourceManager.Instance.Green : ResourceManager.Instance.Red);
        textToShow.SetText(changeFactor.value > 0 ? "+" + changeFactor.value : changeFactor.value.ToString());
        textToShow.ObjectTransform.position = touristComponents.npcTransform.position + startingOffset;

        Coroutines.Instance.StartCoroutine(FloatUpAndFadeDisplay(textToShow));
    }

    private IEnumerator FloatUpAndFadeDisplay(OutlinedText outlinedText)
    {
        float alpha = 1;
        float currentYOffset = 0;

        outlinedText.ObjectInScene.SetActive(true);

        while (alpha >= 0)
        {
            //Change alpha
            alpha -= Time.deltaTime * fadeSpeed;
            outlinedText.SetAlpha(alpha);

            //Change position
            currentYOffset += (floatUpSpeed * Time.deltaTime);
            outlinedText.ObjectTransform.position = touristComponents.npcTransform.position + startingOffset + new Vector3(0, currentYOffset);

            yield return 0;
        }

        outlinedText.ObjectInScene.SetActive(false);

        //If npc is deleted, displayPool will be set to null
        if (displayPool == null)
            outlinedText.Destroy(); 
        else
            displayPool.Enqueue(outlinedText);
    }

    private OutlinedText CreateNewOutlinedText()
    {
        OutlinedText newText = new OutlinedText(ResourceManager.Instance.IndicatorsCanvas.transform);
        newText.ObjectTransform.localScale = new Vector3Int(2, 2, 2);
        newText.ObjectInScene.SetActive(false);
        return newText;
    }
} 
