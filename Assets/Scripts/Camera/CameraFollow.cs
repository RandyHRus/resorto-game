using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraFollow
{
    public static Transform Following { get; private set; }

    private static readonly float targetShiftTime = 0.3f;

    private static bool changingTarget;
    private static Coroutine targetShiftingCoroutine = null;

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

        void RestartCoroutine() {
            previousTargetPosition = t.position;

            if (targetShiftingCoroutine != null)
                Coroutines.Instance.StopCoroutine(targetShiftingCoroutine);

            targetShiftingCoroutine = Coroutines.Instance.StartCoroutine(LerpEffect.LerpVectorTime(cameraTransform.position, t.position, targetShiftTime, OnProgress, OnEnd, false));
        }

        if (targetShiftingCoroutine != null)
            Coroutines.Instance.StopCoroutine(targetShiftingCoroutine);

        changingTarget = true;

        Following = t;
        RestartCoroutine();
    }
}
