using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouristHappinessManager
{
    private TouristInstance touristInstance;

    public TouristHappinessManager(TouristInstance touristInstance, NPCStateMachine touristStateMachine)
    {
        this.touristInstance = touristInstance;

        touristStateMachine.OnActivityCompleted += OnActivityCompletedHandler;
    }

    public void OnActivityCompletedHandler(Activity activity, float completenessFrac)
    {
        if (touristInstance.IsInterestedInActivity(activity))
            touristInstance.happiness.ChangeHappiness(TouristHappinessFactor.CompleteInterestActivity, completenessFrac);
        else
            touristInstance.happiness.ChangeHappiness(TouristHappinessFactor.CompleteNonInterestActivity, completenessFrac);
    }
}
