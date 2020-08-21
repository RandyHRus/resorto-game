using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTwoAxisDirection
{
    public CharacterTwoAxisDirectionEnum DirectionEnum { get; private set; }
    public Vector2 DirectionVector { get; private set; }

    public CharacterTwoAxisDirection(CharacterTwoAxisDirectionEnum directionEnum)
    {
        this.DirectionEnum = directionEnum;
        switch (directionEnum)
        {
            case (CharacterTwoAxisDirectionEnum.Back):
                DirectionVector = new Vector2(0, -1);
                break;
            case (CharacterTwoAxisDirectionEnum.Front):
                DirectionVector = new Vector2(0, 1);
                break;
            case (CharacterTwoAxisDirectionEnum.Left):
                DirectionVector = new Vector2(-1, 0);
                break;
            case (CharacterTwoAxisDirectionEnum.Right):
                DirectionVector = new Vector2(1, 0);
                break;
            default:
                break;
        }
    }
}

public enum CharacterTwoAxisDirectionEnum
{
    Back,
    Front,
    Left,
    Right
}