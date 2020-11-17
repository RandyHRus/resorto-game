using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotelRoomRegionInstance : RegionInstance
{
    private ArrayHashSet<Vector2Int> bedPositions = new ArrayHashSet<Vector2Int>();
    private static IBuildable bedInformation;

    public bool Occupied { get; private set; } = false;
    public bool IsValid { get; private set; } = false;
    public bool IsValidAndUnOccupied => IsValid && !Occupied;

    public delegate void RoomOccpied(HotelRoomRegionInstance room);
    public event RoomOccpied OnRoomOccupied;

    public delegate void RoomUnOccupied(HotelRoomRegionInstance room);
    public event RoomUnOccupied OnRoomUnOccupied;

    public delegate void RoomValidityChanged(HotelRoomRegionInstance room);
    public event RoomValidityChanged OnRoomValidityChanged;

    static HotelRoomRegionInstance()
    {
        bedInformation = ResourceManager.Instance.BedObjectInfo;
    }

    public HotelRoomRegionInstance(string instanceName, RegionInformation info, HashSet<Vector2Int> positions) : base(instanceName, info, positions)
    {
    }

    public override void AddPositions(HashSet<Vector2Int> positions)
    {
        base.AddPositions(positions);

        foreach (Vector2Int pos in positions)
        {
            TileInformationManager.Instance.TryGetTileInformation(pos, out TileInformation tileInfo);
            RefreshCheckTileForBed(tileInfo);
        }

        RefreshRoomValidity();
    }

    public override void OnRegionTileModifiedHandler(TileInformation tileInfo)
    {
        RefreshCheckTileForBed(tileInfo);
        RefreshRoomValidity();

        base.OnRegionTileModifiedHandler(tileInfo);
    }

    public Vector2Int GetRandomBedPositionInRoom()
    {
        return bedPositions.GetRandom();
    }

    private void RefreshCheckTileForBed(TileInformation tileInfo)
    {
        BuildOnTile topMostBuild = tileInfo.TopMostBuild;

        if (topMostBuild?.BuildInfo != bedInformation) {
            //Means bed was removed
            if (bedPositions.Contains(tileInfo.position))
            {
                bedPositions.Remove(tileInfo.position);
            }
        }
        else if (topMostBuild?.BuildInfo == bedInformation)
        {
            //Means new bed added
            if (!bedPositions.Contains(tileInfo.position))
            {
                bedPositions.Add(tileInfo.position);
            }
        }
    }

    private void RefreshRoomValidity()
    {
        bool validityOld = IsValid;
        IsValid = (GetWarnings().Count == 0);

        if (validityOld != IsValid)
            OnRoomValidityChanged?.Invoke(this);
    }

    public override List<string> GetWarnings()
    {
        List<string> warnings =  base.GetWarnings();

        if (bedPositions.Count <= 0)
        {
            warnings.Add("No bed!");
        }

        return warnings;
    }

    public void OccupyRoom()
    {
        if (Occupied)
            throw new System.Exception("Already occupied");

        Occupied = true;
        OnRoomOccupied?.Invoke(this);
    }

    public void UnOccupyRoom()
    {
        if (!Occupied)
            throw new System.Exception("Already unoccupied!");

        Occupied = false;
        OnRoomUnOccupied?.Invoke(this);
    }
}
