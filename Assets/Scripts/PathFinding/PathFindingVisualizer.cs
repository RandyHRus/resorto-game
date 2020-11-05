using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class PathFindingVisualizer
{
    private static LineRenderer lineRenderer => ResourceManager.Instance.PathFindingVisualizerLineRenderer;

    public static void VisualizePath(LinkedList<Tuple<Vector2Int, Vector2Int?>> path)
    {
        lineRenderer.gameObject.SetActive(true);

        lineRenderer.positionCount = path.Count;

        Vector3[] pathArray = new Vector3[path.Count];

        int index = 0;
        foreach (Tuple<Vector2Int, Vector2Int?> pos in path)
        {
            pathArray[index] = new Vector3(pos.Item1.x, pos.Item1.y, 0);
            index++;
        }
        lineRenderer.SetPositions(pathArray);
    }

    public static void Hide()
    {
        lineRenderer.gameObject.SetActive(false);
    }
}
