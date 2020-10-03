using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBehaviour : WildlifeBehaviour
{
    private FishState state = FishState.Idle;

    public override float TargetAlpha { get { return 0.6f; } }

    private float fleeingDistance = 0.7f; //Fish will flee if target (bobber) lands this close (or closer) to it
    private float maxSeeingDistance = FishInformation.FISH_SEEING_DISTANCE; //Fish can only see target this close to it
    private float fishSeeingAngleDot = FishInformation.FISH_SEEING_ANGLE_DOT_PRODUCT;

    private float fleeSpeed = 1f;

    private float maxIdleTurnSpeed = 20f;
    private float idleRotationAcceleration = 0.5f;
    private float minRotationSwitchTime = 3f, maxRotationSwitchTime = 20f; //Time to switch which direction rotating

    private float minIdleAnimationSpeed = 0.2f;
    private float minIdleMoveSpeed = 0f;
    private float maxIdleMoveSpeed = 0.4f;
    private float idleMoveAcceleration = 0.2f;
    private float minMoveSwitchTime = 4f, maxMoveSwitchTime = 8f; //Time to switch accelerating or decelerating

    private float hookedTuggingSize = 0.07f;
    private float hookedTuggingSpeed = 8f;

    public delegate void Hooked(FishBehaviour fish);
    public event Hooked OnHooked;

    public void SetTarget(Vector2 target) //Ex. bobber location changed
    {
        if (state != FishState.Targeting)
        {
            float distanceToTarget = Vector2.Distance(Transform.position, target);
            if (distanceToTarget <= fleeingDistance)
            {
                TrySwitchState(FishState.Fleeing, null);
            }
            else if (distanceToTarget <= maxSeeingDistance)
            {
                Vector2 targetDir = (target - (Vector2)Transform.position).normalized;
                Vector2 forward = (-Transform.right).normalized;

                //Fish fov is -45 to 45 degrees, alert fish if bobber in sight of fish
                if (Vector2.Dot(forward, targetDir) > fishSeeingAngleDot) //0.707 dot product of 2 unit vectors represents 45 degrees, 
                {
                    TrySwitchState(FishState.Targeting, new object[] { target });
                }
            }
        }
    }

    public void TryStartFlee()
    {
        TrySwitchState(FishState.Fleeing, null);
    }

    public override void Initialize()
    {
        TrySwitchState(FishState.Idle, null);
    }

    private bool TrySwitchState(FishState proposedState, object[] args)
    {
        if (state == FishState.Fleeing)
        {
            return false;
        }

        state = proposedState;

        switch (proposedState) {  
            
            case (FishState.Idle):
                {
                    StartCoroutine(IdleMovement());
                    break;
                }
            case (FishState.Fleeing):
                {
                    Animator.speed = 1;
                    StartFadeOutAndDestroy();
                    StartCoroutine(Fleeing());
                    break;
                }
            case (FishState.Targeting):
                {
                    StartCoroutine(Targeting((Vector2)args[0]));
                    break;
                }
            case (FishState.Hooked):
                {
                    OnHooked?.Invoke(this);
                    Animator.speed = 1.3f;
                    StartCoroutine(HookedEnum());
                    break;
                }
            default:
                break;
        }

        return true;
    }

    IEnumerator Targeting(Vector2 target)
    {
        float currentMoveSpeed = 0;
        //Turn to target
        {
            Vector2 vectorToTarget = target - (Vector2)Transform.position;
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
            Quaternion q = Quaternion.AngleAxis(angle + 180, Vector3.forward);
            transform.rotation = q;
        }

        float distanceToTravel = Vector2.Distance(target, Transform.position) - (FishInformation.FISH_WORLD_WIDTH / 2f);
        Vector2 startPos = Transform.position;
        Vector2 forward = (-Transform.right);

        yield return new WaitForSeconds(Random.Range(0.3f, 1f));

        //Move
        while (state == FishState.Targeting)
        {
            if (currentMoveSpeed < maxIdleMoveSpeed)
            {
                currentMoveSpeed += idleMoveAcceleration * Time.deltaTime;
                Animator.speed = ((currentMoveSpeed + minIdleAnimationSpeed) / (maxIdleMoveSpeed + minIdleAnimationSpeed));
            }

            if (currentMoveSpeed > maxIdleMoveSpeed)
                currentMoveSpeed = maxIdleMoveSpeed;


            float distanceTraveled = Vector2.Distance(Transform.position, startPos);
            Vector2 proposedMovement = forward * currentMoveSpeed * Time.deltaTime;

            if (distanceTraveled + proposedMovement.magnitude > distanceToTravel)
            {
                //Finished moving
                Transform.position = startPos + (forward * distanceToTravel);
                TrySwitchState(FishState.Hooked, null);
                break;
            }
            else
            {
                Transform.position = (Vector2)Transform.position + (proposedMovement);
            }
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

        Vector3Int currentTile = new Vector3Int(Mathf.RoundToInt(Transform.position.x), Mathf.RoundToInt(Transform.position.y), 0);

        while (state == FishState.Idle)
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

            //Change move speed (and animation speed)
            if (accelerating)
            {
                if (currentMoveSpeed < maxIdleMoveSpeed)
                { //Accelerate
                    currentMoveSpeed += idleMoveAcceleration * Time.deltaTime;
                    if (currentMoveSpeed > maxIdleMoveSpeed)
                    {
                        currentMoveSpeed = maxIdleMoveSpeed;
                    }

                    Animator.speed = ((currentMoveSpeed + minIdleAnimationSpeed) / (maxIdleMoveSpeed + minIdleAnimationSpeed));
                }
            }
            else
            {
                if (currentMoveSpeed > minIdleMoveSpeed)
                { //Decelerate
                    currentMoveSpeed -= idleMoveAcceleration * Time.deltaTime;
                    if (currentMoveSpeed < minIdleMoveSpeed)
                    {
                        currentMoveSpeed = minIdleMoveSpeed;
                    }

                    Animator.speed = ((currentMoveSpeed + minIdleAnimationSpeed) / (maxIdleMoveSpeed + minIdleAnimationSpeed));
                }
            }

            //Change rotation speed
            if (rotatingCW)
            {
                if (currentRotatingSpeed < maxIdleTurnSpeed)
                { //CW
                    currentRotatingSpeed += idleRotationAcceleration * Time.deltaTime;
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
                    currentRotatingSpeed -= idleRotationAcceleration * Time.deltaTime;
                    if (currentRotatingSpeed < -maxIdleTurnSpeed)
                    {
                        currentRotatingSpeed = -maxIdleTurnSpeed;
                    }
                }
            }

            if (currentRotatingSpeed > 0)
                Transform.Rotate(Vector3.forward * Time.deltaTime * currentRotatingSpeed);

            if (currentMoveSpeed > 0)
            {
                Vector2 change = (-Transform.right) * currentMoveSpeed * Time.deltaTime;
                Vector2 proposedPosition = (Vector2)Transform.position + change;
                Vector3Int proposedTile = new Vector3Int(Mathf.RoundToInt(proposedPosition.x), Mathf.RoundToInt(proposedPosition.y), 0);

                //Fade out if there is collision(land/sand) ahead
                if (proposedTile != currentTile)
                {
                    CheckForHeadingCollision(change, proposedTile, out bool Tile_h_collision, out bool tile_v_collision, out bool tile_hv_collision);
                    if (tile_hv_collision || tile_v_collision || tile_hv_collision)
                    {
                        StartFadeOutAndDestroy();
                    }
                }
                Transform.position = proposedPosition;
            }
            
            yield return 0;
        }
    }

    IEnumerator Fleeing()
    {
        while (state == FishState.Fleeing)
        {
            transform.Translate(Vector3.left * Time.deltaTime * fleeSpeed);
            yield return 0;
        }
    }

    IEnumerator HookedEnum()
    {
        Vector2 forward = (-Transform.right);
        Vector2 defaultPosition = Transform.position;

        while (state == FishState.Hooked)
        {
            float offset = Mathf.Sin(hookedTuggingSpeed * Time.time) * hookedTuggingSize;
            Transform.position = defaultPosition + (forward * offset);

            yield return 0;
        }
    }

    private void CheckForHeadingCollision(Vector2 headingVector, Vector3Int tile, out bool tile_h_collision, out bool tile_v_collision, out bool tile_hv_collision)
    {
        TileInformation tile_h = null, tile_v = null, tile_hv = null; //Tiles next to tile

        int signX = (int)Mathf.Sign(headingVector.x);
        int signY = (int)Mathf.Sign(headingVector.y);
        if (headingVector.x != 0)
        {
            tile_h = TileInformationManager.Instance.GetTileInformation(new Vector3Int(tile.x + signX, tile.y, 0));
        }
        if (headingVector.y != 0)
        {
            tile_v = TileInformationManager.Instance.GetTileInformation(new Vector3Int(tile.x, tile.y + signY, 0));
        }
        if (headingVector.x != 0 && headingVector.y != 0)
        {
            tile_hv = TileInformationManager.Instance.GetTileInformation(new Vector3Int(tile.x + signX, tile.y + signY, 0));
        }

        tile_h_collision = (tile_h == null || tile_h.tileLocation != TileLocation.DeepWater);
        tile_v_collision = (tile_v == null || tile_v.tileLocation != TileLocation.DeepWater);
        tile_hv_collision = (tile_hv == null || tile_hv.tileLocation != TileLocation.DeepWater);
    }


    #region fade

    public override void Despawn()
    {
        if (state == FishState.Targeting || state == FishState.Hooked)
            return;

        StartFadeOutAndDestroy();
    }

    #endregion

    private enum FishState
    {
        Idle,
        Fleeing,
        Targeting,
        Hooked
    }
}

