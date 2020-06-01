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
    [SerializeField] private Text timeText = null;

    private const int morningStart = 5;
    private const int midDayStart = 10;
    private const int eveningStart = 17;
    private const int nightStart = 20;

    public delegate void OnTurnedMorning();
    public static event OnTurnedMorning onTurnedMorning;

    public delegate void OnTurnedMidDay();
    public static event OnTurnedMidDay onTurnedMidDay;

    public delegate void OnTurnedEvening();
    public static event OnTurnedEvening onTurnedEvening;

    public delegate void OnTurnedNight();
    public static event OnTurnedNight onTurnedNight;

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

            timeText.text = "Day " + day + "  " + hourString + ":" + minuteString; //TODO seperate canvases to increase performance
        }

        if (hour != previousHour)
            HourChanged();

        previousHour = hour;

    }

    private void HourChanged()
    {
        switch (hour) {
            case (morningStart):
                onTurnedMorning?.Invoke();
                break;

            case (midDayStart):
                onTurnedMidDay?.Invoke();
                break;

            case (eveningStart):
                onTurnedEvening?.Invoke();
                break;

            case (nightStart):
                onTurnedNight?.Invoke();
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
}

public enum TimeOfDay
{
    Morning,
    MidDay,
    Evening,
    Night
}
