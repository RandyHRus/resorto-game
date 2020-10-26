using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class FishingPhase
{
    public delegate void ChangePhase(Type phaseType, object[] args);
    public event ChangePhase OnChangePhase;

    public static readonly float maxCastLength = 3f;

    public void InvokeChangePhase(Type phaseType, object[] args)
    {
        OnChangePhase?.Invoke(phaseType, args);
    }

    public readonly FishingResources resources;

    public FishingPhase(FishingResources resources)
    {
        this.resources = resources;
    }

    public abstract void StartState(object[] args);

    public abstract void Execute();

    public virtual void LateExecute() { }

    public abstract void EndState();
}

public class FishingDefaultPhase : FishingPhase
{
    public FishingDefaultPhase(FishingResources resources) : base(resources) { }

    public override void EndState()
    {
        
    }

    public override void Execute()
    {
    }

    public override void StartState(object[] args)
    {
    }
}

public class FishingChargingPhase : FishingPhase
{
    protected static readonly float maxCastHoldTime = 1f;
    protected float timer;

    protected float xDir;
    protected float yDir;

    public FishingChargingPhase(FishingResources resources) : base(resources) { }

    public override void StartState(object[] args)
    {
        xDir = (float)args[0];
        yDir = (float)args[1];

        //Animation
        {
            resources.animator.SetBool("LineCastFinished", false);
            resources.animator.SetBool("LineOut", true);
            resources.animator.speed = 0;
        }
        resources.fishinglineRenderer.positionCount = FishingResources.numPoints;

        timer = 0;
    }

    public override void Execute()
    {
        timer += Time.deltaTime;

        if (timer >= maxCastHoldTime)
            timer = maxCastHoldTime;

        if (timer >= maxCastHoldTime)
        {
            InvokeChangePhase(typeof(FishingCastingPhase), new object[] { 1f, xDir, yDir });
            return;
        }
    }

    public override void EndState()
    {
    }
}

public class FishingCastingPhase : FishingPhase
{
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
    float targetLineLength;
    bool firstExecute;

    Coroutine initializeCoroutine;

    public FishingCastingPhase(FishingResources resources) : base(resources) { }

    public override void StartState(object[] args)
    {
        chargeFrac = (float)args[0];
        xDir = (float)args[1];
        yDir = (float)args[2];

        firstExecute = true;
        initializeCoroutine = Coroutines.Instance.StartCoroutine(DelayedInitialize());
    }

    IEnumerator DelayedInitialize()
    {
        initialized = false;

        yield return 0; //Wait a frame to get proper fishing rod position (HeightToEndOfRod could return 0 if this isn't here) TODO: maybe there is better way?

        resources.animator.SetBool("LineOut", true);
        resources.animator.speed = 1;

        lineEndPosition = new Vector2(0, 0);
        lineMiddlePosition = new Vector2(0, 0);

        float heightToEndOfRod = resources.fishingRod.position.y - resources.characterTransform.position.y;

        var minimumOffset = 0.02f; //this is there so that line can't be completely 90 degrees down
        float minCastLength = heightToEndOfRod + minimumOffset;
        targetLineLength = ((maxCastLength - minCastLength) * chargeFrac) + minCastLength;

        if (yDir != 0)
        {
            beginLineAngle = 90;
        }
        else
        {
            float endLineAngle = 270 + (Mathf.Rad2Deg * Mathf.Acos(heightToEndOfRod / targetLineLength)); //Degrees
            beginLineAngle = endLineAngle + castAngle; //Degrees
        }

        timer = 0;
        dragTimer = 0;
        initialized = true;
    }

    public override void Execute()
    {
        if (!initialized)
        {
            return;
        }

        if (firstExecute)
        {
            firstExecute = false;

            /*
             *  Cant be in initialization (StartState function) or DelayedInitialization because if initialization is 
             *  run after execute in same frame, line will be active but not in correct position 
             */
            resources.fishinglineInstance.SetActive(true);
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

            lineStartPosition = resources.fishingRod.position;
            float currentLineLength = targetLineLength * (timer / castSpeed);
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
                BezierLine.CreateBezierLine(resources.points, lineStartPosition, lineMiddlePosition, lineEndPosition);
                resources.fishinglineRenderer.SetPositions(resources.points);

                resources.fishinglineTransform.position = lineEndPosition; //For moving bobber to end of line
            }
        }
        else
        {
            InvokeChangePhase(typeof(FishingBobbingPhase), new object[] { lineMiddlePosition, lineEndPosition });
            return;
        }
    }

    public override void EndState()
    {
        if (initializeCoroutine != null)
            Coroutines.Instance.StopCoroutine(initializeCoroutine); //If state was finished before actual initialization
    }
}

public class FishingBobbingPhase : FishingPhase
{
    private static readonly float bobHeight = 0.04f;
    private static readonly float bobSpeed = 3f;

    private HashSet<FishBehaviour> fishInRange = new HashSet<FishBehaviour>();
    private ContactFilter2D wildlifeFilter;

    private float timer;
    private Vector2 lineEndDefaultPosition, lineMiddleDefaultPosition;
    private Vector2 lineEndPosition, lineMiddlePosition;
    private Coroutine alertCoroutine;

    private FishBehaviour hookedFish;

    public FishingBobbingPhase(FishingResources resources) : base(resources) { }

    public override void StartState(object[] args)
    {
        resources.animator.SetBool("LineCastFinished", true);
        resources.animator.SetBool("LineOut", true);
        resources.animator.speed = 1;
        resources.fishinglineInstance.SetActive(true);

        timer = 0;
        lineMiddleDefaultPosition = (Vector2)args[0];
        lineEndDefaultPosition = (Vector2)args[1];

        lineEndPosition = lineEndDefaultPosition;
        lineMiddlePosition = lineMiddleDefaultPosition;

        //Create fish contact filter
        {
            wildlifeFilter = new ContactFilter2D();
            wildlifeFilter.useTriggers = false;
            wildlifeFilter.SetLayerMask(1 << LayerMask.NameToLayer("Wildlife"));
            wildlifeFilter.useLayerMask = true;
        }

        alertCoroutine = Coroutines.Instance.StartCoroutine(AlertNearbyFishToTarget(lineEndPosition));
    }

    public override void Execute()
    {
        float offset = Mathf.Sin(bobSpeed * timer) * bobHeight;
        lineMiddlePosition = new Vector2(lineMiddleDefaultPosition.x, lineMiddleDefaultPosition.y + (offset / 2));
        lineEndPosition = new Vector2(lineEndDefaultPosition.x, lineEndDefaultPosition.y + offset);

        Vector2 lineStartPosition = resources.fishingRod.position; //This will change by bobbing up and down so needs to be in execute

        BezierLine.CreateBezierLine(resources.points, lineStartPosition, lineMiddlePosition, lineEndPosition);
        resources.fishinglineRenderer.SetPositions(resources.points);

        resources.fishinglineTransform.position = lineEndPosition; //For moving bobber to end of line

        timer += Time.deltaTime;
    }

    public override void EndState()
    {
        Coroutines.Instance.StopCoroutine(alertCoroutine);

        foreach (FishBehaviour fish in fishInRange)
        {
            if (fish != null && fish != hookedFish)
            {
                fish.TryStartFlee();
                fish.OnHooked -= OnFishBite;
            }
        }
        fishInRange.Clear();
    }

    public IEnumerator AlertNearbyFishToTarget(Vector2 lineEndPosition)
    {
        List<Collider2D> results = new List<Collider2D>(5);
        Physics2D.OverlapCircle(lineEndPosition, FishInformation.FISH_SEEING_DISTANCE, wildlifeFilter, results);

        foreach (FishBehaviour fish in fishInRange)
        {
            if (fish != null)
                fish.OnHooked -= OnFishBite;
        }
        fishInRange.Clear(); //Refresh every time

        foreach (Collider2D c in results)
        {
            GameObject obj = c.gameObject;
            if (obj.TryGetComponent<FishBehaviour>(out FishBehaviour component))
            {
                component.SetTarget(lineEndPosition);
                component.OnHooked += OnFishBite;
                fishInRange.Add(component);
            }
        }

        yield return new WaitForSeconds(3); //No need to do every frame.
    }

    public void OnFishBite(FishBehaviour hookedFish)
    {
        this.hookedFish = hookedFish;
        fishInRange.Remove(hookedFish); //So it doesn't flee
        InvokeChangePhase(typeof(FishingHookedPhase), new object[] { hookedFish.transform });
        return;
    }
}

public class FishingHookedPhase : FishingPhase
{
    protected Transform swimmingFishTransform;
    protected Vector2 lineEndPosition, lineStartPosition;

    public FishingHookedPhase(FishingResources resources) : base(resources) { }

    public override void StartState(object[] args)
    {
        resources.animator.SetBool("LineCastFinished", true);
        resources.animator.SetBool("LineOut", true);
        resources.animator.SetBool("Hooked", true);
        resources.fishinglineInstance.SetActive(true);
        resources.fishinglineRenderer.positionCount = 2;

        swimmingFishTransform = (Transform)args[0];
    }

    public override void Execute()
    {
        if (swimmingFishTransform == null)
        {
            InvokeChangePhase(typeof(FishingDefaultPhase), null);
            return;
        }

        lineEndPosition = swimmingFishTransform.position + ((-swimmingFishTransform.right) * FishInformation.FISH_WORLD_WIDTH / 2);
        lineStartPosition = resources.fishingRod.position;

        resources.fishinglineTransform.position = lineEndPosition; //For moving bobber to end of line
        resources.fishinglineRenderer.SetPositions(new Vector3[] { lineStartPosition, lineEndPosition });
    }

    public override void EndState()
    {
        if (swimmingFishTransform != null)
            swimmingFishTransform.GetComponent<FishBehaviour>().TryStartFlee();
    }
}

public struct FishingResources
{
    public static readonly int numPoints = 10;

    public readonly Animator animator;
    public readonly GameObject fishinglineInstance;
    public readonly LineRenderer fishinglineRenderer;
    public readonly Transform fishinglineTransform;
    public readonly Transform characterTransform;
    public readonly Transform fishingRod;
    public readonly Vector3[] points;
    public readonly object[] additionalResources;

    public FishingResources(Transform characterTransform, object[] additionalResources = null)
    {
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

        this.characterTransform = characterTransform;
        animator = characterTransform.GetComponent<Animator>();

        fishinglineInstance = GameObject.Instantiate(ResourceManager.Instance.FishingLinePrefab);
        fishinglineInstance.transform.SetParent(characterTransform);
        fishinglineInstance.SetActive(false);

        fishinglineRenderer = fishinglineInstance.GetComponent<LineRenderer>();
        fishinglineRenderer.positionCount = numPoints;
        fishinglineRenderer.generateLightingData = true;

        fishinglineTransform = fishinglineInstance.transform;

        fishingRod = FindChildWithTag(characterTransform, "FishingRod");

        points = new Vector3[numPoints];

        this.additionalResources = additionalResources;
    }
}
