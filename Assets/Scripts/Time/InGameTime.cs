using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InGameTime
{
    public readonly int day;
    public readonly int hour;
    public readonly int minute;

    public InGameTime(int hour, int minute, int day = -1)
    {
        this.day = day;
        this.hour = hour;
        this.minute = minute;
    }

    public bool IsRecurring()
    {
        return day == -1;
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
            return this.day == ((InGameTime)obj).hour &&
                   this.hour == ((InGameTime)obj).hour &&
                   this.minute == ((InGameTime)obj).minute;
        }
    }
}
