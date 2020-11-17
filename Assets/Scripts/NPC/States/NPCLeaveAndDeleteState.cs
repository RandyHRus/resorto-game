using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/NPC/LeaveAndDelete")]
public class NPCLeaveAndDeleteState : NPCState
{
    public delegate void NPCDeleting();
    public event NPCDeleting OnNPCDeleting;

    public override string DisplayMessage => "";

    public override void StartState(object[] args)
    {
        OnNPCDeleting?.Invoke();
    }

    public override void Execute()
    {
        
    }

    public override void EndState()
    {

    }

}
