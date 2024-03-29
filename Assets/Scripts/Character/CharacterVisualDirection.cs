﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterVisualDirection
{
    public Vector2 DirectionVector { get; private set; }

    private Transform characterTransform;
    private Animator characterAnimator;

    public CharacterVisualDirection(Transform characterTransform)
    {
        this.characterTransform = characterTransform;
        characterAnimator = characterTransform.GetComponent<Animator>();

        SetDirection(CharacterVisualDirectionEnum.FrontRight);
    }

    public void SetDirection(CharacterVisualDirectionEnum directionEnum)
    {
        switch (directionEnum)
        {
            case (CharacterVisualDirectionEnum.BackLeft):
                DirectionVector = new Vector2(-1, 1);
                break;
            case (CharacterVisualDirectionEnum.BackRight):
                DirectionVector = new Vector2(1, 1);
                break;
            case (CharacterVisualDirectionEnum.FrontLeft):
                DirectionVector = new Vector2(-1, -1);
                break;
            case (CharacterVisualDirectionEnum.FrontRight):
                DirectionVector = new Vector2(1, -1);
                break;
            case (CharacterVisualDirectionEnum.Left):
                DirectionVector = new Vector2(-1, 0);
                break;
            case (CharacterVisualDirectionEnum.Right):
                DirectionVector = new Vector2(1, 0);
                break;
            default:
                throw new System.Exception("Unknown direction!");
        }

        UpdateCharacter();
    }

    public void SetDirectionOnMove(Vector2 moveVector)
    {
        Vector2 currentVector = DirectionVector;
        Vector2 proposedVector = new Vector2();

        if (moveVector.x == 0)
            proposedVector.x = currentVector.x;
        else
            proposedVector.x = Mathf.Sign(moveVector.x);

        proposedVector.y = moveVector.y == 0 ? 0 : Mathf.Sign(moveVector.y); //This can be 0, horizontal cannot be 0

        DirectionVector = proposedVector;

        UpdateCharacter();
    }

    private void UpdateCharacter()
    {
        characterTransform.localScale = new Vector3(DirectionVector.x, 1, 1);
        characterAnimator.SetFloat("Vertical", DirectionVector.y);
    }
}

public enum CharacterVisualDirectionEnum
{
    BackLeft,
    BackRight,
    FrontLeft,
    FrontRight,
    Left,
    Right
}
