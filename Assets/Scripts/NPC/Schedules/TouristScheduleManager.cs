using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class TouristScheduleManager : NPCScheduleManager
{
    public readonly InGameTime sleepTime;
    public readonly InGameTime wakeTime;
    public readonly int sleepHours;
    public readonly int leaveDay;

    private InGameTime? nextSleepTime;
    private InGameTime? nextWakeTime;

    float minWaitTimeTillNextRefresh = 5f;
    float maxWaitTimeTillNextRefresh = 10f;

    private InGameTime? currentSubscribedUnloadingDockGobackTime = null;
    private InGameTime? currentSubscribedGoingToBedLocationTime = null;

    private readonly InGameTime boatLeaveTimeOnTouristLeaveDay;

    private Vector2Int? bedAccessLocation;

    private Coroutine refreshCoroutine;

    private readonly InGameTime BUFFER_TIME = new InGameTime(0, 10, 0);

    public TouristScheduleManager(InGameTime sleepTime, int sleepHours, int leaveDay, 
        NPCSchedule[] schedules, TouristComponents touristComponents): base(schedules, touristComponents)
    {
        this.sleepTime = sleepTime;
        this.wakeTime = sleepTime + new InGameTime(sleepHours, 0);
        this.sleepHours = sleepHours;
        this.leaveDay = leaveDay;

        InGameTime boatLeaveTimeRecurse = BoatManager.Instance.BoatLeaveTime;
        boatLeaveTimeOnTouristLeaveDay = new InGameTime(boatLeaveTimeRecurse.hour, boatLeaveTimeRecurse.minute, leaveDay);

        npcComponents.SubscribeToEvent(NPCInstanceEvent.Delete, OnNPCDeleteHandler);

        GetNextSleepCycleTimes();
        refreshCoroutine = Coroutines.Instance.StartCoroutine(RefreshGoBackTimes());

        npcComponents.SubscribeToEvent(NPCInstanceEvent.TryStartScheduleAction, OnNPCTryStartScheduleActionHandler);
    }


    //Runs when done dropping off luggage, done sleeping, done eating
    public override Type OnEndStateGetNextState(out object[] args)
    {
        InGameTime currentTime = TimeManager.Instance.GetCurrentTime();

        //Just spawned
        if (CurrentState == null)
        {
            args = null;
            return typeof(TouristCheckInSchedule);
        }
        //Could run when tourists are done dropping off luggage
        if (currentTime >= nextSleepTime)
        {
            RefreshBedAccessLocation();
            args = new object[] { bedAccessLocation, nextWakeTime };
            return typeof(NPCGoingToSleepSchedule);
        }
        //TODO: If tourist is hungry goto food schedule
        else if (false)
        {
            //Do somethings
        }
        else
        {
            args = null;
            return typeof(NPCActivitiesSchedule);
        }
    }

    IEnumerator RefreshGoBackTimes()
    {
        InGameTime? RefreshTimeToStartGoing(Vector2Int? targetPosition, InGameTime targetTime, InGameTime? currentSubscriptionTime, CustomEventGroup<InGameTime>.Delegate onTimeDelegate)
        {
            if (currentSubscriptionTime != null)
                TimeManager.Instance.UnsubscribeFromTime((InGameTime)currentSubscriptionTime, onTimeDelegate);

            if (targetPosition == null)
                return null;

            Vector2Int roundedPosition = new Vector2Int(Mathf.RoundToInt(npcComponents.npcTransform.position.x), Mathf.RoundToInt(npcComponents.npcTransform.position.y));
            InGameTime? timeToGoToTarget = AStar.EstimatePathTime(roundedPosition,
                                                                  (Vector2Int)targetPosition,
                                                                  npcComponents.moveSpeed,
                                                                  out LinkedList<Tuple<Vector2Int, Vector2Int?>> path);

            if (timeToGoToTarget == null)
            {
                Debug.Log("Tourist trapped! " + npcComponents.npcInformation.NpcName);
                return null;
            }

            InGameTime newTimeToGoBack = targetTime - (InGameTime)timeToGoToTarget;

            if (TimeManager.Instance.GetCurrentTime() >= newTimeToGoBack)
                onTimeDelegate(null);

            TimeManager.Instance.SubscribeToTime(newTimeToGoBack, onTimeDelegate);

            return newTimeToGoBack;
        }

        while (true)
        {
            InGameTime soonBeforeBoatLeaveTime = boatLeaveTimeOnTouristLeaveDay - BUFFER_TIME;
            Vector2Int unloadingDockPos = RegionManager.Instance.GetRandomRegionInstanceOfType(ResourceManager.Instance.BoatUnloadingRegion).GetRegionPositions()[0];
            currentSubscribedUnloadingDockGobackTime = RefreshTimeToStartGoing(unloadingDockPos, soonBeforeBoatLeaveTime, currentSubscribedUnloadingDockGobackTime, OnGoBackToUnloadingDockTimeHandler);

            InGameTime soonBeforeSleepTime = (InGameTime)nextSleepTime - BUFFER_TIME;
            RefreshBedAccessLocation();
            currentSubscribedGoingToBedLocationTime = RefreshTimeToStartGoing(bedAccessLocation, soonBeforeSleepTime, currentSubscribedGoingToBedLocationTime, OnGoBackToBedTimeHandler);

            //Randomness to stagger so that not all tourists will have this executed in same frame
            yield return new WaitForSeconds(UnityEngine.Random.Range(minWaitTimeTillNextRefresh, maxWaitTimeTillNextRefresh));
        }
    }

    private void RefreshBedAccessLocation()
    {
        bedAccessLocation = ((TouristComponents)npcComponents).GetRandomBedAccessLocation();
    }

    private void GetNextSleepCycleTimes()
    {
        if (nextWakeTime != null)
            TimeManager.Instance.UnsubscribeFromTime((InGameTime)nextWakeTime, OnWakeTimeHandler);

        // We need to do it this way, calculate wake time first then sleep time because
        // we don't know if sleep time will be the same day or the next day (could be like 1am tomorrow or 11pm today)
        nextWakeTime = new InGameTime(wakeTime.hour, 0, TimeManager.Instance.GetCurrentTime().day + 1);
        nextSleepTime = nextWakeTime - new InGameTime(sleepHours, 0, 0);

        TimeManager.Instance.SubscribeToTime((InGameTime)nextWakeTime, OnWakeTimeHandler);
    }


    private void OnGoBackToBedTimeHandler(object[] args)
    {
        //Use the same bed access location that was used to calculate time to get there,
        //Otherwise time could become inaccurate
        if (CurrentState.AllowTransitionToGoingToSleep)
            SwitchState<NPCGoingToSleepSchedule>(new object[] { bedAccessLocation, nextWakeTime });
    }

    private void OnGoBackToUnloadingDockTimeHandler(object[] args)
    {
        SwitchState<NPCLeavingSchedule>();
    }

    private void OnWakeTimeHandler(object[] args)
    {
        GetNextSleepCycleTimes();
    }

    private void OnNPCDeleteHandler(object[] args)
    {
        npcComponents.UnsubscribeToEvent(NPCInstanceEvent.Delete, OnNPCDeleteHandler);
        npcComponents.UnsubscribeToEvent(NPCInstanceEvent.TryStartScheduleAction, OnNPCTryStartScheduleActionHandler);

        TimeManager.Instance.UnsubscribeFromTime((InGameTime)nextWakeTime, OnWakeTimeHandler);

        Coroutines.Instance.StopCoroutine(refreshCoroutine);
    }

    private void OnNPCTryStartScheduleActionHandler(object[] args)
    {
        CurrentState.TryStartScheduleAction();
    }
}