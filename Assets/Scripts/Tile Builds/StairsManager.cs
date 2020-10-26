using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairsManager: MonoBehaviour
{
    public static bool StairsPlaceable(Vector2Int pos, out BuildRotation rot)
    {       
        rot = 0;

        TileInformation tileInfo = TileInformationManager.Instance.GetTileInformation(pos);
        TileInformation aboveTileInfo = TileInformationManager.Instance.GetTileInformation(new Vector2Int(pos.x, pos.y + 1));

        if (tileInfo == null)
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
                TileInformation checkForDockInfo = TileInformationManager.Instance.GetTileInformation(GetStairsConnectedDockPosition(pos, t.Item2));
                TileInformation checkForSandInfo = TileInformationManager.Instance.GetTileInformation(t.Item1);
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

    public static bool TryCreateStairs(StairsVariant variant, Vector2Int pos)
    {
        if (!StairsPlaceable(pos, out BuildRotation rot))
            return false;

        TileInformation tileInfo = TileInformationManager.Instance.GetTileInformation(pos);

        GameObject obj = new GameObject("Stairs");
        SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = variant.GetSprite(rot);
        renderer.sortingLayerName = "DynamicY";
        obj.transform.position = new Vector3(pos.x, pos.y, DynamicZDepth.GetDynamicZDepth(pos.y, DynamicZDepth.OBJECTS_STANDARD_OFFSET));

        tileInfo.CreateBuild(obj, variant, new HashSet<Vector2Int>() { pos }, rot, ObjectType.Ground);

        return true;
    }

    public static Vector2Int GetStairsConnectedDockPosition(Vector2Int StairsPosition, BuildRotation stairsRotation)
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
