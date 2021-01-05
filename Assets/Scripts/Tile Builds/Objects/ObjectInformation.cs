using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu]
public class ObjectInformation : StructureInformation, IBuildable
{
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

    [SerializeField] private TileLocation placeableLocations = 0;
    public TileLocation PlaceableLocations => placeableLocations;

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

    [SerializeField] private AssetReference dropItem = null;

    [SerializeField] private int transparencyCapableYSize = 0;
    public int TransparencyCapableYSize => transparencyCapableYSize;

    private float dropHeight = 0.5f;
    private float minDropXSpeed = -0.4f;
    private float maxDropXSpeed = 0.4f;

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

    public void OnRemoveThroughPlayerInteraction(BuildOnTile build)
    {
        if (dropItem != null)
        {
            Vector2Int pos = build.BottomLeft;
            DropItems.DropItem(pos, dropHeight, new InventoryItemInstance(dropItem), 1, UnityEngine.Random.Range(minDropXSpeed, maxDropXSpeed));
        }
    }

    public Vector2Int GetSizeOnTile(BuildRotation rotation)
    {
        return GetSpriteInformation(rotation).Size;
    }

    public void OnCreate(BuildOnTile buildOnTile)
    {
        
    }

    public void OnRemove(BuildOnTile buildOnTile)
    {
        
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