using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public static class DirectionMethods
{
    public static Vector2Int DirectionVector(this Direction d)
    {
        switch (d)
        {
            case (Direction.Up):
                return new Vector2Int(0, 1);
            case (Direction.Down):
                return new Vector2Int(0, -1);
            case (Direction.Left):
                return new Vector2Int(-1, 0);
            case (Direction.Right):
                return new Vector2Int(1, 0);
            default:
                throw new System.NotImplementedException();
        }
    }
}