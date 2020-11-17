using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InGameTime
{
    public readonly int day;
    public readonly int hour;
    public readonly int minute;

    //Set hour and minutes for recurring time (Each day)
    //Set hour, minutes, and day for set time
    public InGameTime(int hour, int minute, int day = int.MinValue)
    {
        if (hour > 23 || hour < 0)
            throw new System.Exception("Invalid hour " + hour);

        if (minute > 59 || minute < 0)
            throw new System.Exception("Invalid minute " + minute);

        this.day = day;
        this.hour = hour;
        this.minute = minute;
    }

    //Create specific time from minutes
    //Ex. 70 minutes -> 1 hour, 10 minute
    public InGameTime(int minutes)
    {
        this.day = (int)Mathf.Floor(minutes / 1440);
        this.hour = (int)Mathf.Floor(minutes / 60) % 24;
        this.minute = minutes % 60;
    }

    public bool IsRecurring()
    {
        return day == int.MinValue;
    }

    public override int GetHashCode() => (day, hour, minute).GetHashCode();

    public override bool Equals(object obj)
    {
        //Check for null and compare run-time types.
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            return this.day == ((InGameTime)obj).day &&
                   this.hour == ((InGameTime)obj).hour &&
                   this.minute == ((InGameTime)obj).minute;
        }
    }

    public static InGameTime operator +(InGameTime t1, InGameTime t2)
    {
        int minute = (int)MathFunctions.Mod(t1.minute + t2.minute, 60);

        int unModedHour = t1.hour + t2.hour + (t1.minute + t2.minute >= 60 ? 1 : 0);
        int hour = (int)MathFunctions.Mod(unModedHour, 24);
        int day = (t1.IsRecurring() || t2.IsRecurring()) ? int.MinValue : 
                                                         t1.day + t2.day + (unModedHour >= 24 ? 1 : 0);

        return new InGameTime(hour, minute, day);
    }

    public static InGameTime operator -(InGameTime t1, InGameTime t2)
    {
        int minute = (int)MathFunctions.Mod(t1.minute - t2.minute, 60);

        int unModedHour = t1.hour - t2.hour + (t1.minute - t2.minute < 0 ? -1 : 0);
        int hour = (int)MathFunctions.Mod(unModedHour, 24);
        int day = (t1.IsRecurring() || t2.IsRecurring()) ? int.MinValue :
                                                         t1.day - t2.day + (unModedHour < 0 ? -1 : 0);

        return new InGameTime(hour, minute, day);
    }

    public static bool operator <(InGameTime t1, InGameTime t2)
    {
        if (t1.IsRecurring() || t2.IsRecurring())
            throw new System.Exception("Cannot compare recurring times");

        return (t1.day < t2.day) ? true : 
                                   (t1.day == t2.day) ?
                                        (t1.hour < t2.hour) ? 
                                        true : 
                                        (t1.hour == t2.hour) ?
                                            (t1.minute < t2.minute): 
                                            false :
                                        false;
    }

    public static bool operator <=(InGameTime t1, InGameTime t2)
    {
        return (t1 < t2 || t1.Equals(t2));
    }

    public static bool operator >(InGameTime t1, InGameTime t2)
    {
        if (t1.IsRecurring() || t2.IsRecurring())
            throw new System.Exception("Cannot compare recurring times");

        return (t1.day > t2.day) ? true :
                                   (t1.day == t2.day) ?
                                        (t1.hour > t2.hour) ?
                                        true :
                                        (t1.hour == t2.hour) ?
                                            (t1.minute > t2.minute) :
                                            false :
                                        false;
    }

    public static bool operator >=(InGameTime t1, InGameTime t2)
    {
        return (t1 > t2 || t1.Equals(t2));
    }
}
