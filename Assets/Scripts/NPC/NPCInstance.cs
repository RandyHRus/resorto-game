using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPCInstance
{
    public readonly NPCInformation npcInformation;
    public readonly Transform npcTransform;
    public readonly CharacterVisualDirection npcDirection;

    public NPCInstance(NPCInformation info, Transform npcTransform)
    {
        this.npcInformation = info;
        this.npcTransform = npcTransform;
        npcDirection = new CharacterVisualDirection(npcTransform);
    }
}
