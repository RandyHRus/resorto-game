using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionInstance
{
    public string InstanceName { get; private set; }

    public readonly RegionInformation regionInformation;
    protected HashSet<Vector2Int> regionPositions;

    public delegate void RegionRemoved();
    public event RegionRemoved OnRegionRemoved;

    public delegate void RegionModified(RegionInstance instance, TileInformation tileInfo);
    public event RegionModified OnRegionModified;

    public RegionInstance(string instanceName, RegionInformation regionInformation, HashSet<Vector2Int> positions)
    {
        this.InstanceName = instanceName;
        this.regionInformation = regionInformation;
        this.regionPositions = new HashSet<Vector2Int>();

        AddPositions(positions);
    }

    public List<Vector2Int> GetRegionPositionsAsList()
    {
        return new List<Vector2Int>(regionPositions);
    }

    public HashSet<Vector2Int> GetRegionPositions()
    {
        return new HashSet<Vector2Int>(regionPositions);
    }

    public virtual void AddPositions(HashSet<Vector2Int> positions)
    {
        foreach (Vector2Int pos in positions)
            regionPositions.Add(pos);

        foreach (Vector2Int pos in regionPositions)
        {
            TileInformationManager.Instance.TryGetTileInformation(pos, out TileInformation tileInfo);
            tileInfo.OnTileModified += OnRegionTileModifiedHandler;
        }
    }

    public void InvokeRegionRemoved()
    {
        foreach (Vector2Int pos in regionPositions)
        {
            TileInformationManager.Instance.TryGetTileInformation(pos, out TileInformation tileInfo);
            tileInfo.OnTileModified -= OnRegionTileModifiedHandler;
        }

        OnRegionRemoved?.Invoke();
    }

    public virtual List<string> GetWarnings()
    {
        return new List<string>();
    }

    public Vector2 GetWeightedMiddlePos()
    {
        float sumX = 0, sumY = 0;
        foreach (Vector2Int pos in regionPositions)
        {
            sumX += pos.x;
            sumY += pos.y;
        }
        return new Vector2(sumX / regionPositions.Count, sumY / regionPositions.Count);
    }

    public virtual void OnRegionTileModifiedHandler(TileInformation tileInfo)
    {
        OnRegionModified?.Invoke(this, tileInfo);
    }
}
