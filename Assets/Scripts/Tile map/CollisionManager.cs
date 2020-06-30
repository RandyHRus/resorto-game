using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    public static float boxColliderSizeX = 0.625f;
    public static float boxColliderSizeY = 0.2f;
    public static float BUFFER = 1 / 16f;

    private static CollisionManager _instance;
    public static CollisionManager Instance { get { return _instance; } }
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
    }

    //Check if object will collide if moved to a proposed position
    public bool CheckForCollisionMovement(Vector2 currentPosition, Vector2 proposedPosition, int tileLayer, out bool collisionX, out bool collisionY)
    {
        float xChange = proposedPosition.x - currentPosition.x;
        float yChange = proposedPosition.y - currentPosition.y;

        collisionX = false;
        collisionY = false;

        if (xChange != 0)
        {
            int xDir = Mathf.RoundToInt(Mathf.Sign(xChange));
            Vector3Int tilePositionToCheckUp = new Vector3Int(Mathf.RoundToInt(proposedPosition.x + (xDir * (boxColliderSizeX / 2f + BUFFER))), Mathf.RoundToInt(currentPosition.y + boxColliderSizeY / 2f), 0);
            Vector3Int tilePositionToCheckDown = new Vector3Int(Mathf.RoundToInt(proposedPosition.x + (xDir * (boxColliderSizeX / 2f + BUFFER))), Mathf.RoundToInt(currentPosition.y - boxColliderSizeY / 2f), 0);

            if (CheckForCollisionOnTile(tilePositionToCheckUp, tileLayer) || CheckForCollisionOnTile(tilePositionToCheckDown, tileLayer))
            {
                collisionX = true;
            }
        }

        if (yChange != 0)
        {
            int yDir = Mathf.RoundToInt(Mathf.Sign(yChange));
            Vector3Int tilePositionToCheckLeft = new Vector3Int(Mathf.RoundToInt(currentPosition.x - boxColliderSizeX / 2f), Mathf.RoundToInt(proposedPosition.y + (yDir * (boxColliderSizeY / 2f + BUFFER))), 0);
            Vector3Int tilePositionToCheckRight = new Vector3Int(Mathf.RoundToInt(currentPosition.x + boxColliderSizeX / 2f), Mathf.RoundToInt(proposedPosition.y + (yDir * (boxColliderSizeY / 2f + BUFFER))), 0);

            if (CheckForCollisionOnTile(tilePositionToCheckLeft, tileLayer) || CheckForCollisionOnTile(tilePositionToCheckRight, tileLayer))
            {
                collisionY = true;
            }
        }

        return (collisionX || collisionY);
    }


    public bool CheckForCollisionOnTile(Vector3Int tilePosition, int tileLayer)
    {
        TileInformation tile = TileInformationManager.Instance.GetTileInformation(tilePosition);
        if (!TileInformationManager.Instance.PositionInMap(tilePosition))
        {
            return true;
        }
        //If water and no ground object, means there is collision with water
        if (tile.tileLocation == TileLocation.WaterEdge)
        {
            if (tile.GetObjectOnTile(ObjectType.ground) == null)
                return true;
        }

        if (TileLocationManager.isCliff.HasFlag(tile.tileLocation))
            return true;

        if (tile.layerNum != tileLayer)
        {
            return true;
        }
        if (tile.collision) //TODO could be changed to precise collision checking?
        {
            return true;
        }
        
        return false;
    }

    public bool CheckForStairsMovement(Vector2 currentPosition, Vector2 proposedPosition, int tileLayer, out Vector3Int goalTile)
    {
        float xChange = proposedPosition.x - currentPosition.x;
        if (xChange != 0)
        {
            int xDir = Mathf.RoundToInt(Mathf.Sign(xChange));
            Vector3Int tilePositionToCheckUp = new Vector3Int(Mathf.RoundToInt(proposedPosition.x + (xDir * (boxColliderSizeX / 2f + BUFFER))), Mathf.RoundToInt(currentPosition.y + boxColliderSizeY / 2f), 0);
            Vector3Int tilePositionToCheckDown = new Vector3Int(Mathf.RoundToInt(proposedPosition.x + (xDir * (boxColliderSizeX / 2f + BUFFER))), Mathf.RoundToInt(currentPosition.y - boxColliderSizeY / 2f), 0);

            if (xChange < 0)
            {
                if (CheckForStairsTile(tilePositionToCheckUp, tileLayer, ObjectRotation.left) &&
                    CheckForStairsTile(tilePositionToCheckDown, tileLayer, ObjectRotation.left))
                {
                    goalTile = new Vector3Int(Mathf.RoundToInt(currentPosition.x - 2), Mathf.RoundToInt(currentPosition.y + 1), 0);
                    return true;
                }
                else if (CheckForStairsTile(new Vector3Int(tilePositionToCheckUp.x, tilePositionToCheckUp.y - 1, 0), tileLayer - 1, ObjectRotation.right) &&
                         CheckForStairsTile(new Vector3Int(tilePositionToCheckDown.x, tilePositionToCheckDown.y - 1, 0), tileLayer - 1, ObjectRotation.right))
                {
                    goalTile = new Vector3Int(Mathf.RoundToInt(currentPosition.x - 2), Mathf.RoundToInt(currentPosition.y - 1), 0);
                    return true;
                }
            }
            else if (xChange > 0)
            {
                if (CheckForStairsTile(tilePositionToCheckUp, tileLayer, ObjectRotation.right) &&
                    CheckForStairsTile(tilePositionToCheckDown, tileLayer, ObjectRotation.right))
                {
                    goalTile = new Vector3Int(Mathf.RoundToInt(currentPosition.x + 2), Mathf.RoundToInt(currentPosition.y + 1), 0);
                    return true;
                }
                else if (CheckForStairsTile(new Vector3Int(tilePositionToCheckUp.x, tilePositionToCheckUp.y - 1, 0), tileLayer - 1, ObjectRotation.left) &&
                         CheckForStairsTile(new Vector3Int(tilePositionToCheckDown.x, tilePositionToCheckDown.y - 1, 0), tileLayer - 1, ObjectRotation.left))
                {
                    goalTile = new Vector3Int(Mathf.RoundToInt(currentPosition.x + 2), Mathf.RoundToInt(currentPosition.y - 1), 0);
                    return true;
                }
            }
        }

        float yChange = proposedPosition.y - currentPosition.y;
        if (yChange != 0)
        {
            int yDir = Mathf.RoundToInt(Mathf.Sign(yChange));
            Vector3Int tilePositionToCheckLeft = new Vector3Int(Mathf.RoundToInt(currentPosition.x - boxColliderSizeX / 2f), Mathf.RoundToInt(proposedPosition.y + (yDir * (boxColliderSizeY / 2f + BUFFER))), 0);
            Vector3Int tilePositionToCheckRight = new Vector3Int(Mathf.RoundToInt(currentPosition.x + boxColliderSizeX / 2f), Mathf.RoundToInt(proposedPosition.y + (yDir * (boxColliderSizeY / 2f + BUFFER))), 0);

            //First check for stairs
            if (yChange > 0 && CheckForStairsTile(tilePositionToCheckLeft, tileLayer, ObjectRotation.front) && CheckForStairsTile(tilePositionToCheckRight, tileLayer, ObjectRotation.front))
            {
                goalTile = new Vector3Int(Mathf.RoundToInt(currentPosition.x), Mathf.RoundToInt(currentPosition.y + 3), 0);
                return true;
            }
            else if (yChange < 0 && 
                CheckForStairsTile(new Vector3Int(tilePositionToCheckLeft.x, tilePositionToCheckLeft.y - 1, 0), tileLayer - 1, ObjectRotation.front) && 
                CheckForStairsTile(new Vector3Int(tilePositionToCheckRight.x, tilePositionToCheckRight.y - 1, 0), tileLayer - 1, ObjectRotation.front))
            {
                goalTile = new Vector3Int(Mathf.RoundToInt(currentPosition.x), Mathf.RoundToInt(currentPosition.y - 3), 0);
                return true;
            }
        }

        goalTile = new Vector3Int();
        return false;
    }

    public bool CheckForStairsTile(Vector3Int tilePosition, int tileLayer, ObjectRotation rotation)
    {
        TileInformation tile = TileInformationManager.Instance.GetTileInformation(tilePosition);

        if (tile == null || tile.standardObject == null)
            return false;

        if (tileLayer != tile.layerNum)
            return false;

        return (tile.standardObject.id == 0 && tile.standardObject.rotation == rotation);
    }
}