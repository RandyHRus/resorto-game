using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDirection : MonoBehaviour
{
    public CharacterVisualDirection VisualDirection { get; private set; }

    private Animator animator;

    private static PlayerDirection _instance;
    public static PlayerDirection Instance { get { return _instance; } }
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

    private void Start()
    {
        VisualDirection = new CharacterVisualDirection(transform);
        animator = GetComponent<Animator>();

        PlayerMovement.PlayerMoved += (Vector2 pos, bool slow, Vector2 previousPos) => VisualDirection.SetDirectionOnMove(pos - previousPos);
    }

    private void Update()
    {
        //Point to direction of mouse on mouseclick
        if (CheckMouseOverUI.GetButtonDownAndNotOnUI("Primary"))
        {
            SetDirectionToMouse();
        }
    }

    private void SetDirectionToMouse()
    {
        float angle = MathFunctions.GetAngleBetweenPoints(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));

        if (angle < 180)
        {
            if (angle < 90 || angle > 270)
                VisualDirection.SetDirection(CharacterVisualDirectionEnum.BackRight);
            else
                VisualDirection.SetDirection(CharacterVisualDirectionEnum.BackLeft);

        }
        else {
            if (angle < 90 || angle > 270)
                VisualDirection.SetDirection(CharacterVisualDirectionEnum.FrontRight);
            else
                VisualDirection.SetDirection(CharacterVisualDirectionEnum.FrontLeft);
        }
    }

    public CharacterTwoAxisDirection GetDirectionToMouse()
    {
        float angle = MathFunctions.GetAngleBetweenPoints(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));

        if (angle > 315 || angle < 45)
        {
            return new CharacterTwoAxisDirection(CharacterTwoAxisDirectionEnum.Right);
        }
        else if (angle >= 45 && angle <= 135)
        {
            return new CharacterTwoAxisDirection(CharacterTwoAxisDirectionEnum.Back);
        }
        else if (angle > 135 && angle < 225)
        {
            return new CharacterTwoAxisDirection(CharacterTwoAxisDirectionEnum.Left);
        }
        else if (angle >= 225 && angle <= 315)
        {
            return new CharacterTwoAxisDirection(CharacterTwoAxisDirectionEnum.Front);
        }
        else
        {
            throw new System.Exception("Invalid angle");
        }
    }
}
