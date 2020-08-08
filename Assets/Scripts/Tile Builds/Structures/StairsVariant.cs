using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Structure Variants/Stairs")]
public class StairsVariant : StructureVariantInformation, IBuildable
{
    [EnumNamedArray(typeof(BuildRotation)), SerializeField]
    private Sprite[] stairsSprites = null;

    public bool Collision { get { return true; } }

    public bool ObjectsCanBePlacedOnTop { get { return false; } }

    public int OnTopOffsetInPixels => throw new System.Exception("Objects should not be placed on top");

    public Sprite GetSprite(BuildRotation dir)
    {
        return stairsSprites[(int)dir];
    }

    public Vector2Int GetSizeOnTile(BuildRotation rotation)
    {
        return new Vector2Int(1, 1);
    }

    public void OnRemove(BuildOnTile build)
    {
        Vector3Int checkForDockPos = StairsManager.GetStairsConnectedDockPosition(build.OccupiedTiles[0], build.Rotation);

        TileInformation tileInfo = TileInformationManager.Instance.GetTileInformation(checkForDockPos);

        if (tileInfo?.NormalFlooringGroup != null)
        {
            tileInfo.NormalFlooringGroup.NormalFloorings[checkForDockPos].Renderer.sprite =
                FlooringManager.GetSprite(tileInfo.NormalFlooringGroup.FlooringVariant, new HashSet<Vector3Int> { build.OccupiedTiles[0] }, false, checkForDockPos, tileInfo.NormalFlooringGroup.Rotation);
        }
    }
}