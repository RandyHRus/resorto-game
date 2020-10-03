using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/FollowNPC")]
public class FollowNPCState : PlayerState
{
    public override bool AllowMovement => false;
    public override bool AllowMouseDirectionChange => false;

    private Transform previousFollowing;

    public override void StartState(object[] args)
    {
        NPCInstance instance = (NPCInstance)args[0];

        previousFollowing = MoveCamera.Instance.Following;
        MoveCamera.Instance.ChangeFollowTarget(instance.ObjectTransform);
    }

    public override void Execute()
    {
        
    }

    public override void EndState()
    {
        MoveCamera.Instance.ChangeFollowTarget(previousFollowing);
    }
}
