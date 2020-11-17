using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FishingRegionInstance : RegionInstance
{
    private ArrayHashSet<Vector2Int> validFishingPositions = new ArrayHashSet<Vector2Int>();

    public FishingRegionInstance(string instanceName, RegionInformation info, HashSet<Vector2Int> positions): base (instanceName, info, positions)
    {      
    }

    public override void AddPositions(HashSet<Vector2Int> positions)
    {
        base.AddPositions(positions);

        foreach (Vector2Int pos in positions)
        {
            UpdateValidFishingPosition(pos);
        }
    }

    public Vector2Int? GetRandomFishingPosition()
    {
        if (validFishingPositions.Count == 0)
            return null;

        Vector2Int pos = validFishingPositions.GetRandom();
        return pos;
    }

    public bool IsValidFishingPositionInThisRegion(Vector2Int pos)
    {
        return validFishingPositions.Contains(pos);
    }

    public bool IsDeepWaterInRegion(Vector2Int pos)
    {
        if (!regionPositions.Contains(pos))
            return false;

        TileInformationManager.Instance.TryGetTileInformation(pos, out TileInformation tileInfo);

        return (tileInfo.tileLocation == TileLocation.DeepWater && tileInfo.NormalFlooringGroup == null);
    }

    private void UpdateValidFishingPosition(Vector2Int pos)
    {
        TileInformationManager.Instance.TryGetTileInformation(pos, out TileInformation tileInfo);

        bool isValid = true;

        //Needs to be in this fishing region
        if (!regionPositions.Contains(pos))
        {
            isValid = false;
        }

        //If there is collision on tile, character can't stand there
        if (CollisionManager.CheckForCollisionOnTile(pos, tileInfo.layerNum))
        {
            isValid = false;
        }


        List<Vector2Int> directionsWithWater = GetDirectionsWithWater(pos);
        if (directionsWithWater.Count == 0)
            isValid = false;

        if (isValid)
        {
            if (!validFishingPositions.Contains(pos))
            {
                validFishingPositions.Add(pos);
            }
        }
        else
        {
            if (validFishingPositions.Contains(pos))
            {
                validFishingPositions.Remove(pos);
            }
        }
    }

    public List<Vector2Int> GetDirectionsWithWater(Vector2Int pos)
    {
        List<Vector2Int> directionsWithWater = new List<Vector2Int>(4);

        Vector2Int[] directionsToCheckForWater = new Vector2Int[]
        {
            Direction.Up.DirectionVector(),
            Direction.Down.DirectionVector(),
            Direction.Left.DirectionVector(),
            Direction.Right.DirectionVector()
        };

        foreach (Vector2Int n in directionsToCheckForWater)
        {          
            if (!TileInformationManager.Instance.TryGetTileInformation(pos + n, out TileInformation nTileInfo))
                continue;

            if (nTileInfo.tileLocation != TileLocation.DeepWater)
                continue;

            if (nTileInfo.NormalFlooringGroup != null)
                continue;

            //Neighbour also needs to be in region
            if (!regionPositions.Contains(pos + n))
                continue;

            directionsWithWater.Add(n);
        }

        return directionsWithWater;
    }

    public override void OnRegionTileModifiedHandler(TileInformation tileInfo)
    {
        base.OnRegionTileModifiedHandler(tileInfo);

        void UpdateValididity(Vector2Int pos)
        {
            if (TileInformationManager.Instance.TryGetTileInformation(pos, out TileInformation thisTileInfo))
                UpdateValidFishingPosition(thisTileInfo.position);
        }

        //Update self
        UpdateValididity(tileInfo.position);

        Vector2Int[] neighbours = tileInfo.neighbours;

        //Update neighbours
        foreach (Vector2Int n in neighbours)
        {
            UpdateValididity(n);
        }
    }

    public override List<string> GetWarnings()
    {
        List<string> warnings = base.GetWarnings();

        if (validFishingPositions.Count == 0)
            warnings.Add("No fishing positions!");

        return warnings;
    }
}
