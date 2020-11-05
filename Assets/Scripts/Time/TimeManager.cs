using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TimeManager : MonoBehaviour
{
    public readonly float timeSpeed = 2;
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

    private Dictionary<InGameTime, TurnedToTimeEvent> turnedTimeEvents  = new Dictionary<InGameTime, TurnedToTimeEvent>();

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

        InGameTime gameTime = new InGameTime(hour, minute);
        InGameTime recurringGameTime = new InGameTime(hour, minute, day);

        if (turnedTimeEvents.TryGetValue(gameTime, out TurnedToTimeEvent timeEvent))
        {
            timeEvent.InvokeEvent();
        }
        if (turnedTimeEvents.TryGetValue(recurringGameTime, out TurnedToTimeEvent timeEvent2))
        {
            timeEvent2.InvokeEvent();
        }

        //Draw on UI
        string timeText = GetTimeText(day, hour, minute);
        text.SetText(timeText);

        if (hour != previousHour)
            HourChanged();

        previousHour = hour;
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
    public void SubScribeToTime(InGameTime gameTime, TurnedToTimeEvent.TurnedTime onTurnedTimeDelegate)
    {
        void RemoveTurnedTimeEvent(InGameTime timeEventKey)
        {
            if (!turnedTimeEvents.TryGetValue(gameTime, out TurnedToTimeEvent senderEvent))
            {
                throw new System.Exception("Key not found");
            }

            senderEvent.OnSubscribersEmpty -= RemoveTurnedTimeEvent;
            turnedTimeEvents.Remove(timeEventKey);
        }

        if (turnedTimeEvents.TryGetValue(gameTime, out TurnedToTimeEvent timeEvent))
        {
            timeEvent.AddSubscriber(onTurnedTimeDelegate);
        }
        else
        {
            TurnedToTimeEvent newTimeEvent = new TurnedToTimeEvent(gameTime, onTurnedTimeDelegate);
            turnedTimeEvents.Add(gameTime, newTimeEvent);
            newTimeEvent.OnSubscribersEmpty += RemoveTurnedTimeEvent;
        }

    }

    public void UnsubscribeFromTime(InGameTime gameTime, TurnedToTimeEvent.TurnedTime onTurnedTimeDelegate)
    {
        if (!turnedTimeEvents.TryGetValue(gameTime, out TurnedToTimeEvent timeEvent))
        {
            throw new System.Exception("Key not found");
        }

        timeEvent.RemoveSubscriber(onTurnedTimeDelegate);
    }

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

public class TurnedToTimeEvent
{
    public delegate void TurnedTime();
    public event TurnedTime OnTurnedTime;

    public delegate void SubscribersEmpty(InGameTime timeEventKey);
    public event SubscribersEmpty OnSubscribersEmpty;

    private HashSet<TurnedTime> delegates;
    private InGameTime key;

    public TurnedToTimeEvent(InGameTime key, TurnedTime initialSubscriber)
    {
        this.key = key;

        delegates = new HashSet<TurnedTime>();
        delegates.Add(initialSubscriber);
    }

    public void AddSubscriber(TurnedTime subscriber)
    {
        delegates.Add(subscriber);
    }

    public void RemoveSubscriber(TurnedTime subscriber)
    {
        if (!delegates.Remove(subscriber))
            throw new System.Exception("Nothing to remove!");

        if (delegates.Count <= 0)
            OnSubscribersEmpty?.Invoke(key);
    }

    public void InvokeEvent()
    {
        foreach (TurnedTime d in delegates)
        {
            d.Invoke();
        }
    }
}