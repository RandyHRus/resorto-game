using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PathFindingVisualizer
{
    private static LineRenderer lineRenderer => ResourceManager.Instance.PathFindingVisualizerLineRenderer;

    public static void VisualizePath(LinkedList<Vector2Int> path)
    {
        lineRenderer.gameObject.SetActive(true);

        lineRenderer.positionCount = path.Count;

        Vector3[] pathArray = new Vector3[path.Count];

        int index = 0;
        foreach (Vector2Int pos in path)
        {
            pathArray[index] = new Vector3(pos.x, pos.y, 0);
            index++;
        }
        lineRenderer.SetPositions(pathArray);
    }

    public static void Hide()
    {
        lineRenderer.gameObject.SetActive(false);
    }
}
