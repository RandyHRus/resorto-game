using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFunctions: MonoBehaviour
{
    public static readonly float targetShiftTime = 0.3f;
    public Coroutine lerpingCoroutine;

    private Transform cameraTransform;

    private static CameraFunctions _instance;
    public static CameraFunctions Instance { get { return _instance; } }
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

    public void LerpToPosition(Vector2 targetPos)
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
