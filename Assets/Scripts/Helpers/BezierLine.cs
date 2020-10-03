using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BezierLine
{
    public static void CreateBezierLine(Vector3[] points, Vector2 lineStartPosition, Vector2 lineMiddlePosition, Vector2 lineEndPosition)
    {
        int numPoints = points.Length;

        //Draw incremental small lines for each point
        for (int i = 0; i < numPoints; i++)
        {
            float t = i / (float)(numPoints - 1);
            points[i] = CalculateQuadraticBezierPoint(t, lineStartPosition, lineMiddlePosition, lineEndPosition);
        }
    }

    public static Vector2 CalculateQuadraticBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector2 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;
        return p;
    }
}
