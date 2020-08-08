using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairsManager: MonoBehaviour
{
    public static bool StairsPlaceable(Vector3Int pos, out BuildRotation rot)
    {       
        rot = 0;

        TileInformation tileInfo = TileInformationManager.Instance.GetTileInformation(pos);
        TileInformation aboveTileInfo = TileInformationManager.Instance.GetTileInformation(new Vector3Int(pos.x, pos.y + 1, 0));

        if (tileInfo == null)
            return false;

        if (tileInfo.BuildsOnTile.TopMostBuild != null)
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
            List<Tuple<Vector3Int, BuildRotation>> positionToBuildRotation = new List<Tuple<Vector3Int, BuildRotation>>
            {
                Tuple.Create(new Vector3Int(pos.x, pos.y - 1, 0), BuildRotation.Front),
                Tuple.Create(new Vector3Int(pos.x, pos.y + 1, 0), BuildRotation.Back),
                Tuple.Create(new Vector3Int(pos.x - 1, pos.y, 0), BuildRotation.Left),
                Tuple.Create(new Vector3Int(pos.x + 1, pos.y, 0), BuildRotation.Right),
            };

            foreach (Tuple<Vector3Int, BuildRotation> t in positionToBuildRotation)
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

    public static bool TryCreateStairs(StairsVariant variant, Vector3Int pos)
    {
        if (!StairsPlaceable(pos, out BuildRotation rot))
            return false;

        TileInformation tileInfo = TileInformationManager.Instance.GetTileInformation(pos);

        GameObject obj = new GameObject("Stairs");
        SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = variant.GetSprite(rot);
        renderer.sortingLayerName = "DynamicY";
        obj.transform.position = new Vector3(pos.x, pos.y, DynamicZDepth.GetDynamicZDepth(pos.y, DynamicZDepth.OBJECTS_STANDARD_OFFSET));

        //Set tile
        BuildOnTile stairsBuild = new BuildOnTile(obj, variant, new List<Vector3Int>() { pos }, rot, ObjectType.Ground);
        tileInfo.BuildsOnTile.SetTileObject(stairsBuild);

        //Refresh connected docks
        if (tileInfo.tileLocation == TileLocation.WaterEdge)
        {
            Vector3Int dockTilePosition = GetStairsConnectedDockPosition(pos, rot);
            TileInformation dockTileInformation = TileInformationManager.Instance.GetTileInformation(dockTilePosition);

            dockTileInformation.NormalFlooringGroup.NormalFloorings[dockTilePosition].Renderer.sprite = 
                FlooringManager.GetSprite(dockTileInformation.NormalFlooringGroup.FlooringVariant, null, false, dockTilePosition, dockTileInformation.NormalFlooringGroup.Rotation);

            dockTileInformation.NormalFlooringGroup.AddConnectedBuild(stairsBuild);
        }

        return true;
    }

    public static Vector3Int GetStairsConnectedDockPosition(Vector3Int StairsPosition, BuildRotation stairsRotation)
    {
        switch (stairsRotation)
        {
            case (BuildRotation.Front):
                return new Vector3Int(StairsPosition.x, StairsPosition.y + 2, 0);

            case (BuildRotation.Left):
                return new Vector3Int(StairsPosition.x + 1, StairsPosition.y + 1, 0);

            case (BuildRotation.Back):
                return new Vector3Int(StairsPosition.x, StairsPosition.y - 1, 0);

            case (BuildRotation.Right):
                return new Vector3Int(StairsPosition.x - 1, StairsPosition.y + 1, 0);

            default:
                throw new System.Exception("Unknown rotation");
        }
    }
}
