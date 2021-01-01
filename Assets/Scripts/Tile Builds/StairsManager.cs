using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairsManager: MonoBehaviour
{
    private static StairsManager _instance;
    public static StairsManager Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public bool StairsPlaceable(Vector2Int pos, out BuildRotation rot)
    {       
        rot = 0;

        TileInformationManager.Instance.TryGetTileInformation(pos, out TileInformation tileInfo);
        TileInformationManager.Instance.TryGetTileInformation(new Vector2Int(pos.x, pos.y + 1), out TileInformation aboveTileInfo);

        if (tileInfo == null || aboveTileInfo == null)
            return false;

        if (tileInfo.TopMostBuild != null)
            return false;

        TileLocation tileLocation = tileInfo.tileLocation;
        TileLocation aboveTileLocation = aboveTileInfo.tileLocation;

        if (TileLocation.Cliff.HasFlag(tileInfo.tileLocation)) {

            switch (tileLocation)
            {
                case (TileLocation.CliffBack):
                    rot = BuildRotation.Back;
                    return true;
                case (TileLocation.CliffRight):
                    if (aboveTileLocation == TileLocation.CliffRight || aboveTileLocation == TileLocation.CliffCornerCurveIn)
                    {
                        rot = BuildRotation.Right;
                        return true;
                    }
                    else
                        return false;
                case (TileLocation.CliffLeft):
                    if (aboveTileLocation == TileLocation.CliffLeft || aboveTileLocation == TileLocation.CliffCornerCurveIn)
                    {
                        rot = BuildRotation.Left;
                        return true;
                    }
                    else
                        return false;
                case (TileLocation.CliffCornerCurveOut):
                    if (aboveTileLocation == TileLocation.CliffRight)
                    {
                        rot = BuildRotation.Right;
                        return true;
                    }
                    else if (aboveTileLocation == TileLocation.CliffLeft)
                    {
                        rot = BuildRotation.Left;
                        return true;
                    }
                    else
                        return false;
                default:
                    return false;
            }
        }
        else if (TileLocation.Land.HasFlag(tileInfo.tileLocation) && aboveTileLocation == TileLocation.CliffFront) {
            rot = BuildRotation.Front;
            return true;
            
        }
        else if (tileInfo.tileLocation == TileLocation.WaterEdge)
        {
            List<Tuple<Vector2Int, BuildRotation>> positionToBuildRotation = new List<Tuple<Vector2Int, BuildRotation>>
            {
                Tuple.Create(new Vector2Int(pos.x, pos.y - 1), BuildRotation.Front),
                Tuple.Create(new Vector2Int(pos.x, pos.y + 1), BuildRotation.Back),
                Tuple.Create(new Vector2Int(pos.x - 1, pos.y), BuildRotation.Left),
                Tuple.Create(new Vector2Int(pos.x + 1, pos.y), BuildRotation.Right),
            };

            foreach (Tuple<Vector2Int, BuildRotation> t in positionToBuildRotation)
            {
                TileInformationManager.Instance.TryGetTileInformation(GetStairsConnectedDockPosition(pos, t.Item2), out TileInformation checkForDockInfo);
                TileInformationManager.Instance.TryGetTileInformation(t.Item1, out TileInformation checkForSandInfo);
                if (checkForDockInfo?.NormalFlooringGroup?.FlooringVariant.GetType() == typeof(DockFlooringVariant) &&
                    checkForSandInfo?.tileLocation == TileLocation.Sand && checkForSandInfo?.layerNum == 0)
                {
                    rot = t.Item2;
                    return true;
                }
            }

            return false;                     
        }
        else
        {
            return false;
        }
    }

    public bool TryCreateStairs(StairsVariant variant, Vector2Int pos)
    {
        if (!StairsPlaceable(pos, out BuildRotation rot))
            return false;

        TileInformationManager.Instance.TryGetTileInformation(pos, out TileInformation tileInfo);

        GameObject obj = new GameObject("Stairs");
        SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = variant.GetSprite(rot);
        renderer.sortingLayerName = "DynamicY";
        obj.transform.position = new Vector3(pos.x, pos.y, DynamicZDepth.GetDynamicZDepth(pos.y, DynamicZDepth.OBJECTS_STANDARD_OFFSET));

        tileInfo.CreateBuild(obj, variant, new HashSet<Vector2Int>() { pos }, rot, ObjectType.Ground);

        return true;
    }

    public Vector2Int GetStairsConnectedDockPosition(Vector2Int StairsPosition, BuildRotation stairsRotation)
    {
        switch (stairsRotation)
        {
            case (BuildRotation.Front):
                return new Vector2Int(StairsPosition.x, StairsPosition.y + 2);

            case (BuildRotation.Left):
                return new Vector2Int(StairsPosition.x + 1, StairsPosition.y + 1);

            case (BuildRotation.Back):
                return new Vector2Int(StairsPosition.x, StairsPosition.y - 1);

            case (BuildRotation.Right):
                return new Vector2Int(StairsPosition.x - 1, StairsPosition.y + 1);

            default:
                throw new System.Exception("Unknown rotation");
        }
    }
}
