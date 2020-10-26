﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Activity/Fishing")]
public class FishingActivity : Activity
{
    [SerializeField] RegionInformation fishingRegionInformation = null;

    public override bool CanStartActivity(out Type switchToState, out object[] switchToStateArgs)
    {
        FishingRegionInstance targetRegion = (FishingRegionInstance)RegionManager.GetRandomRegionInstanceOfType(fishingRegionInformation);

        switchToState = null;
        switchToStateArgs = null;

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

        switchToState = typeof(NPCWalkToPositionState);
        switchToStateArgs = new object[] { randomFishingPosInRegion, typeof(NPCFishingState), null, "Going fishing" };

        return true;
    }
}