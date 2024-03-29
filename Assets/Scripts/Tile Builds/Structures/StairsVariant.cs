﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Structure Variants/Stairs")]
public class StairsVariant : StructureInformation, IBuildable
{
    [EnumNamedArray(typeof(BuildRotation)), SerializeField]
    private Sprite[] stairsSprites = null;

    public bool Collision { get { return true; } }

    public bool ObjectsCanBePlacedOnTop { get { return false; } }

    public int OnTopOffsetInPixels => throw new System.Exception("Objects should not be placed on top");

    public int TransparencyCapableYSize => 0;

    public Sprite GetSprite(BuildRotation dir)
    {
        return stairsSprites[(int)dir];
    }

    public Vector2Int GetSizeOnTile(BuildRotation rotation)
    {
        return new Vector2Int(1, 1);
    }

    public void OnCreate(BuildOnTile build)
    {
        TileInformationManager.Instance.TryGetTileInformation(build.BottomLeft, out TileInformation buildTileInfo);

        //Refresh connected docks
        if (buildTileInfo.tileLocation == TileLocation.WaterEdge)
        {
            Vector2Int dockTilePosition = StairsManager.Instance.GetStairsConnectedDockPosition(build.BottomLeft, build.Rotation);
            TileInformationManager.Instance.TryGetTileInformation(dockTilePosition, out TileInformation dockTileInformation);

            dockTileInformation.NormalFlooringGroup.NormalFloorings[dockTilePosition].Renderer.sprite =
                FlooringManager.Instance.GetSprite(dockTileInformation.NormalFlooringGroup.FlooringVariant, null, false, dockTilePosition, dockTileInformation.NormalFlooringGroup.Rotation);

            dockTileInformation.NormalFlooringGroup.AddConnectedBuild(build);
        }

        //Set stairs on connected tiles
        {
            Stairs stairs = new Stairs(build.BottomLeft, buildTileInfo.layerNum, build.Rotation);
            buildTileInfo.SetStairs(stairs);
        }
    }

    public void OnRemoveThroughPlayerInteraction(BuildOnTile build)
    {
        
    }

    public void OnRemove(BuildOnTile build)
    {
        TileInformationManager.Instance.TryGetTileInformation(build.BottomLeft, out TileInformation buildTileInfo);

        //Change connected dock sprite
        {
            Vector2Int checkForDockPos = StairsManager.Instance.GetStairsConnectedDockPosition(build.BottomLeft, build.Rotation);
            TileInformationManager.Instance.TryGetTileInformation(checkForDockPos, out TileInformation checkForDockTileInfo);
            if (checkForDockTileInfo?.NormalFlooringGroup != null)
            {
                checkForDockTileInfo.NormalFlooringGroup.NormalFloorings[checkForDockPos].Renderer.sprite =
                    FlooringManager.Instance.GetSprite(checkForDockTileInfo.NormalFlooringGroup.FlooringVariant, new HashSet<Vector2Int> { build.BottomLeft }, false, checkForDockPos, checkForDockTileInfo.NormalFlooringGroup.Rotation);
            }
        }

        //Remove stairs connected on tile
        {
            buildTileInfo.UnSetStairs();
        }
    }
}