using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ObjectInformation : ScriptableObject
{
    [SerializeField] private string objectName = null;
    public string ObjectName => objectName;

    [SerializeField] private GameObject prefab = null;
    public GameObject Prefab => prefab;

    [SerializeField] private bool collision = false;
    public bool Collision => collision;

    [SerializeField] private bool objectsCanBePlacedOnTop = false;
    public bool ObjectsCanBePlacedOnTop => objectsCanBePlacedOnTop;

    //Only used when ObjectType is onTop
    [ConditionalHide("objectsCanBePlacedOnTop", true, false), SerializeField] private int onTopOffsetInPixels = 0;
    public int OnTopOffsetInPixels => onTopOffsetInPixels;

    [SerializeField] private ObjectType type = ObjectType.standard;
    public ObjectType Type => type;

    [SerializeField] private ObjectPlaceableLocation location = ObjectPlaceableLocation.Land;
    public ObjectPlaceableLocation Location => location;

    [SerializeField] private bool hasSprite = true;
    public bool HasSprite => hasSprite;

    [ConditionalHide("hasSprite", true, false), SerializeField] private ObjectSpriteInformation front = null;
    public ObjectSpriteInformation Front => front;

    [ConditionalHide("hasSprite", true, false), SerializeField] private ObjectSpriteInformation left = null;
    public ObjectSpriteInformation Left => left;

    [ConditionalHide("hasSprite", true, false), SerializeField] private ObjectSpriteInformation back = null;
    public ObjectSpriteInformation Back => back;

    [ConditionalHide("hasSprite", true, false), SerializeField] private ObjectSpriteInformation right = null;
    public ObjectSpriteInformation Right => right;

    [ConditionalHide("hasSprite", true, true), SerializeField] private Vector2Int size = new Vector2Int();
    public Vector2Int SizeWhenNoSprite => size;

    [SerializeField] private InventoryItemInformation dropItem = null;
    public InventoryItemInformation DropItem => dropItem;

    public ObjectSpriteInformation GetSpriteInformation(ObjectRotation rotation)
    {
        switch (rotation) {
            case (ObjectRotation.front):
                return front;
            case (ObjectRotation.left):
                return left;
            case (ObjectRotation.back):
                return back;
            case (ObjectRotation.right):
                return right;
            default:
                return null;
        }   
    }
}

[System.Serializable]
public class ObjectSpriteInformation
{

    [SerializeField] private Sprite sprite = null;
    public Sprite Sprite => sprite;

    [SerializeField] private int xSize = 0;
    public int XSize => xSize;

    [SerializeField] private int ySize = 0;
    public int YSize => ySize;
}

public enum ObjectRotation
{
    front,
    left,
    back,
    right
}

public enum ObjectPlaceableLocation
{
    Land,
    Water,
    SandOnly,
    GrassOnly
}

public enum ObjectType
{
    None,

    standard,
    ground,
    onTop
}