using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraFollow
{
    public static Transform Following { get; private set; }

    private static bool changingTarget;

    private static readonly Transform cameraTransform;

    static CameraFollow()
    {
        cameraTransform = Camera.main.transform;
    }

    public static void ExecuteLateUpdate()
    {
        if (!changingTarget)
            cameraTransform.position = new Vector3(Following.position.x, Following.position.y, -100);
    }

    public static void ChangeFollowTarget(Transform t)
    {
        Vector2 previousTargetPosition;

        void OnProgress(Vector2 pos)
        {
            cameraTransform.position = new Vector3(pos.x, pos.y, -100);

            if (previousTargetPosition != (Vector2)t.position)
            {
                RestartCoroutine();
            }
        }

        void OnEnd()
        {
            changingTarget = false;
        }

        void RestartCoroutine()
        {
            previousTargetPosition = t.position;

            if (CameraFunctions.lerpingCoroutine != null)
                Coroutines.Instance.StopCoroutine(CameraFunctions.lerpingCoroutine);

            CameraFunctions.lerpingCoroutine = Coroutines.Instance.StartCoroutine(LerpEffect.LerpVectorTime(cameraTransform.position, t.position, CameraFunctions.targetShiftTime, OnProgress, OnEnd, false));
        }

        if (CameraFunctions.lerpingCoroutine != null)
            Coroutines.Instance.StopCoroutine(CameraFunctions.lerpingCoroutine);

        changingTarget = true;

        Following = t;
        RestartCoroutine();
    }
}
