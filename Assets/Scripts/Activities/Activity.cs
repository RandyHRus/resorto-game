using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Activity : ScriptableObject { }

public abstract class Activity<T>: Activity where T: ActivityState
{
    [SerializeField] private string activityName = "";
    public string ActivityName => activityName;

    [SerializeField] private Sprite icon = null;
    public Sprite Icon => icon;
}

[CreateAssetMenu(menuName = "Activity/Fishing")]
public class FishingActivity : Activity<FishingActivityState> { }