﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class TouristsGenerator : MonoBehaviour
{
    [SerializeField] private TouristInterest[] interestsList = null;

    [EnumNamedArray(typeof(TouristPersonality)), SerializeField]
    private TextAsset[] dialoguesFiles = null;
    private TextAsset GetDialoguesFile(TouristPersonality p) { return dialoguesFiles[(int)p]; }

    private static TouristsGenerator _instance;
    public static TouristsGenerator Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public TouristDialogue GenerateRandomDialogue(TouristInformation info)
    {
        return new TouristDialogue(GetDialoguesFile(info.Personality));
    }

    public TouristInterest[] GetRandomInterestsList()
    {
        int interestsCount = UnityEngine.Random.Range(2, 5); //2 to 4 interests

        System.Random rnd = new System.Random();
        int[] myRndNos = Enumerable.Range(0, interestsList.Length).OrderBy(j => rnd.Next()).Take(interestsCount).ToArray();

        TouristInterest[] interests = new TouristInterest[interestsCount];
        for (int i = 0; i < interestsCount; i++)
        {
            interests[i] = interestsList[myRndNos[i]];
        }

        return interests;
    }
}
