using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ObjectInformation : ScriptableObject
{
    public int id;
    public string objectName;
    public GameObject prefab;
    public Sprite iconSprite;
    public bool collision;
    public bool objectsCanBePlacedOnTop;
    public ObjectType type;
    public ObjectPlaceableLocation location;
    public SpriteInformation front;
    public SpriteInformation left;
    public SpriteInformation back;
    public SpriteInformation right;

    public SpriteInformation GetSpriteInformation(ObjectRotation rotation)
    {
        if (rotation == ObjectRotation.front) return front;
        else if (rotation == ObjectRotation.left) return left;
        else if (rotation == ObjectRotation.back) return back;
        else if (rotation == ObjectRotation.right) return right;
        else return null;
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
    land,
    water,
    sandOnly
}

public enum ObjectType
{
    standard,
    ground,
    onTop
}