using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TimeManager : MonoBehaviour
{
    [SerializeField] public float timeSpeed = 5;
    [SerializeField] private int day;
    [SerializeField] private int hour;
    [SerializeField] private int minute;
    private float timePassed;
    [SerializeField] private GameObject timeTextObj = null;

    private OutlinedText text;

    private const int morningStart = 5;
    private const int midDayStart = 10;
    private const int eveningStart = 17;
    private const int nightStart = 20;

    public delegate void OnTurnedTimeOfDayDelegate();
    public static event OnTurnedTimeOfDayDelegate OnTurnedMorning;
    public static event OnTurnedTimeOfDayDelegate OnTurnedMidDay;
    public static event OnTurnedTimeOfDayDelegate OnTurnedEvening;
    public static event OnTurnedTimeOfDayDelegate OnTurnedNight;

    private static TimeManager _instance;
    public static TimeManager Instance { get { return _instance; } }

    private CustomEventManager<InGameTime> onTurnedTimeEventManager = new CustomEventManager<InGameTime>();

    private InGameTime previousTime = default;

    private void Awake()
    {
        //Singleton
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
        {
            timePassed = 0;
        }
        {
            text = new OutlinedText(timeTextObj);
        }
    }

    private int previousHour = -1;

    private void Update()
    {
        //Change time
        {
            timePassed += Time.deltaTime * timeSpeed;

            //We don't want it to skip any minutes, (Could messs with subscriptions
            //so even if timePassed is greater than 2, it will only calculate for 1.
            if (timePassed >= 1)
            {
                minute += 1;
                timePassed = 0;

                if (minute >= 60)
                {
                    minute = 0;
                    hour += 1;

                    if (hour >= 24)
                    {
                        hour = 0;
                        day += 1;
                    }
                }
            }
        }

        InGameTime gameTime = new InGameTime(hour, minute, day);
        InGameTime recurringGameTime = new InGameTime(hour, minute);

        if (!previousTime.Equals(gameTime))
        {
            onTurnedTimeEventManager.TryInvokeEventGroup(gameTime, null);
            onTurnedTimeEventManager.TryInvokeEventGroup(recurringGameTime, null);
            previousTime = gameTime;
        }

        //Draw on UI
        string timeText = GetTimeText(day, hour, minute);
        text.SetText(timeText);

        if (hour != previousHour)
            HourChanged();

        previousHour = hour;
    }

    public void SubscribeToTime(InGameTime time, CustomEventGroup<InGameTime>.Delegate eventHandler)
    {
        onTurnedTimeEventManager.Subscribe(time, eventHandler);
    }

    public void UnsubscribeFromTime(InGameTime time, CustomEventGroup<InGameTime>.Delegate eventHandler)
    {
        onTurnedTimeEventManager.Unsubscribe(time, eventHandler);
    }

    private void HourChanged()
    {
        //GetTimeEvent(HourChanged).InvokeTurnedHourEvent();

        switch (hour) {
            case (morningStart):
                OnTurnedMorning?.Invoke();
                Debug.Log("Morning");
                break;

            case (midDayStart):
                OnTurnedMidDay?.Invoke();
                Debug.Log("Mid day");
                break;

            case (eveningStart):
                OnTurnedEvening?.Invoke();
                Debug.Log("Evening");
                break;

            case (nightStart):
                OnTurnedNight?.Invoke();
                Debug.Log("Night");
                break;
        }
    }

    public TimeOfDay GetTimeOfDay()
    {
        if (hour >= morningStart && hour < midDayStart)
            return TimeOfDay.Morning;
        else if (hour >= midDayStart && hour < eveningStart)
            return TimeOfDay.MidDay;
        else if (hour >= eveningStart && hour < nightStart)
            return TimeOfDay.Evening;
        else
            return TimeOfDay.Night;
    }

    public InGameTime GetCurrentTime()
    {
        return new InGameTime(hour, minute, day);
    }

    //Day is optional
    //If day is set to -1, event will trigger every day


    private string GetTimeText(int _day, int _hour, int _minute)
    {
        string hourString = _hour.ToString();
        if (_hour < 10)
            hourString = "0" + hourString;

        string minuteString = _minute.ToString();
        if (_minute < 10)
            minuteString = "0" + minuteString;

        return "Day " + _day + "  " + hourString + ":" + minuteString;
    }
}

public enum TimeOfDay
{
    Morning,
    MidDay,
    Evening,
    Night
}