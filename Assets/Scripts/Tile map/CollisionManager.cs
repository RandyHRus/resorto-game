using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager
{
    public static float boxColliderSizeX = 0.625f;
    public static float boxColliderSizeY = 0.2f;
    public static float BUFFER = 1 / 16f;

    //Check if character will collide if moved to a proposed position
    public static bool CheckForCollisionMovement(Vector2 currentPosition, Vector2 proposedPosition, int tileLayer, out bool collisionX, out bool collisionY)
    {
        float xChange = proposedPosition.x - currentPosition.x;
        float yChange = proposedPosition.y - currentPosition.y;

        collisionX = false;
        collisionY = false;

        if (xChange != 0)
        {
            int xDir = Mathf.RoundToInt(Mathf.Sign(xChange));
            Vector2Int tilePositionToCheckUp = new Vector2Int(Mathf.RoundToInt(proposedPosition.x + (xDir * (boxColliderSizeX / 2f + BUFFER))), Mathf.RoundToInt(currentPosition.y + boxColliderSizeY / 2f));
            Vector2Int tilePositionToCheckDown = new Vector2Int(Mathf.RoundToInt(proposedPosition.x + (xDir * (boxColliderSizeX / 2f + BUFFER))), Mathf.RoundToInt(currentPosition.y - boxColliderSizeY / 2f));

            if (CheckForCollisionOnTile(tilePositionToCheckUp, tileLayer) || CheckForCollisionOnTile(tilePositionToCheckDown, tileLayer))
            {
                collisionX = true;
            }
        }

        if (yChange != 0)
        {
            int yDir = Mathf.RoundToInt(Mathf.Sign(yChange));
            Vector2Int tilePositionToCheckLeft = new Vector2Int(Mathf.RoundToInt(currentPosition.x - boxColliderSizeX / 2f), Mathf.RoundToInt(proposedPosition.y + (yDir * (boxColliderSizeY / 2f + BUFFER))));
            Vector2Int tilePositionToCheckRight = new Vector2Int(Mathf.RoundToInt(currentPosition.x + boxColliderSizeX / 2f), Mathf.RoundToInt(proposedPosition.y + (yDir * (boxColliderSizeY / 2f + BUFFER))));

            if (CheckForCollisionOnTile(tilePositionToCheckLeft, tileLayer) || CheckForCollisionOnTile(tilePositionToCheckRight, tileLayer))
            {
                collisionY = true;
            }
        }

        return (collisionX || collisionY);
    }


    public static bool CheckForCollisionOnTile(Vector2Int tilePosition, int tileLayer)
    {
        if (!TileInformationManager.Instance.TryGetTileInformation(tilePosition, out TileInformation tile))
            return true;

        if (tile.BuildCollision) //TODO could be changed to precise collision checking?
            return true;

        //If water and no dock, there is collision
        if (TileLocation.DeepWater.HasFlag(tile.tileLocation))
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

    public static bool CheckForStairsMovement(Vector2 currentPosition, Vector2 proposedPosition, int tileLayer, out StairsStartPosition stairsStartPosition)
    {
        float xChange = proposedPosition.x - currentPosition.x;
        float yChange = proposedPosition.y - currentPosition.y;

        TileInformationManager.Instance.TryGetTileInformation(
            new Vector2Int(Mathf.RoundToInt(currentPosition.x), Mathf.RoundToInt(currentPosition.y)), out TileInformation currentTilePosition);

        if (xChange != 0)
        {
            int xDir = Mathf.RoundToInt(Mathf.Sign(xChange));

            TileInformationManager.Instance.TryGetTileInformation(
                new Vector2Int(Mathf.RoundToInt(currentPosition.x + (xDir * (boxColliderSizeX / 2f + BUFFER))), Mathf.RoundToInt(currentPosition.y + boxColliderSizeY / 2f)), out TileInformation currentTilePositionToCheckUp);
            TileInformationManager.Instance.TryGetTileInformation(
                new Vector2Int(Mathf.RoundToInt(currentPosition.x + (xDir * (boxColliderSizeX / 2f + BUFFER))), Mathf.RoundToInt(currentPosition.y - boxColliderSizeY / 2f)), out TileInformation currentTilePositionCheckDown);

            if (currentTilePositionToCheckUp?.StairsStartPositions.Count > 0 &&
                currentTilePositionCheckDown?.StairsStartPositions.Count > 0)
            {
                Direction playerXMoveDirection = xChange > 0 ? Direction.Right : Direction.Left;

                if (currentTilePositionToCheckUp.StairsStartPositionWithDirectionExists(playerXMoveDirection, out StairsStartPosition pos1))
                {
                    if (currentTilePositionCheckDown.StairsStartPositionWithDirectionExists(playerXMoveDirection, out StairsStartPosition pos2))
                    {
                        if (currentTilePosition.StairsStartPositionWithDirectionExists(playerXMoveDirection, out StairsStartPosition pos3))
                        {
                            stairsStartPosition = pos3;
                            return true;
                        }
                    }
                }
            }
        }

        if (yChange != 0)
        {
            int yDir = Mathf.RoundToInt(Mathf.Sign(yChange));
            TileInformationManager.Instance.TryGetTileInformation(
                new Vector2Int(Mathf.RoundToInt(currentPosition.x - boxColliderSizeX / 2f), Mathf.RoundToInt(currentPosition.y + (yDir * (boxColliderSizeY / 2f + BUFFER)))), out TileInformation currentTilePositionToCheckLeft);
            TileInformationManager.Instance.TryGetTileInformation(
                new Vector2Int(Mathf.RoundToInt(currentPosition.x + boxColliderSizeX / 2f), Mathf.RoundToInt(currentPosition.y + (yDir * (boxColliderSizeY / 2f + BUFFER)))), out TileInformation currentTilePositionToCheckRight);

            if (currentTilePositionToCheckLeft?.StairsStartPositions.Count > 0 &&
                currentTilePositionToCheckRight?.StairsStartPositions.Count > 0)
            {
                Direction playerYMoveDirection = yChange > 0 ? Direction.Up : Direction.Down;

                if (currentTilePositionToCheckLeft.StairsStartPositionWithDirectionExists(playerYMoveDirection, out StairsStartPosition pos1))
                {
                    if (currentTilePositionToCheckRight.StairsStartPositionWithDirectionExists(playerYMoveDirection, out StairsStartPosition pos2))
                    {
                        if (currentTilePosition.StairsStartPositionWithDirectionExists(playerYMoveDirection, out StairsStartPosition pos3))
                        {
                            stairsStartPosition = pos3;
                            return true;
                        }
                    }
                }
            }
        }

        stairsStartPosition = default;
        return false;
    }
}