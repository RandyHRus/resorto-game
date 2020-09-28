using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] private Transform player = null;

    public Transform Following { get; private set; }

    private bool changingTarget;
    private float targetShiftTime = 0.3f;
    Coroutine targetShiftingCoroutine = null;

    private static MoveCamera _instance;
    public static MoveCamera Instance { get { return _instance; } }
    private void Awake()
    {
        //Singleton
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }

        ChangeFollowTarget(player);
    }

    private void LateUpdate()
    {
        if (!changingTarget)
            transform.position = new Vector3(Following.position.x, Following.position.y, -100);
    }

    public void ChangeFollowTarget(Transform t)
    {
        Vector2 previousTargetPosition;

        void OnProgress(Vector2 pos)
        {
            transform.position = new Vector3(pos.x, pos.y, -100);

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
                StopCoroutine(targetShiftingCoroutine);

            targetShiftingCoroutine = StartCoroutine(LerpEffect.LerpVectorTime(transform.position, t.position, targetShiftTime, OnProgress, OnEnd));
        }

        if (targetShiftingCoroutine != null)
            StopCoroutine(targetShiftingCoroutine);

        changingTarget = true;

        Following = t;
        RestartCoroutine();
    }
}
