﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSleepingState : NPCState
{
    public override string DisplayMessage => "Sleeping";

    public NPCSleepingState(NPCInstance npcInstance) : base(npcInstance) { }

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