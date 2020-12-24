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

    public NPCIdleState(NPCInstance npcInstance): base(npcInstance)
    {
        animator = npcGameObject.GetComponent<Animator>();
    }

    public override void StartState(object[] args)
    {
        Vector2Int position = new Vector2Int(Mathf.RoundToInt(npcInstance.npcTransform.position.x), Mathf.RoundToInt(npcInstance.npcTransform.position.y));
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
                float proposedX = npcInstance.npcTransform.position.x + xOffset;
                float proposedY = npcInstance.npcTransform.position.y + yOffset;
                target = new Vector2(proposedX, proposedY);


                idleMoving = true;
                moveStartPos = npcInstance.npcTransform.position;
                travelDistance = Vector2.Distance(moveStartPos, target);

                {
                    npcInstance.npcDirection.SetDirectionOnMove(target - moveStartPos);
                    animator.SetBool("Walking", xOffset != 0 || yOffset != 0);
                }
            }
        }
        else
        {
            Vector2 currentPos = npcInstance.npcTransform.position;
            Vector2 proposedPos = Vector2.MoveTowards(currentPos, target, npcInstance.moveSpeed * Time.deltaTime);

            bool stop = false;
            if (CollisionManager.CheckForCollisionMovement(currentPos, proposedPos, tileLayer, out bool collisionX, out bool collisionY))
            {
                stop = true;
            }
            else
            {
                //Actual movement
                //TODO: Fix depth not working very well
                npcInstance.npcTransform.position = new Vector3(proposedPos.x, proposedPos.y, DynamicZDepth.GetDynamicZDepth(proposedPos, DynamicZDepth.NPC_OFFSET));

                if (Vector2.Distance(moveStartPos, npcInstance.npcTransform.position) >= travelDistance)
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
