using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Activity : ScriptableObject
{
    [SerializeField] private string activityName = "";
    public string ActivityName => activityName;

    [SerializeField] private Sprite icon = null;
    public Sprite Icon => icon;

    public abstract bool GetActivityLocationAndStateToSwitchTo(out Vector2Int? location, out Type switchToState, out object[] switchToStateArgs, out string goingToLocationMessage);
}