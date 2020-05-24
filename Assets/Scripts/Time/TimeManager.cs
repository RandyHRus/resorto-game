using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private float timeSpeed = 2;
    public int day;
    public int hour;
    public int minute;
    private float timePassed;
    [SerializeField] private Text timeText = null;

    private int dayStartHour = 7;
    private int nightStartHour = 19;


    public delegate void OnTurnedDay();
    public static event OnTurnedDay onTurnedDay;

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


    int previousHour;
    int previousMinute;

    private void Update()
    {

        //Change time
        {
            timePassed += Time.deltaTime * timeSpeed;
            if (timePassed > 1)
            {
                minute += 1;
                timePassed -= 1;

                //Reset timer if lag
                if (timePassed > 1)
                {
                    timePassed = 0;
                }

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

        //Send events
        if (hour == nightStartHour && previousHour != nightStartHour)
        {
            onTurnedNight?.Invoke();
        }
        else if (hour == dayStartHour && previousHour != dayStartHour)
        {
            onTurnedDay?.Invoke();
        }

        previousHour = hour;
        previousMinute = minute;
    }

    public bool IsDay()
    {
        return (hour >= dayStartHour && hour < nightStartHour);
    }
}
