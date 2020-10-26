using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParrotBehaviour : WildlifeBehaviour, INonTileClickable
{
    [SerializeField] private Transform innerTransform = null;
    private float flySpeed = 1f;
    private float fleeSpeed = 2f;
    private float floatTimeSeconds = 1f;

    public override float TargetAlpha { get { return 1f; } }

    Transform playerTransform;

    public delegate void CallBack();

    private ParrotState currentState;

    private Coroutine floatCoroutine;
    private Coroutine targetingCoroutine;

    private Dictionary<Type, ParrotState> typeToStateInstance;

    public override void Initialize()
    {
        //Set random direction
        {
            innerTransform.localScale = new Vector2(UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1, 1);
            Animator.SetFloat("yDirection", UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1);
        }

        typeToStateInstance = new Dictionary<Type, ParrotState>()
        {
            { typeof(IdleOnGroundState), new IdleOnGroundState(this) },
            { typeof(IdleFlyingState),   new IdleFlyingState(this) },
            { typeof(FollowingState),    new FollowingState(this) },
            { typeof(FleeingState),      new FleeingState(this) }
        };

        playerTransform = ResourceManager.Instance.Player;

        TrySwitchState<IdleOnGroundState>(null);
    }

    private void Update()
    {
        currentState.Execute();
    }

    private bool TrySwitchState<T>(object[] args) where T: ParrotState
    {
        ParrotState proposedState = typeToStateInstance[typeof(T)];

        if (currentState != null && !currentState.TryEnd())
            return false;

        currentState = proposedState;
        proposedState.OnStart(args);
        return true;
    }

    public Coroutine FloatToHeight(float height, CallBack callback)
    {
        if (floatCoroutine != null)
            StopCoroutine(floatCoroutine);

        floatCoroutine = StartCoroutine(FloatToHeightEnumerator(height, callback));
        return floatCoroutine;
    }

    public Coroutine FlyToTarget(Vector2 targetPos, CallBack callback)
    {
        void Progress(Vector2 position)
        {
            Transform.position = new Vector3(position.x, position.y, DynamicZDepth.GetDynamicZDepth(position.y, DynamicZDepth.ParrotFlying));
        }

        void End()
        {
            callback();
        }

        if (targetingCoroutine != null)
            StopCoroutine(targetingCoroutine);

        Animator.SetBool("Flying", true);
        innerTransform.localScale = new Vector3((Transform.position.x >= targetPos.x) ? -1 : 1, 1, 1);
        Animator.SetFloat("yDirection", (Transform.position.y >= targetPos.y) ? -1 : 1);
        targetingCoroutine = StartCoroutine(LerpEffect.LerpVectorSpeed(Transform.position, targetPos, flySpeed, Progress, End));

        return targetingCoroutine;
    }

    IEnumerator FloatToHeightEnumerator(float height, CallBack endFloat)
    {
        Animator.SetBool("Flying", true);

        float startPos = innerTransform.localPosition.y;
        if (startPos == height)
        {
            endFloat();
            yield break;
        }
            
        float endPos = height;

        float timer = 0;

        while (timer < floatTimeSeconds)
        {
            timer += Time.deltaTime;
            if (timer > floatTimeSeconds)
                timer = floatTimeSeconds;

            innerTransform.localPosition = new Vector2(0, Mathf.Lerp(startPos, endPos, timer / floatTimeSeconds));

            yield return 0;
        }

        endFloat();
    }

    private abstract class ParrotState
    {
        public ParrotBehaviour BehaviourScript { get; private set; }

        public ParrotState(ParrotBehaviour behaviourScript)
        {
            this.BehaviourScript = behaviourScript;
        }

        public virtual void OnStart(object[] args)
        {

        }

        public virtual void Execute()
        {

        }

        public virtual bool TryEnd()
        {
            return true;
        }
    }

    private class IdleOnGroundState: ParrotState
    {
        private Coroutine waitCoroutine;

        public IdleOnGroundState(ParrotBehaviour behaviourScript) : base(behaviourScript)
        {

        }

        public override void OnStart(object[] args)
        {
            BehaviourScript.FloatToHeight(0f, OnFloatEnd);
            waitCoroutine = BehaviourScript.StartCoroutine(Wait());
        }

        public override bool TryEnd()
        {
            if (waitCoroutine != null)
                BehaviourScript.StopCoroutine(waitCoroutine);

            return true;
        }

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(5, 20));
            BehaviourScript.TrySwitchState<IdleFlyingState>(null);
        }

        void OnFloatEnd()
        {
            BehaviourScript.Animator.SetBool("Flying", false);
            BehaviourScript.Transform.position = new Vector3(BehaviourScript.Transform.position.x,
                                                             BehaviourScript.Transform.position.y,
                                                             DynamicZDepth.GetDynamicZDepth(BehaviourScript.Transform.position.y, DynamicZDepth.ParrotOnGround));
        }
    }

    private class IdleFlyingState: ParrotState
    {
        private Vector2 target;

        public IdleFlyingState(ParrotBehaviour behaviourScript) : base(behaviourScript)
        {

        }

        public override void OnStart(object[] args)
        {
            BehaviourScript.FloatToHeight(1f, OnFloatEnd);
            RestartTarget();
        }

        public override bool TryEnd()
        {
            return true;
        }

        private void RestartTarget()
        {
            float targetX = BehaviourScript.Transform.position.x + UnityEngine.Random.Range(-4, 4);
            float targetY = BehaviourScript.Transform.position.y + UnityEngine.Random.Range(-4, 4);
            target = new Vector2(targetX, targetY);
            BehaviourScript.FlyToTarget(target, OnTargetEnd);
        }

        void OnFloatEnd()
        {

        }

        void OnTargetEnd()
        {
            if ((Vector2)BehaviourScript.Transform.position == target)
            {
                if (UnityEngine.Random.Range(0, 10) > 6)
                    BehaviourScript.TrySwitchState<IdleOnGroundState>(null);
                else
                    RestartTarget();
            }
        }
    }

    private class FollowingState: ParrotState
    {
        private Coroutine targetingCoroutine;
        private Vector2 goalPos;
        private Transform target;
        private Vector2 offsetFromTarget;

        public FollowingState(ParrotBehaviour behaviourScript) : base(behaviourScript)
        {

        }

        public override void OnStart(object[] args)
        {
            target = (Transform)args[0];
            offsetFromTarget = (Vector2)args[1];

            BehaviourScript.FloatToHeight(1f, OnFloatEnd);
            BehaviourScript.Animator.SetBool("Flying", true);

            RestartTarget();
        }

        public override void Execute()
        {
            //Target has moved, recalculate goal
            if (((Vector2)target.position + offsetFromTarget) != goalPos)
            {
                if (targetingCoroutine != null)
                    BehaviourScript.StopCoroutine(targetingCoroutine);
                RestartTarget();
            }
        }

        public override bool TryEnd()
        {
            BehaviourScript.StopCoroutine(targetingCoroutine);
            return true;
        }

        void RestartTarget()
        {
            goalPos = (Vector2)target.position + offsetFromTarget;
            targetingCoroutine = BehaviourScript.FlyToTarget(goalPos, OnTargetEnd);
        }

        void OnFloatEnd()
        {

        }

        void OnTargetEnd()
        {

        }
    }

    private class FleeingState: ParrotState
    {
        private int xDirection;

        public FleeingState(ParrotBehaviour behaviourScript) : base(behaviourScript)
        {

        }

        public override void OnStart(object[] args)
        {
            if (BehaviourScript.floatCoroutine != null)
                BehaviourScript.StopCoroutine(BehaviourScript.floatCoroutine);

            if (BehaviourScript.targetingCoroutine != null)
                BehaviourScript.StopCoroutine(BehaviourScript.targetingCoroutine);

            xDirection = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
            BehaviourScript.innerTransform.localScale = new Vector2(xDirection, 1);
            BehaviourScript.Animator.SetBool("Flying", true);
            BehaviourScript.StartCoroutine(Flee());
            BehaviourScript.StartFadeOutAndDestroy();
        }

        public override bool TryEnd()
        {
            //Cannot end fleeing
            return false;
        }

        IEnumerator Flee()
        {
            while (true)
            {
                Vector2 direction = (new Vector3(xDirection, 1, 0)).normalized * Time.deltaTime * BehaviourScript.fleeSpeed;
                BehaviourScript.Transform.position += new Vector3(direction.x, 0);
                BehaviourScript.innerTransform.position += new Vector3(0, direction.y);
                yield return 0;
            }
        }
    }

    public override void Despawn()
    {
        if (currentState == typeToStateInstance[typeof(FollowingState)])
            return;

        TrySwitchState<FleeingState>(null);
    }

    public override void Startle()
    {
        if (currentState == typeToStateInstance[typeof(FollowingState)])
            return;

        Despawn();
    }

    public void NearbyAndOnClick()
    {
        TrySwitchState<FollowingState>(new object[] { playerTransform, new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)) });
    }
}