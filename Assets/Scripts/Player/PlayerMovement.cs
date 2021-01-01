using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float slowMoveSpeed = 0.65f;
    private Transform playerTransform;
    //private SpriteRenderer playerRenderer;

    private float boxColliderSizeX;
    public static float boxColliderSizeY;
    public static float BUFFER;

    private Animator animator;

    private int currentTileLayer; //Starts at 0 which means the player is on the sand layer, which means we need to check collision at land layer 0
    public int CurrentTileLayer => currentTileLayer;

    public delegate void OnPlayerMove(Vector2 position, bool slow, Vector2 directionVector);
    public event OnPlayerMove PlayerMoved;

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
        animator = GetComponent<Animator>();

        boxColliderSizeX = CollisionManager.boxColliderSizeX;
        boxColliderSizeY = CollisionManager.boxColliderSizeY;
        BUFFER = CollisionManager.BUFFER;
    }

    //Called from PlayerInitialization
    public void InitializeLayerAndDepth()
    {
        Vector2Int tilePos = new Vector2Int(Mathf.RoundToInt(playerTransform.position.x), Mathf.RoundToInt(playerTransform.position.y));

        TileInformationManager.Instance.TryGetTileInformation(tilePos, out TileInformation tileInfo);
        int layerNum = tileInfo.layerNum;

        currentTileLayer = layerNum;

        //Set depth
        playerTransform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, DynamicZDepth.GetDynamicZDepth(playerTransform.position, DynamicZDepth.PLAYER_OFFSET));
    }

    public void Execute()
    {
        Vector3 previousPos = playerTransform.position;

        //Animation
        Vector2 directionVector;
        {
            int moveH = (int)Input.GetAxisRaw("Horizontal");
            int MoveV = (int)Input.GetAxisRaw("Vertical");
            animator.SetBool("Walking", moveH != 0 || MoveV != 0);
            animator.speed = Input.GetButton("Slow move") ? 0.5f : 1f;
            directionVector = new Vector2(moveH, MoveV);
        }
        //Movement
        {
            if (!walkingOnStairs)
                RegularMovement();
            else
                StairsMovement();
        }

        if (directionVector.x != 0 || directionVector.y != 0)
            PlayerMoved?.Invoke(playerTransform.position, Input.GetButton("Slow move"), directionVector);

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
        HashSet<Vector2Int> beforeTiles = new HashSet<Vector2Int>() {
                new Vector2Int(Mathf.RoundToInt(playerTransform.position.x - boxColliderSizeX / 2f), Mathf.RoundToInt(playerTransform.position.y + boxColliderSizeY / 2f)),
                new Vector2Int(Mathf.RoundToInt(playerTransform.position.x + boxColliderSizeX / 2f), Mathf.RoundToInt(playerTransform.position.y + boxColliderSizeY / 2f)),
                new Vector2Int(Mathf.RoundToInt(playerTransform.position.x - boxColliderSizeX / 2f), Mathf.RoundToInt(playerTransform.position.y - boxColliderSizeY / 2f)),
                new Vector2Int(Mathf.RoundToInt(playerTransform.position.x + boxColliderSizeX / 2f), Mathf.RoundToInt(playerTransform.position.y - boxColliderSizeY / 2f))
            };

        //Movement
        {
            var proposedX = playerTransform.position.x + movement.x;
            var proposedY = playerTransform.position.y + movement.y;

            if (CollisionManager.CheckForCollisionMovement(playerTransform.position, new Vector2(proposedX, proposedY), currentTileLayer, out bool collisionX, out bool collisionY))
            {
                if (CollisionManager.CheckForStairsMovement(playerTransform.position, new Vector2(proposedX, proposedY), currentTileLayer, out StairsStartPosition stairsStartPosition))
                {
                    //Start stairs Movement
                    StartStairsMovement(stairsStartPosition);
                    playerTransform.position = new Vector3(proposedX, proposedY, DynamicZDepth.GetDynamicZDepth(proposedY, DynamicZDepth.CHARACTER_ON_STAIRS));
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
            HashSet<Vector2Int> afterTiles = new HashSet<Vector2Int>() {
                new Vector2Int(Mathf.RoundToInt(playerTransform.position.x - boxColliderSizeX / 2f), Mathf.RoundToInt(playerTransform.position.y + boxColliderSizeY / 2f)),
                new Vector2Int(Mathf.RoundToInt(playerTransform.position.x + boxColliderSizeX / 2f), Mathf.RoundToInt(playerTransform.position.y + boxColliderSizeY / 2f)),
                new Vector2Int(Mathf.RoundToInt(playerTransform.position.x - boxColliderSizeX / 2f), Mathf.RoundToInt(playerTransform.position.y - boxColliderSizeY / 2f)),
                new Vector2Int(Mathf.RoundToInt(playerTransform.position.x + boxColliderSizeX / 2f), Mathf.RoundToInt(playerTransform.position.y - boxColliderSizeY / 2f))
            };

            foreach (Vector2Int beforeTile in beforeTiles)
            {
                bool foundTile = false;
                foreach (Vector2Int afterTile in afterTiles)
                {
                    if (afterTile == beforeTile)
                        foundTile = true;
                }
                if (!foundTile)
                {
                    TileInformationManager.Instance.TryGetTileInformation(beforeTile, out TileInformation t);
                    t.StepOff();
                }
                else
                    afterTiles.Remove(beforeTile);
            }
            foreach (Vector2Int afterTile in afterTiles)
            {
                //We removed all duplicates in last loop so we can directly call step on function
                TileInformationManager.Instance.TryGetTileInformation(afterTile, out TileInformation t);
                t.StepOn();
            }
        }
    }

    //Player should not be moving more than 1 block a frame!! It will break things!!
    private Vector2 GetMovementVector(int rawX, int rawY)
    {
        Vector2 movement = new Vector2(rawX, rawY).normalized * Time.deltaTime * (Input.GetButton("Slow move") ? slowMoveSpeed : moveSpeed);
        {
            if (Mathf.Abs(movement.x) > 0.5f)
                movement.x = Mathf.Sign(movement.x) * 0.5f;
            if (Mathf.Abs(movement.y) > 0.5f)
                movement.y = Mathf.Sign(movement.y) * 0.5f;
        }
        return movement;
    }

    private bool walkingOnStairs;
    private Vector2Int endTile;
    private Vector2Int beginTile;
    private int endTileLayer;
    private int stairsXDirection;
    private int stairsYDirection;
    private float StairsSortingDepth;

    private void StartStairsMovement(StairsStartPosition stairsStartPosition)
    {
        walkingOnStairs = true;
        beginTile = new Vector2Int(Mathf.RoundToInt(playerTransform.position.x), Mathf.RoundToInt(playerTransform.position.y));
        endTile = stairsStartPosition.endPosition;
        endTileLayer = stairsStartPosition.endLayerNum;
        stairsXDirection = (endTile.x != beginTile.x) ? (int)Mathf.Sign(endTile.x - beginTile.x) : 0; // -1 left, 0 none, 1 right
        stairsYDirection = (endTile.y != beginTile.y) ? (int)Mathf.Sign(endTile.y - beginTile.y) : 0; // -1 left, 0 none, 1 right
        StairsSortingDepth = (endTile.y < beginTile.y) ? DynamicZDepth.GetDynamicZDepth(endTile, DynamicZDepth.CHARACTER_ON_STAIRS) : DynamicZDepth.GetDynamicZDepth(beginTile, DynamicZDepth.CHARACTER_ON_STAIRS);

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
            Vector2Int proposedXTile = new Vector2Int(Mathf.RoundToInt(proposedX), Mathf.RoundToInt(playerTransform.position.y));

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

        HashSet<Vector2Int> proposedCornerTiles = new HashSet<Vector2Int>()
        {   
            new Vector2Int(Mathf.RoundToInt(proposedX - (boxColliderSizeX / 2f)), Mathf.RoundToInt(proposedY + (boxColliderSizeY / 2f))), //TopLeft
            new Vector2Int(Mathf.RoundToInt(proposedX + (boxColliderSizeX / 2f)), Mathf.RoundToInt(proposedY + (boxColliderSizeY / 2f))), //TopRight
            new Vector2Int(Mathf.RoundToInt(proposedX - (boxColliderSizeX / 2f)), Mathf.RoundToInt(proposedY - (boxColliderSizeY / 2f))), //BottomLeft
            new Vector2Int(Mathf.RoundToInt(proposedX + (boxColliderSizeX / 2f)), Mathf.RoundToInt(proposedY - (boxColliderSizeY / 2f)))  //BottomRight
        };

        //Check for collision
        {
            foreach (Vector2Int corner in proposedCornerTiles)
            {
                if (corner == beginTile && CollisionManager.CheckForCollisionOnTile(corner, currentTileLayer))
                    return;
                else if (corner == endTile && CollisionManager.CheckForCollisionOnTile(corner, endTileLayer))
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