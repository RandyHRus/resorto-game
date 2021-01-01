using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSleepingState : NPCState
{
    public override string DisplayMessage => "Sleeping";

    public NPCSleepingState(NPCComponents npcComponents) : base(npcComponents) { }

    public override void StartState(object[] args)
    {
    }

    public override void Execute()
    {
        
    }

    public override void EndState()
    {
        
    }
}
