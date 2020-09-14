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

        BuildOnTile topMostObject = tileInfo.TopMostBuild;
        if (topMostObject == null)
            return false;

        //Check if there is any objects on above, if there is, it cannot be destroyed
        foreach (Vector3Int checkPos in topMostObject.OccupiedTiles)
        {
            TileInformation thisInfo = TileInformationManager.Instance.GetTileInformation(checkPos);

            if (thisInfo.TopMostBuild != topMostObject)
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

        removedBuildInfo = buildOnTile.BuildInfo;
        buildOnTile.RemoveBuild();

        return true;
    }

    public static bool BuildRemovable(BuildOnTile build)
    {
        return BuildRemovable((Vector3Int)build.BottomLeft, out BuildOnTile outBuild) ? outBuild == build : false;
    }

    public static bool TryRemoveBuild(BuildOnTile build)
    {
        if (!BuildRemovable(build))
            return false;

        build.RemoveBuild();

        return true;
    }
}
