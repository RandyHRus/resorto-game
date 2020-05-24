using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInformationManager : MonoBehaviour
{
    [SerializeField] private ObjectInformation[] objectInformation = null;
    public Dictionary<int, ObjectInformation> objectInformationMap { get; private set; }

    private static ObjectInformationManager _instance;
    public static ObjectInformationManager Instance { get { return _instance; } }

    private void Awake()
    {
        //Singleton
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }
        {
            //Initialize dictionary
            objectInformationMap = new Dictionary<int, ObjectInformation>();
            foreach (ObjectInformation info in objectInformation)
            {
                objectInformationMap.Add(info.id, info);
            }
        }
    }
}

[System.Serializable]
public class ObjectInformation
{
    public int id;
    public string name;
    public Sprite iconSprite;
    public GameObject prefab;
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
        if      (rotation == ObjectRotation.front) return front;
        else if (rotation == ObjectRotation.left)  return left;
        else if (rotation == ObjectRotation.back)  return back;
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