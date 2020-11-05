using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouristHappiness
{
    public int Value { get; private set; }

    public delegate void HappinessChanged(TouristHappinessFactor changeFactor, int newHappinessValue, TouristHappinessEnum newHappinessEnum);
    public event HappinessChanged OnHappinessChanged;

    public TouristHappiness()
    {
        Value = 60;
    }
    
    public TouristHappinessEnum GetTouristHappinessEnum()
    {
        if (Value < 30)
            return TouristHappinessEnum.VeryUnhappy;
        else if (Value < 50)
            return TouristHappinessEnum.Unhappy;
        else if (Value < 70)
            return TouristHappinessEnum.Neutral;
        else if (Value < 90)
            return TouristHappinessEnum.Happy;
        else 
            return TouristHappinessEnum.VeryHappy;
    }

    public void ChangeHappiness(TouristHappinessFactor changeFactor, float factorFrac = 1f)
    {
        int proposedValue = Value + Mathf.RoundToInt(changeFactor.value * factorFrac);

        if (proposedValue >= 100)
            Value = 100;
        else if (proposedValue <= 0)
            Value = 0;
        else
            Value = proposedValue;

        OnHappinessChanged?.Invoke(changeFactor, Value, GetTouristHappinessEnum());
    }
}

public class TouristHappinessFactor
{
    public readonly int value;

    private TouristHappinessFactor(int value) { this.value = value; }

    public static TouristHappinessFactor CompleteInterestActivity       => new TouristHappinessFactor(10);
    public static TouristHappinessFactor CompleteNonInterestActivity    => new TouristHappinessFactor(5);
    //public static TouristHappinessFactor Hungry                         => new TouristHappinessFactor(-5);
    //public static TouristHappinessFactor NoInterestedActivities         => new TouristHappinessFactor(-5);
    //public static TouristHappinessFactor Dirty                          => new TouristHappinessFactor(-5);
    //public static TouristHappinessFactor Tired                          => new TouristHappinessFactor(-5);
    //public static TouristHappinessFactor DisgustingFood                 => new TouristHappinessFactor(-5);
    //public static TouristHappinessFactor CaughtFish                     => new TouristHappinessFactor(7);
    //etc
} 

public enum TouristHappinessEnum
{
    VeryHappy,
    Happy,
    Neutral,
    Unhappy,
    VeryUnhappy
}