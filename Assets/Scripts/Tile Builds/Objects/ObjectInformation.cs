using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ObjectInformation : ScriptableObject, IBuildable
{
    [SerializeField] private string _name = null;
    public string Name => _name;

    [SerializeField] private bool useDefaultPrefab = true;
    public bool UseDefaultPrefab => useDefaultPrefab;

    [ConditionalHide("useDefaultPrefab", true, true), SerializeField] private GameObject prefab = null;
    public GameObject Prefab => prefab;

    [SerializeField] private bool collision = false;
    public bool Collision => collision;

    [SerializeField] private bool objectsCanBePlacedOnTop = false;
    public bool ObjectsCanBePlacedOnTop => objectsCanBePlacedOnTop;

    //Only used when ObjectType is onTop
    [ConditionalHide("objectsCanBePlacedOnTop", true, false), SerializeField] private int onTopOffsetInPixels = 0;
    public int OnTopOffsetInPixels => onTopOffsetInPixels;

    [SerializeField] private ObjectType type = ObjectType.Standard;
    public ObjectType Type => type;

    [SerializeField] private ObjectPlaceableLocation location = ObjectPlaceableLocation.Land;
    public ObjectPlaceableLocation Location => location;

    [SerializeField] private bool hasSprite = true;
    public bool HasSprite => hasSprite;

    [ConditionalHide("hasSprite", true, false), SerializeField]
    private ObjectSpriteInformation frontSprite = null;
    public ObjectSpriteInformation FrontSprite => frontSprite;

    [ConditionalHide("hasSprite", true, false), SerializeField]
    private ObjectSpriteInformation rightSprite = null;
    public ObjectSpriteInformation RightSprite => rightSprite;

    [ConditionalHide("hasSprite", true, false), SerializeField]
    private ObjectSpriteInformation backSprite = null;
    public ObjectSpriteInformation BackSprite => backSprite;

    [ConditionalHide("hasSprite", true, false), SerializeField]
    private ObjectSpriteInformation leftSprite = null;
    public ObjectSpriteInformation LeftSprite => leftSprite;

    [ConditionalHide("hasSprite", true, true), SerializeField]
    private Vector2Int sizeWhenNoSprite = new Vector2Int();
    public Vector2Int SizeWhenNoSprite => sizeWhenNoSprite;

    [SerializeField] private InventoryItemInformation dropItem = null;

    public ObjectSpriteInformation GetSpriteInformation(BuildRotation rotation)
    {
        switch (rotation)
        {
            case (BuildRotation.Front):
                return frontSprite;
            case (BuildRotation.Right):
                return rightSprite;
            case (BuildRotation.Back):
                return backSprite;
            case (BuildRotation.Left):
                return leftSprite;
            default:
                throw new System.Exception("Unknown rotation");
        }
    }

    public void OnRemove(BuildOnTile build)
    {
        if (dropItem != null)
        {
            Vector3Int pos = build.OccupiedTiles[0];
            DropItems.DropItem(new Vector2(pos.x, pos.y), dropItem, 1, true);
        }
    }

    public Vector2Int GetSizeOnTile(BuildRotation rotation)
    {
        return GetSpriteInformation(rotation).Size;
    }
}

[System.Serializable]
public class ObjectSpriteInformation
{
    [SerializeField] private Sprite sprite = null;
    public Sprite Sprite => sprite;

    [SerializeField] private Vector2Int size = new Vector2Int();
    public Vector2Int Size => size;
}