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

    private HashSet<GameObject> fishInRange  = new HashSet<GameObject>();


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
            lineRenderer.generateLightingData = true;

            fishingLineTransform = fishingLineInstance.transform;

            fishingLineInstance.SetActive(false);

            points = new Vector3[numPoints];
        }
        //Configure progress bar
        {
            progressBar = new ProgressBar(progressBarCanvas);
            progressBar.Show(false);
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
        get { return (fishingState == FishingStates.None); }
    }

    public void Execute()
    {
        if (Input.GetButtonDown("Primary"))
        {
            if (fishingState == FishingStates.None)
            {
                SwitchFishingState(FishingStates.Charging, null);
            }
        }
    }

    public void StartState(object[] args)
    {
        animator.SetLayerWeight(animator.GetLayerIndex("Fishing"), 1);
    }

    public bool TryEndState()
    {
        if (fishingState == FishingStates.None)
        {
            animator.SetLayerWeight(animator.GetLayerIndex("Fishing"), 0);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void OnFishBite(GameObject hookedFish)
    {
        foreach (GameObject fish in fishInRange)
        {
            if (fish != null && fish != hookedFish)
                fish.GetComponent<FishBehaviour>().StartFlee();
        }

        SwitchFishingState(FishingStates.FishHooked, new object[] { hookedFish.transform });
    }

    private void SwitchFishingState(FishingStates proposedState, object[] args)
    {
        fishingState = proposedState;

        //Start new state
        switch (proposedState)
        {
            case (FishingStates.None):
                animator.SetBool("LineOut", false);
                animator.SetBool("Hooked", false);
                fishingLineInstance.SetActive(false);

                fishingRod.gameObject.SetActive(false);

                foreach (GameObject fish in fishInRange)
                {
                    if (fish != null)
                        fish.GetComponent<FishBehaviour>().StartFlee();
                }
                fishInRange.Clear();

                lineRenderer.positionCount = numPoints;

                break;

            case (FishingStates.Charging):
                //Animation
                {
                    PlayerMovement.Instance.StopMovement(); //TODO: Maybe there could be a better way to call this?
                    animator.SetBool("LineCastFinished", false);
                    animator.SetBool("LineOut", true);
                    animator.speed = 0;
                }
                fishingRod.gameObject.SetActive(true);
                progressBar.RectTransform.anchoredPosition = (new Vector2(playerTransform.position.x, playerTransform.position.y + 1.5f));
                progressBar.Show(true);

                StartCoroutine(Charging());
                break;

            case (FishingStates.LineCasting):
                //fishingLineInstance.SetActive(true); This had to be set in coroutine to because yield was making line appear before it was supposed to
                animator.speed = 1;
                progressBar.Show(false);

                float chargeTimer = (float)args[0];
                StartCoroutine(LineCasting(chargeTimer));

                break;

            case (FishingStates.Bobbing):
                animator.SetBool("LineCastFinished", true);

                Vector2 lineMiddlePosition = (Vector2)args[0];
                Vector2 lineEndPosition = (Vector2)args[1];
                StartCoroutine(LineBobbing(lineMiddlePosition, lineEndPosition));

                break;

            case (FishingStates.FishHooked):
                animator.SetBool("Hooked", true);
                lineRenderer.positionCount = 2;

                Transform fishTransform = (Transform)args[0];
                StartCoroutine(FishHooked(fishTransform));
                break;

            default:
                break;
        }
    }

    IEnumerator Charging()
    {
        float timer = 0;
        while (true)
        {
            timer += Time.deltaTime;

            if (timer >= maxCastHoldTime)
                timer = maxCastHoldTime;

            progressBar.SetFill(timer / maxCastHoldTime);

            if (Input.GetButtonUp("Primary") || timer >= maxCastHoldTime)
            {
                SwitchFishingState(FishingStates.LineCasting, new object[] { timer });
                break;
            }

            yield return 0;
        }
    }

    IEnumerator LineCasting(float chargeTimer)
    {
        yield return 0; //Wait a frame to get proper fishing rod position (HeightToEndOfRod could return 0 if this isn't here) TODO: maybe there is better way?
        fishingLineInstance.SetActive(true); //Needs to go after yield

        Vector2 playerDirectionVector = PlayerMovement.Instance.direction.directionVector;

        Vector2 lineEndPosition = new Vector2(0,0), lineMiddlePosition = new Vector2(0, 0);

        float heightToEndOfRod = fishingRod.position.y - playerTransform.position.y;

        var minimumOffset = 0.02f; //this is there so that line can't be completely 90 degrees down
        float minCastLength = heightToEndOfRod + minimumOffset;
        float maxLineLength = ((maxCastLength - minCastLength) * (chargeTimer / maxCastHoldTime)) + minCastLength;

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

        SwitchFishingState(FishingStates.Bobbing, new object[] { lineMiddlePosition, lineEndPosition });
    }

    IEnumerator LineBobbing(Vector2 lineMiddlePosition, Vector2 lineEndPosition)
    {
        float timer = 0;
        Vector2 lineEndDefaultPosition = lineEndPosition;
        Vector2 lineMiddleDefaultPosition = lineMiddlePosition;

        while (fishingState == FishingStates.Bobbing)
        {
            StartCoroutine(AlertNearbyFishToTarget(lineEndPosition));

            Vector2 lineStartPosition = fishingRod.position;

            float offset = Mathf.Sin(bobSpeed * timer) * bobHeight;
            lineMiddlePosition = new Vector2(lineMiddleDefaultPosition.x, lineMiddleDefaultPosition.y + (offset / 2));
            lineEndPosition = new Vector2(lineEndDefaultPosition.x, lineEndDefaultPosition.y + offset);

            CreateBezierLine(lineStartPosition, lineMiddlePosition, lineEndPosition);
            fishingLineTransform.position = lineEndPosition; //For moving bobber to end of line

            timer += Time.deltaTime;

            if (Input.GetButtonDown("Primary"))
            {
                SwitchFishingState(FishingStates.None, null);
                break;
            }

            yield return 0;
        }
    }

    IEnumerator FishHooked(Transform fish)
    {
        int clickCount = 5; //Todo: Come up with something else other then clicks

        while (fishingState == FishingStates.FishHooked)
        {
            Vector2 lineEndPosition = fish.position + ((-fish.right) * FishManager.FISH_WORLD_WIDTH / 2);
            Vector2 lineStartPosition = fishingRod.position;

            fishingLineTransform.position = lineEndPosition; //For moving bobber to end of line
            lineRenderer.SetPositions(new Vector3[] { lineStartPosition, lineEndPosition });

            if (Input.GetButtonDown("Primary"))
                clickCount--;

            if (clickCount <= 0)
            {
                Destroy(fish.gameObject);
                SwitchFishingState(FishingStates.None, null);
            }
            yield return 0; 
        }
    }


    IEnumerator AlertNearbyFishToTarget(Vector2 lineEndPosition)
    {
        while (fishingState == FishingStates.Bobbing)
        {
            List<Collider2D> results = new List<Collider2D>();
            Physics2D.OverlapCircle(lineEndPosition, fishScanRadius, fishFilter, results);
            fishInRange.Clear(); //Refresh every time

            foreach (Collider2D fishCollider in results)
            {
                GameObject obj = fishCollider.gameObject;
                fishCollider.gameObject.GetComponent<FishBehaviour>().SetTarget(lineEndPosition);
                fishInRange.Add(obj);
            }

            yield return new WaitForSeconds(1); //No need to do every frame.
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
        None,
        Charging,
        LineCasting,
        Bobbing,
        FishHooked
    }
}
