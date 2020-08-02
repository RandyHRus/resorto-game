using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairsManager
{  
    public static bool StairsPlaceable(out StairsDirection dir)
    {
        dir = StairsDirection.Front;
        return true;
    }

    public static bool TryCreateStairs()
    {
        if (!StairsPlaceable(out StairsDirection dir))
            return false;

        return true;
    }
}
