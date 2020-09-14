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

        PlayerMovement.PlayerMoved += (Vector2 pos, bool slow, Vector2 directionVector) => VisualDirection.SetDirectionOnMove(directionVector);
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

        if (angle <= 45 || angle >= 315)
        {
            VisualDirection.SetDirection(CharacterVisualDirectionEnum.Right);
        }
        else if (angle >= 135 && angle <= 215)
        {
            VisualDirection.SetDirection(CharacterVisualDirectionEnum.Left);
        }
        else if (angle < 180)
        {
            if (angle < 90)
                VisualDirection.SetDirection(CharacterVisualDirectionEnum.BackRight);
            else
                VisualDirection.SetDirection(CharacterVisualDirectionEnum.BackLeft);

        }
        else {
            if (angle > 270)
                VisualDirection.SetDirection(CharacterVisualDirectionEnum.FrontRight);
            else
                VisualDirection.SetDirection(CharacterVisualDirectionEnum.FrontLeft);
        }
    }
}
