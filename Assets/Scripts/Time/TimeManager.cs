using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private float timeSpeed = 0;
    public int day;
    public int hour;
    public int minute;
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
    private TurnedHourEvent[] hourEvents;

    private static TimeManager _instance;
    public static TimeManager Instance { get { return _instance; } }

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
        {
            hourEvents = new TurnedHourEvent[24];
            for (int i = 0; i < 24; i++)
            {
                hourEvents[i] = new TurnedHourEvent();
            }
        }
    }

    private int previousHour = -1;

    private void Update()
    {

        //Change time
        {
            timePassed += Time.deltaTime * timeSpeed;
            while (timePassed > 1)
            {
                minute += 1;
                timePassed -= 1;

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

        //Draw on UI
        {
            string hourString = hour.ToString();
            if (hour < 10)
                hourString = "0" + hourString;

            string minuteString = minute.ToString();
            if (minute < 10)
                minuteString = "0" + minuteString;

            text.SetText("Day " + day + "  " + hourString + ":" + minuteString);
        }

        if (hour != previousHour)
            HourChanged();

        previousHour = hour;

    }

    private void HourChanged()
    {
        GetHourEvent(hour).InvokeTurnedHourEvent();

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

    public TurnedHourEvent GetHourEvent(int hour)
    {
        //Debug.Log(hour);
        return hourEvents[hour];
    }
}

public enum TimeOfDay
{
    Morning,
    MidDay,
    Evening,
    Night
}

public class TurnedHourEvent
{
    public delegate void TurnedHour();
    public event TurnedHour OnTournedHour;

    public void InvokeTurnedHourEvent()
    {
        OnTournedHour?.Invoke();
    }
}