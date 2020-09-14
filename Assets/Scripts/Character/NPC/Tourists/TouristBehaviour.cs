using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouristBehaviour : MonoBehaviour
{
    NPCState state;
    Transform npcTransform;
    Animator animator;
    private float moveSpeed = 1;
    private int tileLayer;

    private void Awake()
    {
        SwitchState(NPCState.idle);

        npcTransform = transform;
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        Vector2Int position = new Vector2Int(Mathf.RoundToInt(npcTransform.position.x), Mathf.RoundToInt(npcTransform.position.x));
        TileInformation info = TileInformationManager.Instance.GetTileInformation(new Vector3Int(position.x, position.y, 0));
        tileLayer = info.layerNum;
    }

    private void Update()
    {
        switch (state)
        {
            case (NPCState.idle):
                IdleMovement();
                break;

            default:
                break;
        }
    }

    private void SwitchState(NPCState proposedState)
    {
        state = proposedState;

        switch (proposedState)
        {
            case (NPCState.idle):
                idleNextMovementTimer = Random.Range(minMovementWait, maxMovementWait);
                break;
            default:
                break;
        }
    }

    private float idleNextMovementTimer = 0;
    private bool idleMoving = false;
    private Vector2 target;
    private Vector2 moveStartPos;
    private float travelDistance;

    private float minMovementWait = 1f;
    private float maxMovementWait = 2f;

    private void IdleMovement()
    {
        if (!idleMoving)
        {
            idleNextMovementTimer -= Time.deltaTime;

            if (idleNextMovementTimer <= 0)
            {
                float xOffset = Random.Range(-1f, 1f);
                float yOffset = Random.Range(-1f, 1f);
                Vector2 offsetVector = new Vector2(xOffset, yOffset);
                float proposedX = npcTransform.position.x + xOffset;
                float proposedY = npcTransform.position.y + yOffset;
                target = new Vector2(proposedX, proposedY);


                idleMoving = true;
                moveStartPos = npcTransform.position;
                travelDistance = Vector2.Distance(moveStartPos, target);

                //Animation
                {
                    if (xOffset != 0)
                        npcTransform.localScale = new Vector3(Mathf.Sign(xOffset), 1, 1);
                    if (yOffset != 0)
                        animator.SetFloat("Vertical", Mathf.Sign(yOffset));
                    animator.SetBool("Walking", xOffset != 0 || yOffset != 0);
                }
            }
        }
        else
        {
            Vector2 currentPos = npcTransform.position;
            Vector2 proposedPos = Vector2.MoveTowards(currentPos, target, moveSpeed * Time.deltaTime);

            bool stop = false;
            if (CollisionManager.CheckForCollisionMovement(currentPos, proposedPos, tileLayer, out bool collisionX, out bool collisionY))
            {
                stop = true;
            }
            else
            {
                //Actual movement
                //TODO: Fix depth not working very well
                npcTransform.position = new Vector3(proposedPos.x, proposedPos.y, DynamicZDepth.GetDynamicZDepth(proposedPos, DynamicZDepth.NPC_OFFSET));

                if (Vector2.Distance(moveStartPos, npcTransform.position) >= travelDistance)
                    stop = true;
            }

            if (stop)
            {
                animator.SetBool("Walking", false);
                idleMoving = false;
                idleNextMovementTimer = Random.Range(minMovementWait, maxMovementWait);
            }
        }
    }
}

public enum NPCState
{
    idle
}