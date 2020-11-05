using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouristSchedule
{
   // public delegate void ScheduleSignalDelegate();
    //public event ScheduleSignalDelegate OnWakeTime;
    //public event ScheduleSignalDelegate OnSleepTime;


    private InGameTime wakeTime;
    private InGameTime sleepTime;
    private int leaveDay;

    public TouristSchedule(InGameTime wakeTime, InGameTime sleepTime, int leaveDay)
    {
        this.wakeTime = wakeTime;
        this.sleepTime = sleepTime;
        this.leaveDay = leaveDay;

        //TimeManager.Instance.SubScribeToTime(wakeTime, InvokeOnWakeTime);
        //TimeManager.Instance.SubScribeToTime(sleepTime, InvokeOnSleepTime);
    }

    //private void InvokeOnWakeTime()
    //{
    //    OnWakeTime?.Invoke();
    //}

    //private void InvokeOnSleepTime()
    //{
    //    OnSleepTime?.Invoke();
    //}
}
