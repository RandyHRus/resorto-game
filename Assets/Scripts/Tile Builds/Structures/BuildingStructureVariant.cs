using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Structure Variants/Buildings")]
public class BuildingStructureVariant : StructureInformation, IBuildable
{
    [SerializeField] private Vector2Int size = new Vector2Int();

    [SerializeField] private GameObject prefab = null;
    public GameObject Prefab => prefab;

    public UIObject uiToShowOnSelect => new UIObject(ResourceManager.Instance.HouseCustomizationMenu);

    public bool Collision { get { return true; } }

    public bool ObjectsCanBePlacedOnTop { get { return false; } }

    public int OnTopOffsetInPixels => throw new System.Exception("Objects should not be placed on top");

    public int TransparencyCapableYSize => 4;

    public Vector2Int GetSizeOnTile(BuildRotation rotation)
    {
        //Buildings should only face front (for now....)
        return size;
    }

    public void OnRemoveThroughState(BuildOnTile buildOnTile)
    {
        //Nothing for now
    }
}
