using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/FollowNPC")]
public class FollowNPCState : PlayerState
{
    public override bool AllowMovement => false;
    public override bool AllowMouseDirectionChange => false;
    public override CameraMode CameraMode => CameraMode.Follow;

    public NPCMonoBehaviour NpcMono { get; private set; }
    private NPCWalkToPositionState walkToPositionState;
    private Transform previousFollowing;

    public override void StartState(object[] args)
    {
        NpcMono = ((NPCComponents)args[0]).npcTransform.GetComponent<NPCMonoBehaviour>() ;
        walkToPositionState = NpcMono.GetStateInstance<NPCWalkToPositionState>();

        previousFollowing = CameraFollow.Instance.Following;
        CameraFollow.Instance.ChangeFollowTarget(NpcMono.ObjectTransform);

        walkToPositionState.OnPathChanged += PathFindingVisualizer.VisualizePath;

        NpcMono.NPCComponents.SubscribeToEvent(NPCInstanceEvent.Delete, OnNPCDeletedHandler);
    }

    public override void Execute()
    {
        if (CheckMouseOverUI.GetButtonDownAndNotOnUI("Primary"))
        {
            Vector2Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();
            if (TileInformationManager.Instance.TryGetTileInformation(mouseTilePosition, out TileInformation mouseTileInfo)) {
                NpcMono.SwitchState<NPCWalkToPositionState>(new object[] { (Vector2Int)mouseTilePosition, null, "Going to target location" });
            }
        }
    }

    public override void EndState()
    {
        walkToPositionState.OnPathChanged -= PathFindingVisualizer.VisualizePath;
        PathFindingVisualizer.Hide();
        CameraFollow.Instance.ChangeFollowTarget(previousFollowing);
    }

    private void OnNPCDeletedHandler(object[] args)
    {
        walkToPositionState.OnPathChanged -= PathFindingVisualizer.VisualizePath;
        NpcMono.NPCComponents.UnsubscribeToEvent(NPCInstanceEvent.Delete, OnNPCDeletedHandler);
        InvokeEndState();
    }
}
