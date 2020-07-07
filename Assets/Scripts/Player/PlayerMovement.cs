using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 0f;
    private Transform playerTransform;
    //private SpriteRenderer playerRenderer;

    private float boxColliderSizeX;
    public static float boxColliderSizeY;
    public static float BUFFER;

    public PlayerDirection direction;
    private Animator animator;

    private int currentTileLayer; //Starts at 0 which means the player is on the sand layer, which means we need to check collision at land layer 0

    private static PlayerMovement _instance;
    public static PlayerMovement Instance { get { return _instance; } }
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

        playerTransform = transform;
        direction = new PlayerDirection();
        animator = GetComponent<Animator>();

        boxColliderSizeX = CollisionManager.boxColliderSizeX;
        boxColliderSizeY = CollisionManager.boxColliderSizeY;
        BUFFER = CollisionManager.BUFFER;
    }

    //Called from PlayerInitialization
    public void InitializeLayerAndDepth()
    {
        Vector3Int tilePos = new Vector3Int(Mathf.RoundToInt(playerTransform.position.x), Mathf.RoundToInt(playerTransform.position.y), 0);
        int layerNum = TileInformationManager.Instance.GetTileInformation(tilePos).layerNum;
        currentTileLayer = layerNum;

        //Set depth
        playerTransform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, DynamicZDepth.GetDynamicZDepth(playerTransform.position, DynamicZDepth.PLAYER_OFFSET));
    }

    public void Execute()
    {
        int animDirectionX, animDirectionY; // -1 or 1
        //Animation
        {
            animDirectionX = (int)Input.GetAxisRaw("Horizontal");
            animDirectionY = (int)Input.GetAxisRaw("Vertical");
            animator.SetBool("Walking", animDirectionX != 0 || animDirectionY != 0); //This should not be affected by mouse click function part below
        }
        //Point to direction of mouse on mouseclick
        if (Input.GetButton("Primary"))
        {
            float angle = MathFunctions.GetAngleBetweenPoints(playerTransform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            //Set 4 way direction (For fishing etc)
            {
                if (angle > 315 || angle < 45)
                {
                    direction.SetDirection(PlayerDirectionEnum.right);
                }
                else if (angle >= 45 && angle <= 135)
                {
                    direction.SetDirection(PlayerDirectionEnum.back);
                }
                else if (angle > 135 && angle < 225)
                {
                    direction.SetDirection(PlayerDirectionEnum.left);
                }
                else //Should be (angle >= 225 && angle <= 315
                {
                    direction.SetDirection(PlayerDirectionEnum.front);
                }
            }
            //Set direction for animation;
            {
                if (angle < 180)
                    animDirectionY = 1;
                else
                    animDirectionY = -1;
                if (angle < 90 || angle > 270)
                    animDirectionX = 1;
                else
                    animDirectionX = -1;
            }
        }
        {
            if (animDirectionX != 0)
                playerTransform.localScale = new Vector3(-animDirectionX, 1, 1);
            if (animDirectionY != 0)
                animator.SetFloat("Vertical", animDirectionY);
        }
        //Movement
        {
            if (!walkingOnStairs)
                RegularMovement();
            else
                StairsMovement();
        }

    }

    public void StopMovement()
    {
        animator.SetBool("Walking", false);
    }

    private void RegularMovement()
    {
        int rawX = (int)Input.GetAxisRaw("Horizontal");
        int rawY = (int)Input.GetAxisRaw("Vertical");

        Vector2 movement = GetMovementVector(rawX, rawY);
        if (movement.x == 0 && movement.y == 0)
            return;

        //For calling stepOn & stepOff functions
        HashSet<Vector3Int> beforeTiles = new HashSet<Vector3Int>() {
                new Vector3Int(Mathf.RoundToInt(playerTransform.position.x - boxColliderSizeX / 2f), Mathf.RoundToInt(playerTransform.position.y + boxColliderSizeY / 2f), 0),
                new Vector3Int(Mathf.RoundToInt(playerTransform.position.x + boxColliderSizeX / 2f), Mathf.RoundToInt(playerTransform.position.y + boxColliderSizeY / 2f), 0),
                new Vector3Int(Mathf.RoundToInt(playerTransform.position.x - boxColliderSizeX / 2f), Mathf.RoundToInt(playerTransform.position.y - boxColliderSizeY / 2f), 0),
                new Vector3Int(Mathf.RoundToInt(playerTransform.position.x + boxColliderSizeX / 2f), Mathf.RoundToInt(playerTransform.position.y - boxColliderSizeY / 2f), 0)
            };

        //Movement
        {
            var proposedX = playerTransform.position.x + movement.x;
            var proposedY = playerTransform.position.y + movement.y;

            if (CollisionManager.Instance.CheckForCollisionMovement(playerTransform.position, new Vector2(proposedX, proposedY), currentTileLayer, out bool collisionX, out bool collisionY))
            {
                if (CollisionManager.Instance.CheckForStairsMovement(playerTransform.position, new Vector2(proposedX, proposedY), currentTileLayer, out Vector3Int goal))
                {
                    //Start stairs Movement
                    StartStairsMovement(goal);
                    playerTransform.position = new Vector3(proposedX, proposedY, DynamicZDepth.GetDynamicZDepth(proposedY, DynamicZDepth.PLAYER_ON_STAIRS));
                    return;
                }
                else
                {
                    //If no stairs found, move player to furthest he can go without colliding (plus buffer)
                    if (collisionX)
                        proposedX = Mathf.RoundToInt(playerTransform.position.x) + rawX * (0.5f - (boxColliderSizeX / 2f + BUFFER));
                    if (collisionY)
                        proposedY = Mathf.RoundToInt(playerTransform.position.y) + rawY * (0.5f - (boxColliderSizeY / 2f + BUFFER));
                }
            }
            //Set position and depth
            {
                playerTransform.position = new Vector3(proposedX, proposedY, DynamicZDepth.GetDynamicZDepth(proposedY, DynamicZDepth.PLAYER_OFFSET));
            }
        }


        //Active object scripts
        {
            HashSet<Vector3Int> afterTiles = new HashSet<Vector3Int>() {
                new Vector3Int(Mathf.RoundToInt(playerTransform.position.x - boxColliderSizeX / 2f), Mathf.RoundToInt(playerTransform.position.y + boxColliderSizeY / 2f), 0),
                new Vector3Int(Mathf.RoundToInt(playerTransform.position.x + boxColliderSizeX / 2f), Mathf.RoundToInt(playerTransform.position.y + boxColliderSizeY / 2f), 0),
                new Vector3Int(Mathf.RoundToInt(playerTransform.position.x - boxColliderSizeX / 2f), Mathf.RoundToInt(playerTransform.position.y - boxColliderSizeY / 2f), 0),
                new Vector3Int(Mathf.RoundToInt(playerTransform.position.x + boxColliderSizeX / 2f), Mathf.RoundToInt(playerTransform.position.y - boxColliderSizeY / 2f), 0)
            };

            foreach (Vector3Int beforeTile in beforeTiles)
            {
                bool foundTile = false;
                foreach (Vector3Int afterTile in afterTiles)
                {
                    if (afterTile == beforeTile)
                        foundTile = true;
                }
                if (!foundTile)
                {
                    TileInformation t = TileInformationManager.Instance.GetTileInformation(beforeTile);
                    if (t != null)
                        t.StepOff();
                }
                else
                    afterTiles.Remove(beforeTile);
            }
            foreach (Vector3Int afterTile in afterTiles)
            {
                //We removed all duplicates in last loop so we can directly call step on function
                TileInformation t = TileInformationManager.Instance.GetTileInformation(afterTile);
                if (t != null)
                    t.StepOn();
            }
        }
    }

    //Player should not be moving more than 1 block a frame!! It will break things!!
    private Vector2 GetMovementVector(int rawX, int rawY)
    {
        Vector2 movement = new Vector2(rawX, rawY).normalized * Time.deltaTime * moveSpeed;
        {
            if (Mathf.Abs(movement.x) > 0.5f)
                movement.x = Mathf.Sign(movement.x) * 0.5f;
            if (Mathf.Abs(movement.y) > 0.5f)
                movement.y = Mathf.Sign(movement.y) * 0.5f;
        }
        return movement;
    }

    private bool walkingOnStairs;
    private Vector3Int endTile;
    private Vector3Int beginTile;
    private int endTileLayer;
    private int stairsXDirection;
    private int stairsYDirection;
    private float StairsSortingDepth;

    private void StartStairsMovement(Vector3Int goal)
    {
        walkingOnStairs = true;
        beginTile = new Vector3Int(Mathf.RoundToInt(playerTransform.position.x), Mathf.RoundToInt(playerTransform.position.y), 0);
        endTile = goal;
        endTileLayer = (endTile.y > beginTile.y) ? currentTileLayer + 1 : currentTileLayer - 1;
        stairsXDirection = (endTile.x != beginTile.x) ? (int)Mathf.Sign(endTile.x - beginTile.x) : 0; // -1 left, 0 none, 1 right
        stairsYDirection = (endTile.y != beginTile.y) ? (int)Mathf.Sign(endTile.y - beginTile.y) : 0; // -1 left, 0 none, 1 right
        StairsSortingDepth = (endTile.y < beginTile.y) ? DynamicZDepth.GetDynamicZDepth(endTile, DynamicZDepth.PLAYER_ON_STAIRS) : DynamicZDepth.GetDynamicZDepth(beginTile, DynamicZDepth.PLAYER_ON_STAIRS);

    }

    private void StairsMovement()
    {
        float proposedX;
        float proposedY;

        //Horizontal movemet stairs
        if (stairsXDirection != 0) {
            int rawX = (int)Input.GetAxisRaw("Horizontal");
            if (rawX == 0)
                return;

            Vector2 movement = GetMovementVector(rawX, 0);

            proposedX = playerTransform.position.x + movement.x;
            Vector3Int proposedXTile = new Vector3Int(Mathf.RoundToInt(proposedX), Mathf.RoundToInt(playerTransform.position.y), 0);

            if (proposedXTile == beginTile || proposedXTile == endTile)
            {
                proposedY = playerTransform.position.y;
            }
            else
            {   
                //Let's calculate where the player y is according to how far player's x is from begin tile position
                float xDistanceFromBeginTileEdge = Mathf.Abs(proposedX - beginTile.x) - 0.5f; //0.5f to get distance from edge of tile
                float yOffset = (stairsYDirection) * (xDistanceFromBeginTileEdge + (boxColliderSizeY / 2f));

                proposedY =  beginTile.y + yOffset;
            }
        }
        //Vertical movement stairs
        else
        {
            int rawY = (int)Input.GetAxisRaw("Vertical");
            if (rawY == 0)
                return;
            Vector2 movement = GetMovementVector(0, rawY);

            proposedX = playerTransform.position.x;
            proposedY = playerTransform.position.y + movement.y;
        }

        HashSet<Vector3Int> proposedCornerTiles = new HashSet<Vector3Int>()
        {   
            new Vector3Int(Mathf.RoundToInt(proposedX - (boxColliderSizeX / 2f)), Mathf.RoundToInt(proposedY + (boxColliderSizeY / 2f)), 0), //TopLeft
            new Vector3Int(Mathf.RoundToInt(proposedX + (boxColliderSizeX / 2f)), Mathf.RoundToInt(proposedY + (boxColliderSizeY / 2f)), 0), //TopRight
            new Vector3Int(Mathf.RoundToInt(proposedX - (boxColliderSizeX / 2f)), Mathf.RoundToInt(proposedY - (boxColliderSizeY / 2f)), 0), //BottomLeft
            new Vector3Int(Mathf.RoundToInt(proposedX + (boxColliderSizeX / 2f)), Mathf.RoundToInt(proposedY - (boxColliderSizeY / 2f)), 0)  //BottomRight
        };

        //Check for collision
        {
            foreach (Vector3Int corner in proposedCornerTiles)
            {
                if (corner == beginTile && CollisionManager.Instance.CheckForCollisionOnTile(corner, currentTileLayer))
                    return;
                else if (corner == endTile && CollisionManager.Instance.CheckForCollisionOnTile(corner, endTileLayer))
                    return;
            }
        }
        //Actual movement
        {
            playerTransform.position = new Vector3(proposedX, proposedY, StairsSortingDepth);
        }
        //End condition
        {
            //Horizontal end condition
            if (stairsXDirection != 0)
            {
                //Checks if all corners are in the same x position on beginTile or endTile
                if (!proposedCornerTiles.Any(cornerTile => cornerTile.x != endTile.x))
                {
                    currentTileLayer = endTileLayer;
                    walkingOnStairs = false;
                }
                else if (!proposedCornerTiles.Any(cornerTile => cornerTile.x != beginTile.x))
                {
                    walkingOnStairs = false;
                }
            }
            //Vertical end condition
            else
            {
                //Checks if all corners are in the same y position on beginTile or endTile
                if (!proposedCornerTiles.Any(cornerTile => cornerTile.y != endTile.y))
                {
                    currentTileLayer = endTileLayer;
                    walkingOnStairs = false;
                }
                else if (!proposedCornerTiles.Any(cornerTile => cornerTile.y != beginTile.y))
                {
                    walkingOnStairs = false;
                }
            }
        }
    }
}

public class PlayerDirection {
    public PlayerDirectionEnum directionEnum;
    public Vector2 directionVector;

    public PlayerDirection() {
        SetDirection(PlayerDirectionEnum.front);
    }

    public void SetDirection(PlayerDirectionEnum directionEnum)
    {
        this.directionEnum = directionEnum;
        switch (directionEnum)
        {
            case (PlayerDirectionEnum.back):
                directionVector = new Vector2(0, -1);
                break;
            case (PlayerDirectionEnum.front):
                directionVector = new Vector2(0, 1);
                break;
            case (PlayerDirectionEnum.left):
                directionVector = new Vector2(-1, 0);
                break;
            case (PlayerDirectionEnum.right):
                directionVector = new Vector2(1, 0);
                break;
            default:
                break;
        }
    }
}

public enum PlayerDirectionEnum
{
    back,
    front,
    left,
    right
}