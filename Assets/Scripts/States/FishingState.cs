using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "States/Fishing")]
public class FishingState : PlayerState
{
    [SerializeField] private GameObject fishingLinePrefab = null;
    [SerializeField] private GameObject caughtFish = null;

    private Canvas progressBarCanvas;
    private ProgressBar progressBar;
    private GameObject fishingLineInstance;
    private LineRenderer lineRenderer;
    private Transform fishingLineTransform;
    private Animator animator;
    private Transform playerTransform;
    private Transform fishingRod;
    private Vector3[] points;
    private ContactFilter2D wildlifeFilter;
    private GameObject player;

    private HashSet<GameObject> fishInRange  = new HashSet<GameObject>();

    private static readonly int numPoints = 10;
    private static readonly float bobHeight = 0.04f;
    private static readonly float bobSpeed = 3f;
    private float fishScanRadius = FishInformation.FISH_SEEING_DISTANCE;

    private Dictionary<Type, State> typeToStateInstance;
    private State currentState;

    public abstract class State
    {
        public FishingState StateInstance { get; private set; }

        public State(FishingState stateInstance)
        {
            this.StateInstance = stateInstance;
        }

        public abstract void StartState(object[] args);

        public abstract void Execute();

        public virtual void LateExecute() { }

        public abstract void EndState();
    }

    public class DefaultFishingState : State
    {
        public DefaultFishingState(FishingState stateInstance): base(stateInstance) { }

        public override void EndState()
        {
            //Nothing needed
        }

        public override void Execute()
        {
            if (CheckMouseOverUI.GetButtonDownAndNotOnUI("Primary"))
            {
                StateInstance.SwitchFishingState<ChargingState>();
            }
        }

        public override void StartState(object[] args)
        {
            //Nothing needed
        }
    }

    public class ChargingState : State
    {
        static readonly float maxCastHoldTime = 1f;
        float timer;
        bool firstExecute;

        public ChargingState(FishingState stateInstance): base(stateInstance) { }

        public override void StartState(object[] args)
        {
            //Animation
            {
                PlayerMovement.Instance.StopMovement(); //TODO: Maybe there could be a better way to call this?
                StateInstance.animator.SetBool("LineCastFinished", false);
                StateInstance.animator.SetBool("LineOut", true);
                StateInstance.animator.speed = 0;
            }
            StateInstance.lineRenderer.positionCount = FishingState.numPoints;
            firstExecute = true;

            timer = 0;
        }

        public override void Execute()
        {
            //StartState is called 1 frame before, so we dont want the progress bar showing 1 frame before
            if (firstExecute)
            {
                StateInstance.progressBar.ObjectTransform.position
                    = (new Vector2(StateInstance.playerTransform.position.x, StateInstance.playerTransform.position.y + 1.5f));
                StateInstance.progressBar.Show(true);
            }

            timer += Time.deltaTime;

            if (timer >= maxCastHoldTime)
                timer = maxCastHoldTime;

            StateInstance.progressBar.SetFill(timer / maxCastHoldTime);

            if (Input.GetButtonUp("Primary") || timer >= maxCastHoldTime)
            {
                StateInstance.SwitchFishingState<CastingState>(new object[] { timer / maxCastHoldTime });
            }
        }

        public override void EndState()
        {
            StateInstance.progressBar.Show(false);
            StateInstance.ResetAnimation();
        }
    }

    public class CastingState : State
    {
        static readonly float maxCastLength = 5f;
        static readonly float castSpeed = 0.6f;
        static readonly float castDragSpeed = 0.8f;
        static readonly float castAngle = 150f;
        static readonly float dragOffsetAngle = 15f;

        float chargeFrac;
        float timer;
        float dragTimer;
        bool initialized;
        float xDir;
        float yDir;
        Vector2 lineStartPosition, lineEndPosition, lineMiddlePosition;
        float beginLineAngle;
        float maxLineLength;
        bool firstExecute;

        public CastingState(FishingState stateInstance): base(stateInstance) { }

        public override void StartState(object[] args)
        {
            //chargeTimer / maxCastHoldTime
            chargeFrac = (float)args[0];
            firstExecute = true;
            Coroutines.Instance.StartCoroutine(DelayedInitialize());
        }

        IEnumerator DelayedInitialize()
        {
            initialized = false;

            yield return 0; //Wait a frame to get proper fishing rod position (HeightToEndOfRod could return 0 if this isn't here) TODO: maybe there is better way?

            StateInstance.animator.SetBool("LineOut", true);
            StateInstance.animator.speed = 1;

            lineEndPosition = new Vector2(0, 0);
            lineMiddlePosition = new Vector2(0, 0);

            float heightToEndOfRod = StateInstance.fishingRod.position.y - StateInstance.playerTransform.position.y;

            var minimumOffset = 0.02f; //this is there so that line can't be completely 90 degrees down
            float minCastLength = heightToEndOfRod + minimumOffset;
            maxLineLength = ((maxCastLength - minCastLength) * chargeFrac) + minCastLength;

            xDir = PlayerDirection.Instance.VisualDirection.DirectionVector.x;
            yDir = PlayerDirection.Instance.VisualDirection.DirectionVector.y;

            if (yDir != 0)
            {
                beginLineAngle = 90;
            }
            else
            {
                float endLineAngle = 270 + (Mathf.Rad2Deg * Mathf.Acos(heightToEndOfRod / maxLineLength)); //Degrees
                beginLineAngle = endLineAngle + castAngle; //Degrees
            }

            timer = 0;
            dragTimer = 0;
            initialized = true;
        }

        public override void Execute()
        {
            if (!initialized) {
                return; 
            }

            if (firstExecute)
            {
                firstExecute = false;

                /*
                 *  Cant be in initialization (StartState function) or DelayedInitialization because if initialization is 
                 *  run after execute in same frame, line will be active but not in correct position 
                 */
                StateInstance.fishingLineInstance.SetActive(true);
            }
        }

        public override void LateExecute()
        {
            if (!initialized)
            {
                return;
            }

            base.LateExecute();

            //These are in late execute because animation needs to end for positions to be correct
            if (timer < castSpeed || dragTimer < castDragSpeed)
            {
                #region increment timer
                if (timer < castSpeed)
                    timer += Time.deltaTime;
                
                if (timer > castSpeed)
                    timer = castSpeed;

                if (dragTimer < castDragSpeed)
                    dragTimer += Time.deltaTime;
                
                if (dragTimer > castDragSpeed)
                    dragTimer = castDragSpeed;
                #endregion

                lineStartPosition = StateInstance.fishingRod.position;
                float currentLineLength = maxLineLength * (timer / castSpeed);
                float lineAngle = beginLineAngle - ((timer / castSpeed) * castAngle);
                float lineDragAngle = beginLineAngle - ((dragTimer / castDragSpeed) * castAngle) - dragOffsetAngle;

                //Horizontal direction
                if (yDir == 0)
                {
                    //Get line end position
                    {
                        float lineEndX = xDir * currentLineLength * Mathf.Cos(Mathf.Deg2Rad * lineAngle);
                        float lineEndY = currentLineLength * Mathf.Sin(Mathf.Deg2Rad * lineAngle);
                        lineEndPosition = new Vector2(lineStartPosition.x + lineEndX, lineStartPosition.y + lineEndY);
                    }
                    //Get line drag (middle) position
                    {
                        float dragLineEndX = xDir * (currentLineLength / 2) * Mathf.Cos(Mathf.Deg2Rad * lineDragAngle);
                        float dragLineEndY = (currentLineLength / 2) * Mathf.Sin(Mathf.Deg2Rad * lineDragAngle);
                        lineMiddlePosition = new Vector2(lineStartPosition.x + dragLineEndX, lineStartPosition.y + dragLineEndY);
                    }
                }
                //Vertical direction 
                //TODO: vertical animation should be same regardless of horizontal direction (meaning vertical animation has only up or down)
                else
                {
                    //Get line end position
                    {
                        float lineEndX = (1 / 4f) * currentLineLength * Mathf.Cos(Mathf.Deg2Rad * lineAngle);
                        float lineEndY = -yDir * currentLineLength * Mathf.Sin(Mathf.Deg2Rad * lineAngle);
                        lineEndPosition = new Vector2(lineStartPosition.x + lineEndX, lineStartPosition.y + lineEndY);
                    }
                    //Get line drag (middle) position
                    {
                        float dragLineEndX = (1 / 4f) * (currentLineLength / 2) * Mathf.Cos(Mathf.Deg2Rad * lineDragAngle);
                        float dragLineEndY = -yDir * (currentLineLength / 2) * Mathf.Sin(Mathf.Deg2Rad * lineDragAngle);
                        lineMiddlePosition = new Vector2(lineStartPosition.x + dragLineEndX, lineStartPosition.y + dragLineEndY);
                    }
                }
                {
                    StateInstance.CreateBezierLine(lineStartPosition, lineMiddlePosition, lineEndPosition);
                    StateInstance.fishingLineTransform.position = lineEndPosition; //For moving bobber to end of line
                }
            }
            else
            {
                StateInstance.SwitchFishingState<LineBobbingState>(new object[] { lineMiddlePosition, lineEndPosition });
            }
        }

        public override void EndState()
        {
            StateInstance.animator.SetBool("LineCastFinished", true);
            StateInstance.ResetAnimation();
        }
    }

    public class LineBobbingState : State
    {
        float timer;
        Vector2 lineEndDefaultPosition, lineMiddleDefaultPosition;
        Vector2 lineEndPosition, lineMiddlePosition;
        Coroutine alertCoroutine;

        public LineBobbingState(FishingState stateInstance): base(stateInstance) { }

        public override void StartState(object[] args)
        {
            StateInstance.animator.SetBool("LineOut", true);
            StateInstance.animator.speed = 1;
            StateInstance.fishingLineInstance.SetActive(true);

            timer = 0;
            lineMiddleDefaultPosition = (Vector2)args[0];
            lineEndDefaultPosition = (Vector2)args[1];

            lineEndPosition = lineEndDefaultPosition;
            lineMiddlePosition = lineMiddleDefaultPosition;

            alertCoroutine = Coroutines.Instance.StartCoroutine(StateInstance.AlertNearbyFishToTarget(lineEndPosition));
        }

        public override void Execute()
        {
            float offset = Mathf.Sin(bobSpeed * timer) * bobHeight;
            lineMiddlePosition = new Vector2(lineMiddleDefaultPosition.x, lineMiddleDefaultPosition.y + (offset / 2));
            lineEndPosition = new Vector2(lineEndDefaultPosition.x, lineEndDefaultPosition.y + offset);

            Vector2 lineStartPosition = StateInstance.fishingRod.position; //This will change by bobbing up and down so needs to be in execute

            StateInstance.CreateBezierLine(lineStartPosition, lineMiddlePosition, lineEndPosition);
            StateInstance.fishingLineTransform.position = lineEndPosition; //For moving bobber to end of line

            timer += Time.deltaTime;

            if (CheckMouseOverUI.GetButtonDownAndNotOnUI("Primary"))
            {
                StateInstance.SwitchFishingState<DefaultFishingState>();
                return;
            }
        }

        public override void EndState()
        {
            Coroutines.Instance.StopCoroutine(alertCoroutine);
            StateInstance.ResetAnimation();

            foreach (GameObject fish in StateInstance.fishInRange)
            {
                if (fish != null)
                    fish.GetComponent<FishBehaviour>().TryStartFlee();
            }
        }
    }

    public class FishHookedState: State
    {
        static readonly float fishCaughtFlySpeed = 10f;

        Transform swimmingFishTransform;
        Transform caughtFishFlyingInstance;
        FishItemInstance randomFish;
        int clickCount;

        public FishHookedState(FishingState stateInstance): base(stateInstance) { }

        public override void StartState(object[] args)
        {
            StateInstance.animator.SetBool("LineOut", true);
            StateInstance.animator.SetBool("Hooked", true);
            StateInstance.fishingLineInstance.SetActive(true);
            StateInstance.lineRenderer.positionCount = 2;

            swimmingFishTransform = (Transform)args[0];
            clickCount = 5; //Todo: Come up with something else other then clicks
        }

        public override void Execute()
        {
            void FishFlyingProgress(Vector2 position)
            {
                caughtFishFlyingInstance.position = new Vector3(position.x, position.y, DynamicZDepth.GetDynamicZDepth(position, DynamicZDepth.CAUGHT_FISH_OFFSET));
            }

            void FishFlyingEnd()
            {
                Destroy(caughtFishFlyingInstance.gameObject);
                InventoryManager.Instance.AddItem(randomFish, 1);
            }

            Vector2 lineEndPosition = swimmingFishTransform.position + ((-swimmingFishTransform.right) * FishInformation.FISH_WORLD_WIDTH / 2);
            Vector2 lineStartPosition = StateInstance.fishingRod.position;

            StateInstance.fishingLineTransform.position = lineEndPosition; //For moving bobber to end of line
            StateInstance.lineRenderer.SetPositions(new Vector3[] { lineStartPosition, lineEndPosition });

            if (CheckMouseOverUI.GetButtonDownAndNotOnUI("Primary"))
                clickCount--;

            if (clickCount <= 0)
            {
                //Finish here
                Destroy(swimmingFishTransform.gameObject);

                caughtFishFlyingInstance = Instantiate(StateInstance.caughtFish, lineEndPosition, Quaternion.identity).transform;

                randomFish = CaughtFishGenerator.Instance.GetRandomFish();
                caughtFishFlyingInstance.GetComponent<SpriteRenderer>().sprite = randomFish.ItemInformation.ItemIcon;

                Coroutines.Instance.StartCoroutine(LerpEffect.LerpVectorSpeed(lineEndPosition, StateInstance.playerTransform.position, fishCaughtFlySpeed, FishFlyingProgress, FishFlyingEnd));

                StateInstance.SwitchFishingState<DefaultFishingState>();
            }
        }

        public override void EndState()
        {
            StateInstance.ResetAnimation();
        }
    }

    public override void Initialize()
    {
        base.Initialize();

        //Configure fishing line object
        {
            fishingLineInstance = Instantiate(fishingLinePrefab);

            lineRenderer = fishingLineInstance.GetComponent<LineRenderer>();
            lineRenderer.positionCount = numPoints;
            lineRenderer.generateLightingData = true;

            fishingLineTransform = fishingLineInstance.transform;

            fishingLineInstance.SetActive(false);

            points = new Vector3[numPoints];
        }
        //Find components
        {
            progressBarCanvas = GameObject.FindGameObjectWithTag("IndicatorsCanvas").GetComponent<Canvas>();
            player = GameObject.FindGameObjectWithTag("Player");

            animator = player.GetComponent<Animator>();
            playerTransform = player.transform;

            //Find fishing rod object in player
            fishingRod = FindChildWithTag(playerTransform, "FishingRod");
        }
        //Configure progress bar
        {
            progressBar = new ProgressBar(progressBarCanvas);
            progressBar.Show(false);
        }
        //Create fish contact filter
        {
            wildlifeFilter = new ContactFilter2D();
            wildlifeFilter.useTriggers = false;
            wildlifeFilter.SetLayerMask(1 << LayerMask.NameToLayer("Wildlife"));
            wildlifeFilter.useLayerMask = true;
        }

        Transform FindChildWithTag(Transform t, string tag)
        {
            if (t.tag == tag)
            {
                return t;
            }

            foreach (Transform ct in t)
            {
                Transform found = FindChildWithTag(ct, tag);
                if (found != null)
                    return found;
            }

            return null;
        }

        //Initialize states
        typeToStateInstance = new Dictionary<Type, State>()
        {
            { typeof(DefaultFishingState),  new DefaultFishingState(this) },
            { typeof(ChargingState),        new ChargingState(this) },
            { typeof(CastingState),         new CastingState(this) },
            { typeof(LineBobbingState),     new LineBobbingState(this) },
            { typeof(FishHookedState),      new FishHookedState(this) }
        };

        SwitchFishingState<DefaultFishingState>();
    }

    public override bool AllowMovement
    {
        get { return (currentState == typeToStateInstance[typeof(DefaultFishingState)]); }
    }

    public override void Execute()
    {
        currentState.Execute();
    }

    public override void LateExecute()
    {
        base.LateExecute();
        currentState.LateExecute();
    }

    public override void StartState(object[] args)
    {
        animator.SetLayerWeight(animator.GetLayerIndex("Fishing"), 1);
    }

    public override void EndState()
    {
        SwitchFishingState<DefaultFishingState>();
        animator.SetLayerWeight(animator.GetLayerIndex("Fishing"), 0);
    }

    public void OnFishBite(GameObject hookedFish)
    {
        fishInRange.Remove(hookedFish); //So it doesn't flee
        SwitchFishingState<FishHookedState>(new object[] { hookedFish.transform });
    }

    private void SwitchFishingState<T>(object[] args = null) where T: State
    {
        currentState?.EndState();
        currentState = typeToStateInstance[typeof(T)];
        currentState.StartState(args);
    }


    IEnumerator AlertNearbyFishToTarget(Vector2 lineEndPosition)
    {
        List<Collider2D> results = new List<Collider2D>();
        Physics2D.OverlapCircle(lineEndPosition, fishScanRadius, wildlifeFilter, results);
        fishInRange.Clear(); //Refresh every time

        foreach (Collider2D c in results)
        {
            GameObject obj = c.gameObject;
            if (obj.TryGetComponent<FishBehaviour>(out FishBehaviour component))
            {
                component.SetTarget(this, lineEndPosition);
                fishInRange.Add(obj);
            }
        }

        yield return new WaitForSeconds(1); //No need to do every frame.
    }

    private void CreateBezierLine(Vector2 lineStartPosition, Vector2 lineMiddlePosition, Vector2 lineEndPosition)
    {
        //Draw incremental small lines for each point
        for (int i = 0; i < numPoints; i++)
        {
            float t = i / (float)(numPoints - 1);
            points[i] = CalculateQuadraticBezierPoint(t, lineStartPosition, lineMiddlePosition, lineEndPosition);
        }
        lineRenderer.SetPositions(points);
    }

    private Vector2 CalculateQuadraticBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector2 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;
        return p;
    }

    private void ResetAnimation()
    {
        animator.SetBool("LineOut", false);
        animator.SetBool("Hooked", false);
        fishingLineInstance.SetActive(false);
    }
}
