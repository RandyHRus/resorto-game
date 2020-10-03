using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouristEnjoyment
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class TouristEnjoymentFactor {

    public readonly int value;

    private TouristEnjoymentFactor(int value) { this.value = value; }

    public static TouristEnjoymentFactor CompleteInterestActivity       => new TouristEnjoymentFactor(30);
    public static TouristEnjoymentFactor CompleteNonInterestActivity    => new TouristEnjoymentFactor(10);
    public static TouristEnjoymentFactor Hungry                         => new TouristEnjoymentFactor(-15);
    public static TouristEnjoymentFactor NoInterestedActivities         => new TouristEnjoymentFactor(-15);
    public static TouristEnjoymentFactor Dirty                          => new TouristEnjoymentFactor(-15);
    public static TouristEnjoymentFactor Tired                          => new TouristEnjoymentFactor(-15);
    public static TouristEnjoymentFactor DisgustingFood                 => new TouristEnjoymentFactor(-15);
    public static TouristEnjoymentFactor CaughtFish                     => new TouristEnjoymentFactor(10);
    //etc
} 