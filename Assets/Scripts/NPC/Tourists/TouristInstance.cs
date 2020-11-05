using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouristInstance: NPCInstance
{
    public TouristInformation TouristInformation => (TouristInformation)npcInformation;
    public readonly TouristDialogue dialogue;
    public readonly TouristInterest[] interests;
    public readonly TouristHappiness happiness;
    public readonly TouristSchedule schedule;

    private HashSet<Activity> interestedActivities;

    public TouristInstance(TouristInformation info, Transform touristTransform, TouristDialogue dialogue, TouristInterest[] interests, TouristHappiness happiness, TouristSchedule schedule): base(info, touristTransform)
    {
        this.dialogue = dialogue;
        this.interests = interests;
        this.happiness = happiness;
        this.schedule = schedule;

        interestedActivities = new HashSet<Activity>();
        foreach (TouristInterest i in interests)
        {
            foreach (Activity a in i.Activies)
            {
                if (!interestedActivities.Contains(a))
                    interestedActivities.Add(a);
            }
        }
    }

    public bool IsInterestedInActivity(Activity activity)
    {
        return interestedActivities.Contains(activity);
    }
}
