using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraFunctions
{
    public static readonly float targetShiftTime = 0.3f;
    public static Coroutine lerpingCoroutine;

    private static readonly Transform cameraTransform;

    static CameraFunctions()
    {
        cameraTransform = Camera.main.transform;
    }

    public static void LerpToPosition(Vector2 targetPos)
    {
        void OnProgress(Vector2 pos)
        {
            cameraTransform.position = new Vector3(pos.x, pos.y, -100);
        }

        Vector2 startPos = cameraTransform.position;

        if (lerpingCoroutine != null)
            Coroutines.Instance.StopCoroutine(lerpingCoroutine);

        lerpingCoroutine = Coroutines.Instance.StartCoroutine(LerpEffect.LerpVectorTime(startPos, targetPos, targetShiftTime, OnProgress, null, false));
    }
}
