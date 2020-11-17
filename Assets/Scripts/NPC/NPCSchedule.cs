using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPCSchedule
{
    protected NPCInstance npcInstance;

    public NPCSchedule(NPCInstance npcInstance)
    {
        this.npcInstance = npcInstance;
    }
}
