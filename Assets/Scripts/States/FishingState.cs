using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingState : MonoBehaviour, IPlayerState
{
    [SerializeField] private GameObject fishingLinePrefab = null;
    [SerializeField] private GameObject player = null;
    [SerializeField] private Canvas progressBarCanvas = null;

    private ProgressBar progressBar;
    private GameObject fishingLineInstance;
    private LineRenderer lineRenderer;
    private Transform fishingLineTransform;
    private Animator animator;
    private Transform playerTransform;
    private Transform fishingRod;
    private Vector3[] points;
    private ContactFilter2D fishFilter;
    private FishingStates fishingState;

    #region line configuration
    //**************************************
    private int numPoints = 10;
    //**************************************
    private float castAngle = 150f;
    private float maxCastLength = 2.5f;
    private float maxCastHoldTime = 1f;
    //**************************************
    private float castSpeed = 0.6f;
    private float castDragSpeed = 0.7f;
    //**************************************
    private float dragOffsetAngle = 15f;
    //**************************************
    private float bobHeight = 0.04f;
    private float bobSpeed = 3f;
    //**************************************
    private float fishScanRadius = FishManager.FISH_SEEING_DISTANCE;
    #endregion

    private static FishingState _instance;
    public static FishingState Instance { get { return _instance; } }
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
        //Configure fishing line object
        {
            fishingLineInstance = Instantiate(fishingLinePrefab);

            lineRenderer = fishingLineInstance.GetComponent<LineRenderer>();
            lineRenderer.positionCount = numPoints;

            fishingLineTransform = fishingLineInstance.transform;

            fishingLineInstance.SetActive(false);

            points = new Vector3[numPoints];
        }
        //Configure progress bar
        {
            progressBar = new ProgressBar(progressBarCanvas);
            progressBar.Hide();
        }
        //Find components
        {
            animator = player.GetComponent<Animator>();
            playerTransform = player.transform;

            //Find fishing rod object in player
            foreach (Transform t in playerTransform)
            {
                if (t.tag == "FishingRod")
                {
                    fishingRod = t;
                }
            }
        }
        //Create fish contact filter
        {
            fishFilter = new ContactFilter2D();
            fishFilter.useTriggers = false;
            fishFilter.SetLayerMask(1 << LayerMask.NameToLayer("Fish"));
            fishFilter.useLayerMask = true;
        }
    }

    public bool AllowMovement
    {
        get { return (fishingState == FishingStates.none); }
    }

    public void Execute()
    {
        if (Input.GetButtonDown("Primary"))
        {
            if (fishingState == FishingStates.none)
            {
                //Animation
                {
                    PlayerMovement.Instance.StopMovement(); //TODO: Maybe there could be a better way to call this?
                    animator.SetBool("LineCastFinished", false);
                    animator.SetLayerWeight(animator.GetLayerIndex("Fishing"), 1);
                    animator.SetBool("Fishing", true);
                    animator.speed = 0;
                }
                fishingRod.gameObject.SetActive(true);
                StartCoroutine(LineHolding());
            }
        }
    }

    public void StartState(object[] args)
    {
        //Nothing needed yet  
    }

    public bool TryEndState()
    {
        return (fishingState == FishingStates.none);
    }

    IEnumerator LineHolding()
    {
        fishingState = FishingStates.holding;
        progressBar.SetPosition(new Vector2(playerTransform.position.x, playerTransform.position.y+1.5f));
        progressBar.Show();

        float timer = 0;
        while (true)
        {
            timer += Time.deltaTime;

            if (timer >= maxCastHoldTime)
                timer = maxCastHoldTime;

            progressBar.SetFill(timer / maxCastHoldTime);

            if (Input.GetButtonUp("Primary") || timer >= maxCastHoldTime)
            {
                animator.speed = 1;
                progressBar.Hide();
                StartCoroutine(LineCasting(timer / maxCastHoldTime));
                break;
            }

            yield return 0;
        }
    }

    IEnumerator LineCasting(float timerFraction)
    {
        fishingState = FishingStates.lineCasting;

        yield return 0; //Wait a frame to get proper fishing rod position (HeightToEndOfRod could return 0 if this isn't here) TODO: maybe there is better way?
        fishingLineInstance.SetActive(true);

        Vector2 playerDirectionVector = PlayerMovement.Instance.direction.directionVector;

        Vector2 lineEndPosition = new Vector2(0,0), lineMiddlePosition = new Vector2(0, 0);

        float heightToEndOfRod = fishingRod.position.y - playerTransform.position.y;
        var minimumOffset = 0.02f; //this is there so that line can't be completely 90 degrees down
        float minCastLength = heightToEndOfRod + minimumOffset;
        float maxLineLength = ((maxCastLength - minCastLength) * timerFraction) + minCastLength;

        float beginLineAngle;
        //Horizontal direction
        if (playerDirectionVector.x != 0)
        {
            float endLineAngle = 270 + (Mathf.Rad2Deg * Mathf.Acos(heightToEndOfRod / maxLineLength)); //Degrees
            beginLineAngle = endLineAngle + castAngle; //Degrees
        }
        //Vertical direction
        else
        {
            beginLineAngle = 90;
        }

        float timer = 0;
        float dragTimer = 0;

        while (timer < castSpeed || dragTimer < castDragSpeed) {
            #region increment timer
            if (timer < castSpeed)
                timer += Time.deltaTime;
            else
                timer = castSpeed;

            if (dragTimer < castDragSpeed)
                dragTimer += Time.deltaTime;
            else
                dragTimer = castDragSpeed;
            #endregion

            Vector2 lineStartPosition = fishingRod.position;
            float currentLineLength = maxLineLength * (timer / castSpeed);
            float lineAngle = beginLineAngle - ((timer / castSpeed) * castAngle);
            float lineDragAngle = beginLineAngle - ((dragTimer / castDragSpeed) * castAngle) - dragOffsetAngle;
            
            //Horizontal direction
            if (playerDirectionVector.x != 0)
            {
                //Get line end position
                {
                    float lineEndX = playerDirectionVector.x * currentLineLength * Mathf.Cos(Mathf.Deg2Rad*lineAngle);
                    float lineEndY = currentLineLength * Mathf.Sin(Mathf.Deg2Rad * lineAngle);
                    lineEndPosition = new Vector2(lineStartPosition.x + lineEndX, lineStartPosition.y + lineEndY);
                }
                //Get line drag (middle) position
                {
                    float dragLineEndX = playerDirectionVector.x * (currentLineLength / 2) * Mathf.Cos(Mathf.Deg2Rad * lineDragAngle);
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
                    float lineEndX = (1/4f)*currentLineLength * Mathf.Cos(Mathf.Deg2Rad * lineAngle);
                    float lineEndY = currentLineLength * Mathf.Sin(Mathf.Deg2Rad * lineAngle);
                    lineEndPosition = new Vector2(lineStartPosition.x + lineEndX, lineStartPosition.y + lineEndY);
                }
                //Get line drag (middle) position
                {
                    float dragLineEndX = (1/4f)*(currentLineLength / 2) * Mathf.Cos(Mathf.Deg2Rad * lineDragAngle);
                    float dragLineEndY = (currentLineLength / 2) * Mathf.Sin(Mathf.Deg2Rad * lineDragAngle);
                    lineMiddlePosition = new Vector2(lineStartPosition.x + dragLineEndX, lineStartPosition.y + dragLineEndY);
                }
            }
            {
                CreateBezierLine(lineStartPosition, lineMiddlePosition, lineEndPosition);
                fishingLineTransform.position = lineEndPosition; //For moving bobber to end of line
            }

            yield return 0;
        }

        animator.SetBool("LineCastFinished", true);

        AlertNearbyFish(lineEndPosition);

        StartCoroutine(LineBobbing(lineMiddlePosition, lineEndPosition));
    }

    IEnumerator LineBobbing(Vector2 lineMiddlePosition, Vector2 lineEndPosition)
    {
        fishingState = FishingStates.bobbing;
        float timer = 0;
        Vector2 lineEndDefaultPosition = lineEndPosition;
        Vector2 lineMiddleDefaultPosition = lineMiddlePosition;

        while (true)
        {
            Vector2 lineStartPosition = fishingRod.position;

            float offset = Mathf.Sin(bobSpeed * timer) * bobHeight;
            lineMiddlePosition = new Vector2(lineMiddleDefaultPosition.x, lineMiddleDefaultPosition.y + (offset / 2));
            lineEndPosition = new Vector2(lineEndDefaultPosition.x, lineEndDefaultPosition.y + offset);

            CreateBezierLine(lineStartPosition, lineMiddlePosition, lineEndPosition);
            fishingLineTransform.position = lineEndPosition; //For moving bobber to end of line

            timer += Time.deltaTime;

            if (Input.GetButtonDown("Primary"))
            {
                //Animation
                {
                    animator.SetBool("Fishing", false);
                    animator.SetLayerWeight(animator.GetLayerIndex("Fishing"), 0);
                }
                fishingRod.gameObject.SetActive(false);
                fishingLineInstance.SetActive(false);
                fishingState = FishingStates.none;

                break;
            }

            yield return 0;
        }
    }

    private void AlertNearbyFish(Vector2 bobberPosition)
    {
        List<Collider2D> results = new List<Collider2D>();
        Physics2D.OverlapCircle(bobberPosition, fishScanRadius, fishFilter, results);

        foreach (Collider2D fishCollider in results)
        {
            Debug.Log("Found");
            fishCollider.gameObject.GetComponent<FishBehaviour>().ChangeTarget(bobberPosition);
        }
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

    private enum FishingStates
    {
        none,
        holding,
        lineCasting,
        bobbing,
    }
}
