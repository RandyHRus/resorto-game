using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathFunctions
{
    //Returns angle between 0 & 360 in degrees
    public static float GetAngleBetweenPoints(Vector2 point1, Vector2 point2)
    {
        return Mod(Mathf.Atan2(point2.y - point1.y, point2.x - point1.x) * Mathf.Rad2Deg, 360);
    }

    //This mod returns positve, Mod function in Mathf might return negative
    public static float Mod(float x, int m)
    {
        float r = x % m;
        return r < 0 ? r + m : r;
    }
}
