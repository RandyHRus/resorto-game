using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouristBoat : Boat
{
    private Vector2Int touristSpawnPosition;
    private int touristCount;

    public override void Initialize()
    {
        base.Initialize();

        OnBoatUnloadingPointReached += SpawnTourists;
        touristSpawnPosition = boatUnloadingRegionInstance.GetRegionPositions()[0];
    }

    private void SpawnTourists()
    {
        for (int i = 0; i < touristCount; i++)
            NPCManager.Instance.CreateTourist(TouristInformation.CreateRandomTouristInformation(), touristSpawnPosition);
    }

    public override void ResetBoat()
    {
        base.ResetBoat();
        touristCount = Random.Range(1, 5);
    }
}
