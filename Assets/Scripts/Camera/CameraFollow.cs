using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow: MonoBehaviour
{
    public Transform Following { get; private set; }

    private bool changingTarget;

    private Transform cameraTransform;

    private static CameraFollow _instance;
    public static CameraFollow Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        cameraTransform = Camera.main.transform;
    }

    public void ExecuteLateUpdate()
    {
        if (!changingTarget)
            cameraTransform.position = new Vector3(Following.position.x, Following.position.y, -100);
    }

    public void ChangeFollowTarget(Transform t)
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

            if (CameraFunctions.Instance.lerpingCoroutine != null)
                Coroutines.Instance.StopCoroutine(CameraFunctions.Instance.lerpingCoroutine);

            CameraFunctions.Instance.lerpingCoroutine = Coroutines.Instance.StartCoroutine(LerpEffect.LerpVectorTime(cameraTransform.position, t.position, CameraFunctions.targetShiftTime, OnProgress, OnEnd, false));
        }

        if (CameraFunctions.Instance.lerpingCoroutine != null)
            Coroutines.Instance.StopCoroutine(CameraFunctions.Instance.lerpingCoroutine);

        changingTarget = true;

        Following = t;
        RestartCoroutine();
    }
}
