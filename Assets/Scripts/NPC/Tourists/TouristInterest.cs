using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Interest")]
public class TouristInterest: ScriptableObject
{
    [SerializeField] private string interestName = "";
    public string InterestName => interestName;

    [SerializeField] private Sprite icon = null;
    public Sprite Icon => icon;

    [SerializeField] private Activity[] activities = null;
    public Activity[] Activies => activities;
}