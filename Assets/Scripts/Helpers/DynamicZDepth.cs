﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicZDepth : MonoBehaviour
{
    public static float OBJECTS_STANDARD_OFFSET = 0f;
    public static float PLAYER_OFFSET = 0f;
    public static float NPC_OFFSET = 0.1f;
    public static float OBJECTS_ONTOP_OFFSET = 0.2f;
    public static float PLAYER_ON_STAIRS = 0.8f;

    public static float ParrotFlying = 0.8f;
    public static float ParrotOnGround = 0f;

    public static float GetDynamicZDepth(Vector3 pos, float offset)
    {
        return GetDynamicZDepth(pos.y, offset);
    }

    public static float GetDynamicZDepth(float yPos, float offset)
    {
        return (yPos - offset);
    }
}
