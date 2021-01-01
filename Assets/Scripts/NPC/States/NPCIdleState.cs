using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPCIdleState : NPCState
{
    private Animator animator;
    private int tileLayer;

    private float idleNextMovementTimer = 0;

    private bool idleMoving = false;
    private Vector2 target;
    private Vector2 moveStartPos;
    private float travelDistance;

    private readonly float minMovementWait = 1f;
    private readonly float maxMovementWait = 2f;

    public override string DisplayMessage => "Idle";

    public NPCIdleState(NPCComponents npcComponents): base(npcComponents)
    {
        animator = npcGameObject.GetComponent<Animator>();
    }

    public override void StartState(object[] args)
    {
        Vector2Int position = new Vector2Int(Mathf.RoundToInt(npcComponents.npcTransform.position.x), Mathf.RoundToInt(npcComponents.npcTransform.position.y));
        TileInformationManager.Instance.TryGetTileInformation(position, out TileInformation info);
        tileLayer = info.layerNum;

        idleNextMovementTimer = Random.Range(minMovementWait, maxMovementWait);
    }

    public override void Execute()
    {
        if (!idleMoving)
        {
            idleNextMovementTimer -= Time.deltaTime;

            if (idleNextMovementTimer <= 0)
            {
                float xOffset = Random.Range(-1f, 1f);
                float yOffset = Random.Range(-1f, 1f);
                Vector2 offsetVector = new Vector2(xOffset, yOffset);
                float proposedX = npcComponents.npcTransform.position.x + xOffset;
                float proposedY = npcComponents.npcTransform.position.y + yOffset;
                target = new Vector2(proposedX, proposedY);


                idleMoving = true;
                moveStartPos = npcComponents.npcTransform.position;
                travelDistance = Vector2.Distance(moveStartPos, target);

                {
                    npcComponents.npcDirection.SetDirectionOnMove(target - moveStartPos);
                    animator.SetBool("Walking", xOffset != 0 || yOffset != 0);
                }
            }
        }
        else
        {
            Vector2 currentPos = npcComponents.npcTransform.position;
            Vector2 proposedPos = Vector2.MoveTowards(currentPos, target, npcComponents.moveSpeed * Time.deltaTime);

            bool stop = false;
            if (CollisionManager.CheckForCollisionMovement(currentPos, proposedPos, tileLayer, out bool collisionX, out bool collisionY))
            {
                stop = true;
            }
            else
            {
                //Actual movement
                //TODO: Fix depth not working very well
                npcComponents.npcTransform.position = new Vector3(proposedPos.x, proposedPos.y, DynamicZDepth.GetDynamicZDepth(proposedPos, DynamicZDepth.NPC_OFFSET));

                if (Vector2.Distance(moveStartPos, npcComponents.npcTransform.position) >= travelDistance)
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

    public override void EndState()
    {
        
    }

}
