using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "NPCStates/WalkToPosition")]
public class NPCWalkToPositionState : NPCState
{
    public delegate void pathChanged(LinkedList<Vector2Int> newPath);
    public event pathChanged OnPathChanged;

    private Vector2Int goal;
    private LinkedList<Vector2Int> currentPath;
    private LinkedListNode<Vector2Int> currentTarget;
    private float moveSpeed = 4;
    private Animator animator;

    private float distanceToNextPositionTravelled, distanceToNextPosition;
    private Vector2 currentStartPos;
    private bool goalReached;

    private Type onFinishStateChange;
    private object[] onFinishStateChangeArgs;

    private string displayMessage;
    public override string DisplayMessage => displayMessage;

    public override void Initialize()
    {
        animator = npcGameObject.GetComponent<Animator>();
    }

    public override void StartState(object[] args)
    {
        goal = (Vector2Int)args[0];
        onFinishStateChange = (Type)args[1];
        onFinishStateChangeArgs = (object[])args[2];
        displayMessage = (string)args[3];

        animator.SetBool("Walking", true);

        RecalculatePath();
    }

    public override void Execute()
    {
        if (goalReached)
            return;

        if (distanceToNextPositionTravelled < distanceToNextPosition)
        {
            distanceToNextPositionTravelled += Time.deltaTime * moveSpeed;
            if (distanceToNextPositionTravelled >= distanceToNextPosition)
                distanceToNextPositionTravelled = distanceToNextPosition;

            Vector2 proposedPos = Vector2.Lerp(currentStartPos, currentTarget.Value, distanceToNextPositionTravelled / distanceToNextPosition);
            npcInstance.npcTransform.position = new Vector3(proposedPos.x, proposedPos.y, DynamicZDepth.GetDynamicZDepth(proposedPos, DynamicZDepth.NPC_OFFSET));
        }

        if (distanceToNextPositionTravelled == distanceToNextPosition)
        {
            GetNextGoal();
        }
    }

    private void GetNextGoal()
    {
        if (currentTarget != null && currentTarget.Next == null)
        {
            InvokeChangeState(onFinishStateChange, onFinishStateChangeArgs);
            goalReached = true;
            return;
        }

        currentStartPos = npcInstance.npcTransform.position;
        currentTarget = currentTarget == null ? currentPath.First : currentTarget.Next;

        distanceToNextPositionTravelled = 0;
        distanceToNextPosition = Vector2.Distance(currentStartPos, currentTarget.Value);

        npcInstance.npcDirection.SetDirectionOnMove(currentTarget.Value - currentStartPos);
    }

    public override void EndState()
    {
        animator.SetBool("Walking", false);
    }

    private void RecalculatePath()
    {
        Vector3 currentPos = npcInstance.npcTransform.position;
        Vector2Int currentPosRounded = new Vector2Int(Mathf.RoundToInt(currentPos.x), Mathf.RoundToInt(currentPos.y));
        LinkedList<Vector2Int> proposedPath = AStar.GetShortestPath(currentPosRounded, goal);

        if (proposedPath != null)
        {
            currentPath = proposedPath;

            currentTarget = null;
            OnPathChanged?.Invoke(currentPath);
        }

        goalReached = false;
        GetNextGoal();
    }
}
