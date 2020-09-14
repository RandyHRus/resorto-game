using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager
{
    public static float boxColliderSizeX = 0.625f;
    public static float boxColliderSizeY = 0.2f;
    public static float BUFFER = 1 / 16f;

    //Check if object will collide if moved to a proposed position
    public static bool CheckForCollisionMovement(Vector2 currentPosition, Vector2 proposedPosition, int tileLayer, out bool collisionX, out bool collisionY)
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


    public static bool CheckForCollisionOnTile(Vector3Int tilePosition, int tileLayer)
    {
        TileInformation tile = TileInformationManager.Instance.GetTileInformation(tilePosition);
        if (!TileInformationManager.Instance.PositionInMap(tilePosition))
            return true;

        if (tile.Collision) //TODO could be changed to precise collision checking?
            return true;

        //If water and no dock, there is collision
        if (TileLocation.Water.HasFlag(tile.tileLocation))
        {
            if (tile.NormalFlooringGroup == null)
                return true;
        }

        if (TileLocation.Cliff.HasFlag(tile.tileLocation))
            return true;

        if (tile.layerNum != tileLayer)
            return true;
        
        return false;
    }

    public static bool CheckForStairsMovement(Vector2 currentPosition, Vector2 proposedPosition, int tileLayer, out Vector3Int goalTile)
    {
        float xChange = proposedPosition.x - currentPosition.x;
        if (xChange != 0)
        {
            int xDir = Mathf.RoundToInt(Mathf.Sign(xChange));
            Vector3Int tilePositionToCheckUp = new Vector3Int(Mathf.RoundToInt(proposedPosition.x + (xDir * (boxColliderSizeX / 2f + BUFFER))), Mathf.RoundToInt(currentPosition.y + boxColliderSizeY / 2f), 0);
            Vector3Int tilePositionToCheckDown = new Vector3Int(Mathf.RoundToInt(proposedPosition.x + (xDir * (boxColliderSizeX / 2f + BUFFER))), Mathf.RoundToInt(currentPosition.y - boxColliderSizeY / 2f), 0);

            if (xChange < 0)
            {
                if (CheckForStairsTile(tilePositionToCheckUp, tileLayer, BuildRotation.Right) &&
                    CheckForStairsTile(tilePositionToCheckDown, tileLayer, BuildRotation.Right))
                {
                    goalTile = new Vector3Int(Mathf.RoundToInt(currentPosition.x - 2), Mathf.RoundToInt(currentPosition.y + 1), 0);
                    return true;
                }
                else if (CheckForStairsTile(new Vector3Int(tilePositionToCheckUp.x, tilePositionToCheckUp.y - 1, 0), tileLayer - 1, BuildRotation.Left) &&
                         CheckForStairsTile(new Vector3Int(tilePositionToCheckDown.x, tilePositionToCheckDown.y - 1, 0), tileLayer - 1, BuildRotation.Left))
                {
                    goalTile = new Vector3Int(Mathf.RoundToInt(currentPosition.x - 2), Mathf.RoundToInt(currentPosition.y - 1), 0);
                    return true;
                }
            }
            else if (xChange > 0)
            {
                if (CheckForStairsTile(tilePositionToCheckUp, tileLayer, BuildRotation.Left) &&
                    CheckForStairsTile(tilePositionToCheckDown, tileLayer, BuildRotation.Left))
                {
                    goalTile = new Vector3Int(Mathf.RoundToInt(currentPosition.x + 2), Mathf.RoundToInt(currentPosition.y + 1), 0);
                    return true;
                }
                else if (CheckForStairsTile(new Vector3Int(tilePositionToCheckUp.x, tilePositionToCheckUp.y - 1, 0), tileLayer - 1, BuildRotation.Right) &&
                         CheckForStairsTile(new Vector3Int(tilePositionToCheckDown.x, tilePositionToCheckDown.y - 1, 0), tileLayer - 1, BuildRotation.Right))
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
            if (yChange > 0)
            {
                if (CheckForStairsTile(tilePositionToCheckLeft, tileLayer, BuildRotation.Front) && 
                    CheckForStairsTile(tilePositionToCheckRight, tileLayer, BuildRotation.Front))
                {
                    goalTile = new Vector3Int(Mathf.RoundToInt(currentPosition.x), Mathf.RoundToInt(currentPosition.y + 3), 0);
                    return true;
                }
                else if (CheckForStairsTile(tilePositionToCheckLeft, tileLayer - 1, BuildRotation.Back) &&
                         CheckForStairsTile(tilePositionToCheckRight, tileLayer - 1, BuildRotation.Back))
                {
                    goalTile = new Vector3Int(Mathf.RoundToInt(currentPosition.x), Mathf.RoundToInt(currentPosition.y + 2), 0);
                    return true;
                }

            }
            else if (yChange < 0)
            {
                if (CheckForStairsTile(new Vector3Int(tilePositionToCheckLeft.x, tilePositionToCheckLeft.y - 1, 0), tileLayer - 1, BuildRotation.Front) &&
                    CheckForStairsTile(new Vector3Int(tilePositionToCheckRight.x, tilePositionToCheckRight.y - 1, 0), tileLayer - 1, BuildRotation.Front))
                {
                    goalTile = new Vector3Int(Mathf.RoundToInt(currentPosition.x), Mathf.RoundToInt(currentPosition.y - 3), 0);
                    return true;
                }
                else if (CheckForStairsTile(tilePositionToCheckLeft, tileLayer, BuildRotation.Back) &&
                         CheckForStairsTile(tilePositionToCheckRight, tileLayer, BuildRotation.Back))
                {
                    goalTile = new Vector3Int(Mathf.RoundToInt(currentPosition.x), Mathf.RoundToInt(currentPosition.y - 2), 0);
                    return true;
                }
            }
        }

        goalTile = new Vector3Int();
        return false;
    }

    public static bool CheckForStairsTile(Vector3Int tilePosition, int tileLayer, BuildRotation rotation)
    {     
        TileInformation tile = TileInformationManager.Instance.GetTileInformation(tilePosition);

        if (tile == null)
            return false;

        if (tileLayer != tile.layerNum)
            return false;

        BuildOnTile build = tile.TopMostBuild;

        if (build == null)
            return false;

        return (build.BuildInfo.GetType() == typeof(StairsVariant) && build.Rotation == rotation);
    }
}