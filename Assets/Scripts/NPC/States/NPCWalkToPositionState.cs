﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NPCWalkToPositionState : NPCState
{
    public delegate void pathChanged(LinkedList<Tuple<Vector2Int, Vector2Int?>> newPath);
    public event pathChanged OnPathChanged;

    private Vector2Int goal;
    private LinkedList<Tuple<Vector2Int, Vector2Int?>> currentPath;
    private LinkedListNode<Tuple<Vector2Int, Vector2Int?>> currentTarget;
    private bool isOnStairs;
    private Vector2Int? stairsPosition;

    private Animator animator;

    private float distanceToNextPositionTravelled, distanceToNextPosition;
    private Vector2 currentStartPos;
    private bool goalReached;

    private Action onFinishAction;

    private string displayMessage;
    public override string DisplayMessage => displayMessage;

    public NPCWalkToPositionState(NPCComponents npcComponents): base(npcComponents)
    {
        animator = npcGameObject.GetComponent<Animator>();
    }

    public override void StartState(object[] args)
    {
        goal = (Vector2Int)args[0];
        onFinishAction = (Action)args[1];
        displayMessage = (string)args[2];

        animator.SetBool("Walking", true);

        RecalculatePath();
    }

    public override void Execute()
    {
        if (goalReached)
            return;

        if (distanceToNextPositionTravelled < distanceToNextPosition)
        {
            distanceToNextPositionTravelled += Time.deltaTime * npcComponents.moveSpeed;
            if (distanceToNextPositionTravelled >= distanceToNextPosition)
                distanceToNextPositionTravelled = distanceToNextPosition;

            Vector2 proposedPos = Vector2.Lerp(currentStartPos, currentTarget.Value.Item1, distanceToNextPositionTravelled / distanceToNextPosition);
            npcComponents.npcTransform.position = new Vector3(proposedPos.x, proposedPos.y, 
                isOnStairs ? DynamicZDepth.GetDynamicZDepth((Vector2Int)stairsPosition, DynamicZDepth.CHARACTER_ON_STAIRS) :
                             DynamicZDepth.GetDynamicZDepth(proposedPos, DynamicZDepth.NPC_OFFSET));
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
            EndWalk();
            return;
        }

        currentStartPos = npcComponents.npcTransform.position;
        currentTarget = (currentTarget == null) ? currentPath.First : currentTarget.Next;

        distanceToNextPositionTravelled = 0;
        distanceToNextPosition = Vector2.Distance(currentStartPos, currentTarget.Value.Item1);

        isOnStairs = currentTarget.Value.Item2 != null;
        stairsPosition = currentTarget.Value.Item2;


        npcComponents.npcDirection.SetDirectionOnMove(currentTarget.Value.Item1 - currentStartPos);
    }

    private void EndWalk()
    {
        onFinishAction?.Invoke();
        goalReached = true;
        InvokeEndState();
    }

    public override void EndState()
    {
        animator.SetBool("Walking", false);
    }

    private void RecalculatePath()
    {
        Vector3 currentPos = npcComponents.npcTransform.position;
        Vector2Int currentPosRounded = new Vector2Int(Mathf.RoundToInt(currentPos.x), Mathf.RoundToInt(currentPos.y));
        LinkedList<Tuple<Vector2Int, Vector2Int?>> proposedPath = AStar.GetShortestPath(currentPosRounded, goal);

        if (proposedPath == null)
        {
            EndWalk();
            return;
        }

        currentPath = proposedPath;
        currentTarget = null;
        OnPathChanged?.Invoke(currentPath);

        goalReached = false;
        GetNextGoal();
    }
}
