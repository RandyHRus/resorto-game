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

    [SerializeField] private ObjectType type = ObjectType.standard;
    public ObjectType Type => type;

    [SerializeField] private ObjectPlaceableLocation location = ObjectPlaceableLocation.Land;
    public ObjectPlaceableLocation Location => location;

    [SerializeField] private bool hasSprite = true;
    public bool HasSprite => hasSprite;

    [ConditionalHide("hasSprite", true), SerializeField] private SpriteInformation front = null;
    public SpriteInformation Front => front;

    [ConditionalHide("hasSprite", true), SerializeField] private SpriteInformation left = null;
    public SpriteInformation Left => left;

    [ConditionalHide("hasSprite", true), SerializeField] private SpriteInformation back = null;
    public SpriteInformation Back => back;

    [ConditionalHide("hasSprite", true), SerializeField] private SpriteInformation right = null;
    public SpriteInformation Right => right;

    [SerializeField] private InventoryItemInformation dropItem = null;
    public InventoryItemInformation DropItem => dropItem;

    public SpriteInformation GetSpriteInformation(ObjectRotation rotation)
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
public class SpriteInformation
{
    public Sprite sprite;
    public int xSize;
    public int ySize;
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