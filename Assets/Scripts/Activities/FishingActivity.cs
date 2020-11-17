using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Activity/Fishing")]
public class FishingActivity : Activity
{
    [SerializeField] RegionInformation fishingRegionInformation = null;

    public override bool GetActivityLocationAndStateToSwitchTo(out Vector2Int? location, out Type switchToState, out object[] switchToStateArgs, out string goingToLocationMessage)
    {
        location = null;
        switchToState = null;
        switchToStateArgs = null;
        goingToLocationMessage = "";

        FishingRegionInstance targetRegion = (FishingRegionInstance)RegionManager.GetRandomRegionInstanceOfType(fishingRegionInformation);

        if (targetRegion == null)
        {
            //Debug.Log("No fishing region found");
            return false;
        }

        Vector2Int? randomFishingPosInRegion = targetRegion.GetRandomFishingPosition();

        if (randomFishingPosInRegion == null)
        {
            //Debug.Log("No valid fishing position found");
            return false;
        }

        //Will walk to position, then start fishing state
        location = randomFishingPosInRegion;
        switchToState = typeof(NPCFishingState);
        switchToStateArgs = null;
        goingToLocationMessage = "Going fishing";

        return true;
    }
}
