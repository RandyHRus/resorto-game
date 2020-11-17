using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TouristSchedule: NPCSchedule
{
    public readonly InGameTime nextWakeTime;
    public readonly InGameTime nextSleepTime;

    public readonly int leaveDay;
    private readonly InGameTime boatLeaveTimeOnTouristLeaveDay;

    private InGameTime? currentSubscribedUnloadingDockGobackTime = null;
    private InGameTime? currentSubscribedGoingToBedLocationTime = null;

    public delegate void OnTimeDelegate();
    public event OnTimeDelegate OnGoBackToUnloadingDockTime;

    public delegate void GoToBedLocationTime(Vector2Int bedAccessLocation);
    public event GoToBedLocationTime OnGoToBedLocationTime;

    float minWaitTimeTillNextRefresh = 20f;
    float lastRefreshTime = -1;

    private Vector2Int? bedAccessLocation;

    private readonly InGameTime BUFFER_TIME = new InGameTime(0, 10, 0);

    public TouristSchedule(TouristInstance touristInstance, InGameTime sleepTime, int sleepHours, int leaveDay): base(touristInstance)
    {
        nextSleepTime = new InGameTime(sleepTime.hour, sleepTime.minute, TimeManager.Instance.GetCurrentTime().day);
        nextWakeTime  = nextSleepTime + new InGameTime(sleepHours, 0, 0);

        this.leaveDay = leaveDay;

        //TimeManager.Instance.SubScribeToTime(wakeTime, InvokeOnWakeTime);
        //TimeManager.Instance.SubScribeToTime(sleepTime, InvokeOnSleepTime);

        InGameTime boatLeaveTimeRecurse = BoatManager.Instance.BoatLeaveTime;
        boatLeaveTimeOnTouristLeaveDay = new InGameTime(boatLeaveTimeRecurse.hour, boatLeaveTimeRecurse.minute, leaveDay);

        RefreshGoBackTimes();

        npcInstance.OnNPCStateChanged += OnNPCStateChangedHandler;
        npcInstance.OnNPCDelete += OnNPCDeleteHandler;
    }

    private void OnNPCDeleteHandler()
    {
        npcInstance.OnNPCStateChanged -= OnNPCStateChangedHandler;
        npcInstance.OnNPCDelete -= OnNPCDeleteHandler;
    }

    private void OnNPCStateChangedHandler(NPCState previousState, NPCState newState)
    {
        RefreshGoBackTimes();
    }

    private void RefreshGoBackTimes()
    {
        //We don't want this executing too often. cause it takes a lot of resources to calculate path.
        if (lastRefreshTime != -1 && Time.time - lastRefreshTime < minWaitTimeTillNextRefresh)
        {
            return;
        }

        lastRefreshTime = Time.time;

        InGameTime soonBeforeBoatLeaveTime = boatLeaveTimeOnTouristLeaveDay - BUFFER_TIME;
        Vector2Int unloadingDockPos = RegionManager.GetRandomRegionInstanceOfType(ResourceManager.Instance.BoatUnloadingRegion).GetRegionPositionsAsList()[0];
        currentSubscribedUnloadingDockGobackTime = RefreshTimeToStartGoing(unloadingDockPos, soonBeforeBoatLeaveTime, currentSubscribedUnloadingDockGobackTime, InvokeOnGoBackToUnloadingDockTime);

        InGameTime soonBeforeSleepTime = nextSleepTime - BUFFER_TIME;
        bedAccessLocation = ((TouristInstance)npcInstance).GetRandomBedAccessLocation();
        currentSubscribedGoingToBedLocationTime = RefreshTimeToStartGoing(bedAccessLocation, soonBeforeSleepTime, currentSubscribedGoingToBedLocationTime, InvokeOnGoBackToBedLocationTime);
    }

    private InGameTime? RefreshTimeToStartGoing(Vector2Int? targetPosition, InGameTime targetTime, InGameTime? currentSubscriptionTime, CustomEventGroup<InGameTime>.Delegate onTimeDelegate)
    {
        if (currentSubscriptionTime != null)
            TimeManager.Instance.UnsubscribeFromTime((InGameTime)currentSubscriptionTime, onTimeDelegate);

        if (targetPosition == null)
            return null;

        Vector2Int roundedPosition = new Vector2Int(Mathf.RoundToInt(npcInstance.npcTransform.position.x), Mathf.RoundToInt(npcInstance.npcTransform.position.y));
        InGameTime? timeToGoToTarget = AStar.EstimatePathTime(roundedPosition,
                                                              (Vector2Int)targetPosition,
                                                              npcInstance.moveSpeed,
                                                              out LinkedList<Tuple<Vector2Int, Vector2Int?>> path);

        if (timeToGoToTarget == null)
        {
            Debug.Log("Tourist trapped!");
            return null;
        }

        InGameTime newTimeToGoBack = targetTime - (InGameTime)timeToGoToTarget;

        if (TimeManager.Instance.GetCurrentTime() >= newTimeToGoBack)
            onTimeDelegate(null);
        else
            TimeManager.Instance.SubscribeToTime(newTimeToGoBack, onTimeDelegate);

        return newTimeToGoBack;
    }

    private void InvokeOnGoBackToUnloadingDockTime(object[] args)
    {
        OnGoBackToUnloadingDockTime?.Invoke();
    }

    private void InvokeOnGoBackToBedLocationTime(object[] args)
    {
        OnGoToBedLocationTime?.Invoke((Vector2Int)bedAccessLocation);
    }
}
