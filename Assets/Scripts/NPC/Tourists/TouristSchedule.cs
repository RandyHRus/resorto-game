using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouristSchedule
{
    public delegate void ScheduleSignalDelegate();
    public event ScheduleSignalDelegate OnWakeTime;
    public event ScheduleSignalDelegate OnSleepTime;

    private int wakeTime;
    private int sleepTime;

    public TouristSchedule(int wakeTime, int sleepTime)
    {
        this.wakeTime = wakeTime;
        this.sleepTime = sleepTime;

        TimeManager.Instance.GetHourEvent(wakeTime).OnTournedHour += InvokeOnWakeTime;
        TimeManager.Instance.GetHourEvent(sleepTime).OnTournedHour += InvokeOnSleepTime;
    }

    private void InvokeOnWakeTime()
    {
        OnWakeTime?.Invoke();
    }

    private void InvokeOnSleepTime()
    {
        OnSleepTime?.Invoke();
    }
}
