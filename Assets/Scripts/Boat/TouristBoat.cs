using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouristBoat : Boat
{
    private Vector2Int touristSpawnPosition;
    private int touristCount;

    public override void Initialize(Vector2Int playerStartingPosition)
    {
        base.Initialize(playerStartingPosition);
        touristSpawnPosition = boatUnloadingRegionInstance.GetRegionPositions()[0];
    }

    public override void Awake()
    {
        base.Awake();
        OnBoatUnloadingPointReached += SpawnTourists;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        OnBoatUnloadingPointReached -= SpawnTourists;
    }

    private void SpawnTourists()
    {
        for (int i = 0; i < touristCount; i++)
            TouristsManager.Instance.CreateTourist(TouristInformation.CreateRandomTouristInformation(), touristSpawnPosition);
    }

    public override void ResetBoat()
    {
        base.ResetBoat();

        touristCount = TouristsManager.Instance.NumberOfTouristsThatCanBeSpawned > 0 ? 
            Random.Range(1, 
                         TouristsManager.Instance.NumberOfTouristsThatCanBeSpawned + 1):
            0;
    }
}
