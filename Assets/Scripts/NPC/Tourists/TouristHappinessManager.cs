using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouristHappinessManager
{
    private TouristInstance touristInstance;
    private NPCStateMachine touristStateMachine;

    public TouristHappinessManager(TouristInstance touristInstance, NPCStateMachine touristStateMachine)
    {
        this.touristInstance = touristInstance;
        this.touristStateMachine = touristStateMachine;

        touristStateMachine.OnActivityCompleted += OnActivityCompletedHandler;
        touristInstance.OnNPCDelete += OnDelete;
    }

    public void OnActivityCompletedHandler(Activity activity, float completenessFrac)
    {
        if (touristInstance.IsInterestedInActivity(activity))
            touristInstance.happiness.ChangeHappiness(TouristHappinessFactor.CompleteInterestActivity, completenessFrac);
        else
            touristInstance.happiness.ChangeHappiness(TouristHappinessFactor.CompleteNonInterestActivity, completenessFrac);
    }

    private void OnDelete()
    {
        touristInstance.OnNPCDelete -= OnDelete;
        touristStateMachine.OnActivityCompleted -= OnActivityCompletedHandler;
    }
}
