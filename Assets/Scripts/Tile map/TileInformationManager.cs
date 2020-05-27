using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInformationManager : MonoBehaviour
{
    public static int tileCountX = 60, tileCountY = 60;

    private TileInformation[,] tileInformationMap;

    private static TileInformationManager _instance;
    public static TileInformationManager Instance { get { return _instance; } }

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
        //Initialize information map
        {
            tileInformationMap = new TileInformation[tileCountX, tileCountY];

            for (int i = 0; i < tileCountX; i++)
            {
                for (int j = 0; j < tileCountY; j++)
                {
                    tileInformationMap[i, j] = new TileInformation();
                }
            }
        }
    }

    public TileInformation GetTileInformation(Vector3Int position)
    {
        if (!PositionInMap(position))
        {
            return null;
        }
        else
        {
            return tileInformationMap[position.x, position.y];
        }
    }

    public bool PositionInMap(Vector3Int position)
    {
        return (position.x >= 0 && position.y >= 0 && position.x < tileCountX && position.y < tileCountY);
    }

    public Vector3Int GetMouseTile()
    {
        Vector2 mouseToWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector3Int(Mathf.RoundToInt(mouseToWorld.x), Mathf.RoundToInt(mouseToWorld.y), 0);
    }
}

public class TileInformation
{
    private bool hasFunctions;
    private ITileObjectFunctions functionsScript; //Only standard objects can have functions (for now)
    public int layerNum; //Water or sand is 0, land will be >1

    public bool isWater;

    private int regionId;

    private bool collision_;
    public bool collision
    {
        get
        {
            if (standardObject == null)
                return false;
            else
                return collision_;
        }
        private set
        {
            collision_ = value;
        }
    }

    private ObjectOnTile onTopObject_;
    public ObjectOnTile onTopObject
    {
        get
        {
            if (onTopObject_ == null || onTopObject_.gameObjectOnTile == null)
                return null;
            else
                return onTopObject_;
        }
        private set
        {
            onTopObject_ = value;
        }
    }

    private ObjectOnTile standardObject_;
    public ObjectOnTile standardObject
    {
        get
        {
            if (standardObject_ == null || standardObject_.gameObjectOnTile == null)
                return null;
            else
                return standardObject_;
        }
        private set
        {
            standardObject_ = value;
        }
    }

    private ObjectOnTile groundObject_;
    public ObjectOnTile groundObject
    {
        get
        {
            if (groundObject_ == null || groundObject_.gameObjectOnTile == null)
                return null;
            else
                return groundObject_;
        }
        private set
        {
            groundObject_ = value;
        }
    }

    public TileInformation()
    {
        hasFunctions = false;
        functionsScript = null;
        layerNum = Constants.INVALID_TILE_LAYER;
        isWater = false;
        onTopObject = null;
        standardObject = null;
        groundObject = null;
        collision = false;
        regionId = 0;
    }

    public bool SetTileObject(GameObject obj, int id, bool collision, ObjectRotation rotation, ObjectType type, bool objectsCanBePlacedOnTop)
    {
        ObjectOnTile objectOnTile = GetObjectOnTile(type);

        if (objectOnTile != null)
        {
            Debug.Log("Tried to set object where there already is an object! This should not happen, check beforehand.");
            return false;
        }
        {
            switch (type)
            {
                case (ObjectType.standard):
                    hasFunctions = obj.GetComponent<ITileObjectFunctions>() != null;
                    if (hasFunctions)
                        functionsScript = obj.GetComponent<ITileObjectFunctions>();

                    this.collision = collision;

                    standardObject = new ObjectOnTile(obj, id, rotation, objectsCanBePlacedOnTop);
                    return true;
                case (ObjectType.ground):
                    groundObject = new ObjectOnTile(obj, id, rotation, objectsCanBePlacedOnTop);
                    return true;
                case (ObjectType.onTop):
                    onTopObject = new ObjectOnTile(obj, id, rotation, objectsCanBePlacedOnTop);
                    return true;
                default:
                    Debug.Log("Unknown type!");
                    return false;
            }
        }
    }

    public void SetRegion(int id)
    {
        regionId = id;
    }

    public RegionInformation GetRegionInformation()
    {
        if (RegionInformationManager.Instance.regionInformationMap.TryGetValue(regionId, out RegionInformation info))
            return info;
        else
            return null;
    }

    public int RemoveTileObject()
    {
        int returnId;

        if (onTopObject != null)
        {
            returnId = onTopObject.id;
            Object.Destroy(onTopObject.gameObjectOnTile);
        }
        else if (standardObject != null)
        {
            returnId = standardObject.id;
            Object.Destroy(standardObject.gameObjectOnTile);
        }
        else if (groundObject != null)
        {
            returnId = groundObject.id;
            Object.Destroy(groundObject.gameObjectOnTile);
        }
        else
        {
            Debug.Log("No object to destroy!");
            returnId = Constants.INVALID_ID;
        }

        return returnId;

    }

    public ObjectOnTile GetObjectOnTile(ObjectType type)
    {
        switch (type)
        {
            case (ObjectType.standard): return standardObject;
            case (ObjectType.onTop): return onTopObject;
            case (ObjectType.ground): return groundObject;
            default:
                Debug.Log("Unknown type!");
                return null;
        }
    }

    public bool TileIsEmpty()
    {
        return (standardObject == null && groundObject == null && onTopObject == null);
    }

    public void StepOn()
    {
        if (hasFunctions && standardObject != null)
            functionsScript.StepOn();
    }

    public void StepOff()
    {
        if (hasFunctions && standardObject != null)
            functionsScript.StepOff();
    }

    public void ClickLeft()
    {
        if (hasFunctions && standardObject != null)
            functionsScript.ClickLeft();
    }

    public void ClickRight()
    {
        if (hasFunctions && standardObject != null)
            functionsScript.ClickRight();
    }
}

public class ObjectOnTile
{
    public ObjectOnTile(GameObject gameObjectOnTile, int id, ObjectRotation rotation, bool objectsCanBePlacedOnTop)
    {
        this.gameObjectOnTile = gameObjectOnTile;
        this.id = id;
        this.rotation = rotation;
        this.objectsCanBePlacedOnTop = objectsCanBePlacedOnTop;
    }

    public GameObject gameObjectOnTile { get; private set; }

    public int id { get; private set; }

    public ObjectRotation rotation { get; private set; }

    public bool objectsCanBePlacedOnTop { get; private set; }
}