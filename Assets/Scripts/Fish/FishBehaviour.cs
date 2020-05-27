using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBehaviour : MonoBehaviour
{
    private Animator anim;
    private SpriteRenderer sprRenderer;
    private Transform fishTransform;
    private FishState state = FishState.Idle;

    private float fadeOutSpeed = 0.5f;
    private float fadeInSpeed = 1f;
    private float maxAlpha = 0.6f;

    private float fleeingDistance = 0.7f; //Fish will flee if target (bobber) lands this close (or closer) to it
    private static float maxSeeingDistance = FishManager.FISH_SEEING_DISTANCE; //Fish can only see target this close to it

    private float fleeSpeed = 1f;

    private float maxIdleTurnSpeed = 10f;
    private float idleRotationAcceleration = 0.2f;
    private float minMoveSwitchTime = 3f, maxMoveSwitchTime = 5f; //Time to switch accelerating or decelerating

    private float minIdleMoveSpeed = 0.2f;
    private float maxIdleMoveSpeed = 0.8f;
    private float idleMoveAcceleration = 0.01f;
    private float minRotationSwitchTime = 3f, maxRotationSwitchTime = 20f; //Time to switch which direction rotating

    bool fadeOutWaiting = false;
    bool fadeInRunning = false;

    private Vector2 target; //Ex. bobber

    public void ChangeTarget(Vector2 target) //Ex. bobber location changed
    {
        this.target = target;

        float distanceToTarget = Vector2.Distance(fishTransform.position, target);
        if (distanceToTarget <= fleeingDistance)
        {
            TrySwitchState(FishState.Fleeing, null);
        }
        else if (distanceToTarget <= maxSeeingDistance)
        {
            TrySwitchState(FishState.Targeting, new object[] { target });
        }
    }

    private void Awake()
    {
        sprRenderer = GetComponent<SpriteRenderer>();
        fishTransform = transform;
        anim = GetComponent<Animator>();

        StartCoroutine(FadeIn());
        TrySwitchState(FishState.Idle, null);
    }

    Coroutine currentStateCoroutine;

    private bool TrySwitchState(FishState proposedState, object[] args)
    {
        if (state == FishState.Fleeing)
        {
            return false;
        }

        if (currentStateCoroutine != null)
            StopCoroutine(currentStateCoroutine);

        switch (proposedState) {  
            
            case (FishState.Idle):
                currentStateCoroutine = StartCoroutine(IdleMovement());
                break;

            case (FishState.Fleeing):
                currentStateCoroutine = StartCoroutine(Fleeing());
                break;

            case (FishState.Targeting):
                currentStateCoroutine = StartCoroutine(Targeting((Vector2)args[0]));
                break;

            default:
                break;
        }

        return true;
    }

    IEnumerator Targeting(Vector2 target)
    {
        while (true)
        {

            yield return 0;
        }
    }

    IEnumerator IdleMovement()
    {
        float moveSwitchTimer = Random.Range(minMoveSwitchTime, maxMoveSwitchTime);
        float currentMoveSpeed = minIdleMoveSpeed;         //Can vary between 0 to maxIdleMoveSpeed
        bool accelerating = true;            //accelerating or decelerating

        float rotationSwitchTimer = Random.Range(minRotationSwitchTime, maxRotationSwitchTime);
        float currentRotatingSpeed = 0f;     //Can vary between -maxIdleTurnSpeed to +maxIdleTurnSpeed
        bool rotatingCW = true;              //True - CW, false - CCW       

        while (true)
        {
            //Change move acceleration direction
            moveSwitchTimer -= Time.deltaTime;
            if (moveSwitchTimer <= 0)
            {
                moveSwitchTimer = Random.Range(minMoveSwitchTime, maxMoveSwitchTime);
                accelerating = !accelerating;
            }

            //Change rotation direction
            rotationSwitchTimer -= Time.deltaTime;
            if (rotationSwitchTimer <= 0)
            {
                rotationSwitchTimer = Random.Range(minRotationSwitchTime, maxRotationSwitchTime);
                rotatingCW = !rotatingCW;
            }

            //Change move speed
            if (accelerating)
            {
                if (currentMoveSpeed < maxIdleMoveSpeed)
                { //Accelerate
                    currentMoveSpeed += idleMoveAcceleration;
                    if (currentMoveSpeed > maxIdleMoveSpeed)
                    {
                        currentMoveSpeed = maxIdleMoveSpeed;
                    }

                    anim.speed = (currentMoveSpeed / maxIdleMoveSpeed);
                }
            }
            else
            {
                if (currentMoveSpeed > minIdleMoveSpeed)
                { //Decelerate
                    currentMoveSpeed -= idleMoveAcceleration;
                    if (currentMoveSpeed < minIdleMoveSpeed)
                    {
                        currentMoveSpeed = minIdleMoveSpeed;
                    }

                    anim.speed = (currentMoveSpeed / maxIdleMoveSpeed);
                }
            }

            //Change rotation speed
            if (rotatingCW)
            {
                if (currentRotatingSpeed < maxIdleTurnSpeed)
                { //CW
                    currentRotatingSpeed += idleRotationAcceleration;
                    if (currentRotatingSpeed > maxIdleTurnSpeed)
                    {
                        currentRotatingSpeed = maxIdleTurnSpeed;
                    }
                }
            }
            else
            {
                if (currentRotatingSpeed > -maxIdleTurnSpeed)
                { //CCW
                    currentRotatingSpeed -= idleRotationAcceleration;
                    if (currentRotatingSpeed < -maxIdleTurnSpeed)
                    {
                        currentRotatingSpeed = -maxIdleTurnSpeed;
                    }
                }
            }

            transform.Rotate(Vector3.forward * Time.deltaTime * currentRotatingSpeed);
            transform.Translate(Vector3.left * Time.deltaTime * currentMoveSpeed);

            yield return 0;
        }
    }

    IEnumerator Fleeing()
    {
        TryStartFadeOut();
        while (true)
        {
            transform.Translate(Vector3.left * Time.deltaTime * fleeSpeed);
            yield return 0;
        }
    }

    #region fade

    //Tries to start fade out process, but if not fully faded in, wait for it to fishish fading in
    public void TryStartFadeOut()
    {
        if (fadeInRunning)
        {
            fadeOutWaiting = true;
        }
        else
        {
            StartCoroutine(FadeOutAndDestroy());
        }
    }

    IEnumerator FadeOutAndDestroy()
    {
        Color32 color = sprRenderer.color;
        float alpha = (color.a / 255f);

        while (true) {
            alpha -= fadeOutSpeed * Time.deltaTime;

            if (alpha <= 0)
            {
                Destroy(gameObject);
                break;
            }
            else
            {
                color.a = (byte)(alpha * 255);
                sprRenderer.color = color;
                yield return 0;
            }
        }
    }

    IEnumerator FadeIn()
    {
        fadeInRunning = true;
        Color32 color = sprRenderer.color;
        float alpha = 0;

        while (true)
        {
            alpha += fadeInSpeed * Time.deltaTime;
            if (alpha >= maxAlpha)
                alpha = maxAlpha;

            color.a = (byte)(alpha * 255);
            sprRenderer.color = color;

            if (alpha == maxAlpha)
            {
                fadeInRunning = false;
                if (fadeOutWaiting)
                {
                    StartCoroutine(FadeOutAndDestroy());
                }
                break;
            }
            else
                yield return 0;
        }
    }
    #endregion

    private enum FishState
    {
        Idle,
        Fleeing,
        Targeting
    }
}

