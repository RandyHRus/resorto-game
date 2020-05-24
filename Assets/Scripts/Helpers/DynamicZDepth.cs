using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicZDepth : MonoBehaviour
{
    public static int OBJECTS_STANDARD_OFFSET = 0;
    public static int PLAYER_OFFSET = 1;
    public static int NPC_OFFSET = 1;
    public static int OBJECTS_ONTOP_OFFSET = 2;
    public static int PLAYER_ON_STAIRS = 8;

    public static float GetDynamicZDepth(Vector3 pos, int offset)
    {
        return ((pos.y)*10 - offset) / 100f;
    }

    public static float GetDynamicZDepth(float yPos, int offset)
    {
        return ((yPos)*10 - offset) / 100f;
    }
}
