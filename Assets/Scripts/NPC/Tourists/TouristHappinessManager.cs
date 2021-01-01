using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouristHappinessManager
{
    private TouristComponents touristComponents;
    private NPCStateMachine touristStateMachine;

    public TouristHappinessManager(TouristComponents touristComponents, NPCStateMachine touristStateMachine)
    {
        this.touristComponents = touristComponents;
        this.touristStateMachine = touristStateMachine;

        touristStateMachine.OnActivityCompleted += OnActivityCompletedHandler;
        touristComponents.SubscribeToEvent(NPCInstanceEvent.Delete, OnDeleteHandler);
    }

    public void OnActivityCompletedHandler(Activity activity, float completenessFrac)
    {
        if (touristComponents.IsInterestedInActivity(activity))
            touristComponents.happiness.ChangeHappiness(TouristHappinessFactor.CompleteInterestActivity, completenessFrac);
        else
            touristComponents.happiness.ChangeHappiness(TouristHappinessFactor.CompleteNonInterestActivity, completenessFrac);
    }

    private void OnDeleteHandler(object[] args)
    {
        touristComponents.UnsubscribeToEvent(NPCInstanceEvent.Delete, OnDeleteHandler);
        touristStateMachine.OnActivityCompleted -= OnActivityCompletedHandler;
    }
}
