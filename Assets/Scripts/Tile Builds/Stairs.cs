using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs
{
    public readonly StairsStartPosition startPositionBelow;
    public readonly StairsStartPosition startPositionAbove;

    public Stairs(Vector2Int pos, int layerNum, BuildRotation rotation)
    {
        Vector2Int belowStartTile, belowDirectionToStairs, aboveStartTile, aboveDirectionToStairs;

        switch (rotation)
        {
            case (BuildRotation.Back):
                {
                    belowStartTile = pos + new Vector2Int(0, 1);
                    belowDirectionToStairs = new Vector2Int(0, -1);

                    aboveStartTile = pos + new Vector2Int(0, -1);
                    aboveDirectionToStairs = new Vector2Int(0, 1);                   

                    break;
                }
            case (BuildRotation.Front):
                {
                    belowStartTile = pos + new Vector2Int(0, -1);
                    belowDirectionToStairs = new Vector2Int(0, 1);

                    aboveStartTile = pos + new Vector2Int(0, 2);
                    aboveDirectionToStairs = new Vector2Int(0, -1);
                    break;
                }
            case (BuildRotation.Left):
                {
                    belowStartTile = pos + new Vector2Int(-1, 0);
                    belowDirectionToStairs = new Vector2Int(1, 0);

                    aboveStartTile = pos + new Vector2Int(1, 1);
                    aboveDirectionToStairs = new Vector2Int(-1, 0);
                    break;
                }
            case (BuildRotation.Right):
                {
                    belowStartTile = pos + new Vector2Int(1, 0);
                    belowDirectionToStairs = new Vector2Int(-1, 0);

                    aboveStartTile = pos + new Vector2Int(-1, 1);
                    aboveDirectionToStairs = new Vector2Int(1, 0);
                }
                break;
            default:
                throw new System.Exception("Unknown rotation");
        }

        startPositionBelow = new StairsStartPosition(pos, belowStartTile, layerNum,     layerNum + 1, belowDirectionToStairs, aboveStartTile);
        startPositionAbove = new StairsStartPosition(pos, aboveStartTile, layerNum + 1, layerNum,     aboveDirectionToStairs, belowStartTile);
    }
}

public class StairsStartPosition
{
    public readonly Vector2Int startPosition;
    public readonly int startLayerNum, endLayerNum;
    public readonly Vector2Int directionToStairs;
    public readonly Vector2Int endPosition;
    public readonly Vector2Int stairsPosition;

    public StairsStartPosition(Vector2Int stairsPosition, Vector2Int startPosition, int startLayerNum, int endLayerNum, Vector2Int directionToStairs, Vector2Int endPosition)
    {
        this.stairsPosition = stairsPosition;
        this.startPosition = startPosition;
        this.startLayerNum = startLayerNum;
        this.endLayerNum = endLayerNum;
        this.directionToStairs = directionToStairs;
        this.endPosition = endPosition;
    }
}
