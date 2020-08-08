using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveManager
{
    public static bool BuildRemovable(Vector3Int pos, out BuildOnTile buildOnTile)
    {
        buildOnTile = null;

        TileInformation tileInfo = TileInformationManager.Instance.GetTileInformation(pos);
        if (tileInfo == null)
            return false;

        BuildOnTile topMostObject = tileInfo.BuildsOnTile.TopMostBuild;
        if (topMostObject == null)
            return false;

        //Check if there is any objects on above, if there is, it cannot be destroyed
        foreach (Vector3Int checkPos in topMostObject.OccupiedTiles)
        {
            BuildGroupOnTile thisGroup = TileInformationManager.Instance.GetTileInformation(checkPos).BuildsOnTile;

            if (thisGroup.TopMostBuild != topMostObject)
                return false;
        }

        buildOnTile = topMostObject;
        return true;
    }

    public static bool TryRemoveBuild(Vector3Int pos, out IBuildable removedBuildInfo)
    {
        if (!BuildRemovable(pos, out BuildOnTile buildOnTile))
        {
            removedBuildInfo = null;
            return false;
        }

        RemoveBuild(pos, buildOnTile, out removedBuildInfo);

        return true;
    }

    public static bool BuildRemovable(BuildOnTile build)
    {
        return BuildRemovable(build.OccupiedTiles[0], out BuildOnTile outBuild) ? outBuild == build : false;
    }

    public static bool TryRemoveBuild(BuildOnTile build)
    {
        if (!BuildRemovable(build))
            return false;

        RemoveBuild(build.OccupiedTiles[0], build, out IBuildable removedBuildInfo);

        return true;
    }

    //Removes top most object
    private static void RemoveBuild(Vector3Int pos, BuildOnTile buildOnTile, out IBuildable removedBuildInfo)
    {
        TileInformation tileInfo = TileInformationManager.Instance.GetTileInformation(pos);
        removedBuildInfo = buildOnTile.BuildInfo;

        buildOnTile.IndicateBuildRemoved();

        if (tileInfo.BuildsOnTile.RemoveTopMostTileObject() != buildOnTile.ModifiedType)
            throw new System.Exception("Removed wrong type!");

        removedBuildInfo.OnRemove(buildOnTile);
    }
}
