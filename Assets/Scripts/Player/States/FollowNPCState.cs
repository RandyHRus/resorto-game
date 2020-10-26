using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/FollowNPC")]
public class FollowNPCState : PlayerState
{
    public override bool AllowMovement => false;
    public override bool AllowMouseDirectionChange => false;

    private NPCMonoBehaviour npcMono;
    private NPCWalkToPositionState walkToPositionState;
    private Transform previousFollowing;

    public override void StartState(object[] args)
    {
        npcMono = ((NPCInstance)args[0]).npcTransform.GetComponent<NPCMonoBehaviour>() ;
        walkToPositionState = npcMono.GetStateInstance<NPCWalkToPositionState>();

        previousFollowing = MoveCamera.Instance.Following;
        MoveCamera.Instance.ChangeFollowTarget(npcMono.ObjectTransform);

        walkToPositionState.OnPathChanged += PathFindingVisualizer.VisualizePath;
    }

    public override void Execute()
    {
        if (CheckMouseOverUI.GetButtonDownAndNotOnUI("Primary"))
        {
            Vector2Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();
            TileInformation mouseTileInfo = TileInformationManager.Instance.GetTileInformation(mouseTilePosition);
            if (mouseTileInfo != null) {
                npcMono.SwitchState<NPCWalkToPositionState>(new object[] { (Vector2Int)mouseTilePosition, null, null, "Going to target location" });
            }
        }
    }

    public override void EndState()
    {
        walkToPositionState.OnPathChanged -= PathFindingVisualizer.VisualizePath;
        PathFindingVisualizer.Hide();
        MoveCamera.Instance.ChangeFollowTarget(previousFollowing);
    }
}
