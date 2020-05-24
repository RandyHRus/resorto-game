using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainManager : MonoBehaviour, IPlayerState
{
    [SerializeField] private Sprite landIndicatorSprite = null;
    [SerializeField] private Sprite sandIndicatorSprite = null;
    [SerializeField] private Tilemap landTileMapPrefab = null;
    [SerializeField] private GameObject tileGrid = null;
    [SerializeField] private Tilemap sandWaterTileMap = null;
    [SerializeField] private Tile[] fullSandTiles = null;
    [SerializeField] private List<StarterTilemaps> starterTilemaps = null;

    [System.Serializable]
    public struct StarterTilemaps
    {
        public Tilemap cliffTilemap;
        public Tilemap grassTilemap;
    }

    private List<TileMapLayer> tilemapLayers_; //Starts at 0, which is the first layer of land

    private GameObject indicator;
    private SpriteRenderer indicatorRenderer;

    private bool coroutineRunning = false;

    #region Terrain Tile Codes
    /*
     *  string represents sides and corners of sprite
     *  _______
     *  |0 1 2|
     *  |7   3|   - Numbers are indexes of string
     *  |6_5_4|   - 0 for false, 1 for true
     *            - Example code "01001000"
    */
    [SerializeField] private TileToCode[] sandWaterTileCodes = null;
    private Dictionary<string, int> sandWaterTileNameToCode = new Dictionary<string, int>();
    private Dictionary<int, Tile> sandWaterCodeToTile = new Dictionary<int, Tile>();


    /*
     *  string represents sides and corners of sprite
     *  _______
     *  |  2  |
     *  |1   3|   - First number 0=grass 1=cliff
     *  |_____|   - Following 4 numbers represents positions: 
     *                  grass: 0 for no grass 1 for grass
     *                  cliff: 0 for no cliff 1 for cliff
     *            - Example code "01000"
    */
    [SerializeField] private TileToCode[] landTileCodes = null;
    private Dictionary<string, int> landTileNameToCode = new Dictionary<string, int>();
    private Dictionary<int, Tile> landCodeToTile = new Dictionary<int, Tile>();

    [System.Serializable]
    public struct TileToCode
    {
        public string code;
        public Tile tile;
    }
    #endregion

    private class TileMapLayer
    {
        public Tilemap cliffTileMap;
        public Tilemap grassTileMap;

        public TileMapLayer(int layer)
        {
            Tilemap[] tilemaps = {
                cliffTileMap  = Instantiate(Instance.landTileMapPrefab),
                grassTileMap  = Instantiate(Instance.landTileMapPrefab)
            };

            for (int i = 0; i < tilemaps.Length; i++)
            {
                Tilemap map = tilemaps[i];
                map.transform.SetParent(Instance.tileGrid.transform);
                map.GetComponent<TilemapRenderer>().sortingLayerName = "LandTileMap";
                map.GetComponent<TilemapRenderer>().sortingOrder = 10 * layer + i;
            }
        }

        public TileMapLayer(Tilemap grassTileMap, Tilemap cliffTileMap)
        {
            this.cliffTileMap = cliffTileMap;
            this.grassTileMap = grassTileMap;
        }
    }

    public bool AllowMovement
    {
        get { return false; }
    }

    private static TerrainManager _instance;
    public static TerrainManager Instance { get { return _instance; } }
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
        //Initialize tile code dictionaries
        {

            foreach (TileToCode tileCode in sandWaterTileCodes)
            {
                sandWaterTileNameToCode.Add(tileCode.tile.name, System.Convert.ToInt32(tileCode.code, 2));
                sandWaterCodeToTile.Add(System.Convert.ToInt32(tileCode.code, 2), tileCode.tile);
            }
            foreach (TileToCode tileCode in landTileCodes)
            {
                landTileNameToCode.Add(tileCode.tile.name, System.Convert.ToInt32(tileCode.code, 2));
                landCodeToTile.Add(System.Convert.ToInt32(tileCode.code, 2), tileCode.tile);
            }
        }
        //Initialize starter terrain
        {
            tilemapLayers_ = new List<TileMapLayer>();

            foreach (StarterTilemaps maps in starterTilemaps)
            {
                tilemapLayers_.Add(new TileMapLayer(maps.grassTilemap, maps.cliffTilemap));
            }
        }
        //Setup indicator
        {
            indicator = new GameObject("Terrain Indicator");
            indicatorRenderer = indicator.AddComponent<SpriteRenderer>();
            indicatorRenderer.sortingLayerName = "Indicator";
            indicator.SetActive(false);
        }
    }

    private void Start()
    {
        //Initialize starter tile maps tile layers
        {
            for (int i = 0; i < TileInformationManager.tileCountX; i++)
            {
                for (int j = 0; j < TileInformationManager.tileCountY; j++)
                {
                    Vector3Int position = new Vector3Int(i, j, 0);
                    {
                        TileInformationManager.Instance.GetTileInformation(position).layerNum = GetHighestGrassLandLayer(position);
                    }
                }
            }
        }
    }

    public void StartState(object[] args)
    {
        indicator.SetActive(true);
    }

    public bool TryEndState()
    {
        if (coroutineRunning)
        {
            return false;
        }
        else
        {
            indicator.SetActive(false);
            return true;
        }
    }


    public void Execute()
    {
        if (coroutineRunning)
            return;

        Vector3Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();

        indicator.transform.position = new Vector2(mouseTilePosition.x, mouseTilePosition.y);
        indicatorRenderer.sprite = landIndicatorSprite;
        indicatorRenderer.color = ResourceManager.Instance.red;

        {
            if (TerrainRemoveable(mouseTilePosition, out int layerNumber))
            {
                indicatorRenderer.color = ResourceManager.Instance.yellow;

                if (Input.GetButtonDown("Secondary"))
                {
                    if (layerNumber == 0)
                    {
                        StartCoroutine(RemoveSand());
                        return;
                    }
                    else if (layerNumber >= 1)
                    {
                        StartCoroutine(RemoveLand(layerNumber));
                        return;
                    }
                }
            }
        }
        {
            if (TerrainPlaceable(mouseTilePosition, out int layerNumber))
            {
                indicatorRenderer.color = ResourceManager.Instance.green;

                if (layerNumber == 0)
                    indicatorRenderer.sprite = sandIndicatorSprite;

                if (Input.GetButtonDown("Primary"))
                {
                    if (layerNumber == 0)
                    {
                        StartCoroutine(PlaceSand());
                        return;
                    }
                    else if (layerNumber >= 1)
                    {
                        StartCoroutine(PlaceLand(layerNumber));
                        return;
                    }
                }
            }
        }
    }

    IEnumerator PlaceSand()
    {
        coroutineRunning = true;
        indicatorRenderer.sprite = null;
        Vector3Int previousTilePosition = new Vector3Int(-1, -1, -1);

        while (Input.GetButton("Primary"))
        {
            Vector3Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();

            if (mouseTilePosition != previousTilePosition)
                previousTilePosition = mouseTilePosition;
            else
                goto Skip;

            if (TerrainPlaceable(mouseTilePosition, out int ln))
            {
                if (ln != 0)
                    goto Skip;
            }
            else
                goto Skip;

            {
                Tile pickedSandTile = fullSandTiles[Random.Range(0, fullSandTiles.Length)];
                sandWaterTileMap.SetTile(mouseTilePosition, pickedSandTile);
                Dictionary<Vector3Int, int> neighboursToCheck = new Dictionary<Vector3Int, int>()
                {
                    {  new Vector3Int(mouseTilePosition.x-1, mouseTilePosition.y+1, 0), 0b00001000 }, //UpLeft    
                    {  new Vector3Int(mouseTilePosition.x,   mouseTilePosition.y+1, 0), 0b00001110 }, //Up        
                    {  new Vector3Int(mouseTilePosition.x+1, mouseTilePosition.y+1, 0), 0b00000010 }, //UpRight   
                    {  new Vector3Int(mouseTilePosition.x+1, mouseTilePosition.y,   0), 0b10000011 }, //Right    
                    {  new Vector3Int(mouseTilePosition.x+1, mouseTilePosition.y-1, 0), 0b10000000 }, //DownRight
                    {  new Vector3Int(mouseTilePosition.x,   mouseTilePosition.y-1, 0), 0b11100000 }, //Down      
                    {  new Vector3Int(mouseTilePosition.x-1, mouseTilePosition.y-1, 0), 0b00100000 }, //DownLeft 
                    {  new Vector3Int(mouseTilePosition.x-1, mouseTilePosition.y,   0), 0b00111000 }  //Left      
                };
                foreach (KeyValuePair<Vector3Int, int> neighbour in neighboursToCheck)
                {
                    TileBase sandAndWaterNeighbourTile = sandWaterTileMap.GetTile(neighbour.Key);
                    if (sandAndWaterNeighbourTile != null)
                    {
                        if (sandWaterTileNameToCode.TryGetValue(sandAndWaterNeighbourTile.name, out int code))
                        {
                            int tileCodeRepresentation = code | neighbour.Value;

                            if (sandWaterCodeToTile.TryGetValue(tileCodeRepresentation, out Tile tile))
                                sandWaterTileMap.SetTile(neighbour.Key, tile);
                            else
                                Debug.Log("Tile representation not found for tile code: " + tileCodeRepresentation);
                        }
                        else
                        {
                            //Removed because sand kept getting printed
                            //Debug.Log("Code representation not found for tile with name: " + sandAndWaterNeighbourTile.name);
                        }
                    }

                }
                //Placed sand
                {
                    TileInformationManager.Instance.GetTileInformation(mouseTilePosition).layerNum = GetHighestGrassLandLayer(mouseTilePosition);
                    ParticlesManager.Instance.PlaySmokeEffect(mouseTilePosition);
                }
            }
            Skip:
            yield return 0;
        }
        coroutineRunning = false;
    }

    IEnumerator RemoveSand()
    {
        coroutineRunning = true;
        indicatorRenderer.sprite = null;
        Vector3Int previousTilePosition = new Vector3Int(-1, -1, -1);

        while (Input.GetButton("Secondary"))
        {
            Vector3Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();

            if (mouseTilePosition != previousTilePosition)
                previousTilePosition = mouseTilePosition;

            if (TerrainRemoveable(mouseTilePosition, out int ln))
            {
                if (ln != 0)
                    goto Skip;
            }
            else
                goto Skip;

            Dictionary<Vector3Int, int> mouseTileBitsToAddFromNeighbours = new Dictionary<Vector3Int, int>()
                {
                    {  new Vector3Int(mouseTilePosition.x-1, mouseTilePosition.y+1, 0), 0b10000000 }, //UpLeft    
                    {  new Vector3Int(mouseTilePosition.x,   mouseTilePosition.y+1, 0), 0b11100000 }, //Up        
                    {  new Vector3Int(mouseTilePosition.x+1, mouseTilePosition.y+1, 0), 0b00100000 }, //UpRight   
                    {  new Vector3Int(mouseTilePosition.x+1, mouseTilePosition.y,   0), 0b00111000 }, //Right    
                    {  new Vector3Int(mouseTilePosition.x+1, mouseTilePosition.y-1, 0), 0b00001000 }, //DownRight
                    {  new Vector3Int(mouseTilePosition.x,   mouseTilePosition.y-1, 0), 0b00001110 }, //Down      
                    {  new Vector3Int(mouseTilePosition.x-1, mouseTilePosition.y-1, 0), 0b00000010 }, //DownLeft 
                    {  new Vector3Int(mouseTilePosition.x-1, mouseTilePosition.y,   0), 0b10000011 }  //Left      
                };

            //Change self tile
            {

                int mouseTileResult = 0b00000000;
                foreach (KeyValuePair<Vector3Int, int> neighbour in mouseTileBitsToAddFromNeighbours)
                {
                    if (LandTileExists(0, neighbour.Key))
                        mouseTileResult = mouseTileResult | neighbour.Value;
                }

                if (sandWaterCodeToTile.TryGetValue(mouseTileResult, out Tile tile))
                    sandWaterTileMap.SetTile(mouseTilePosition, tile);
                else
                    Debug.Log("Tile representation not found for tile code: " + mouseTileResult);
            }
            //Change neighbours tile
            {
                foreach (KeyValuePair<Vector3Int, int> neighbour in mouseTileBitsToAddFromNeighbours)
                {
                    if (LandTileExists(0, neighbour.Key))
                    {
                        continue;
                    }
                    Dictionary<Vector3Int, int> neighboursTileBitsToAddFromNeighbours = new Dictionary<Vector3Int, int>()
                    {
                        {  new Vector3Int(neighbour.Key.x-1, neighbour.Key.y+1, 0), 0b10000000 }, //UpLeft    
                        {  new Vector3Int(neighbour.Key.x,   neighbour.Key.y+1, 0), 0b11100000 }, //Up        
                        {  new Vector3Int(neighbour.Key.x+1, neighbour.Key.y+1, 0), 0b00100000 }, //UpRight   
                        {  new Vector3Int(neighbour.Key.x+1, neighbour.Key.y,   0), 0b00111000 }, //Right    
                        {  new Vector3Int(neighbour.Key.x+1, neighbour.Key.y-1, 0), 0b00001000 }, //DownRight
                        {  new Vector3Int(neighbour.Key.x,   neighbour.Key.y-1, 0), 0b00001110 }, //Down      
                        {  new Vector3Int(neighbour.Key.x-1, neighbour.Key.y-1, 0), 0b00000010 }, //DownLeft 
                        {  new Vector3Int(neighbour.Key.x-1, neighbour.Key.y,   0), 0b10000011 }  //Left      
                    };

                    int neighbourTileResult = 0b00000000;
                    foreach (KeyValuePair<Vector3Int, int> neighboursNeighbour in neighboursTileBitsToAddFromNeighbours)
                    {
                        if (LandTileExists(0, neighboursNeighbour.Key))
                            neighbourTileResult = neighbourTileResult | neighboursNeighbour.Value;
                    }
                    {
                        if (sandWaterCodeToTile.TryGetValue(neighbourTileResult, out Tile tile))
                            sandWaterTileMap.SetTile(neighbour.Key, tile);
                        else
                            Debug.Log("Tile representation not found for tile code: " + neighbourTileResult);
                    }
                }
                //Removed sand
                {
                    ParticlesManager.Instance.PlaySmokeEffect(mouseTilePosition);
                    TileInformationManager.Instance.GetTileInformation(mouseTilePosition).layerNum = GetHighestGrassLandLayer(mouseTilePosition);
                }
            }


            Skip:
            yield return 0;
        }
        coroutineRunning = false;
    }

    IEnumerator PlaceLand(int layerNumber)
    {
        coroutineRunning = true;
        indicatorRenderer.sprite = null;
        Vector3Int previousTilePosition = new Vector3Int(-1, -1, -1);

        TileMapLayer tileMapLayer;
        {
            //Create or get new land tilemap layer
            if (tilemapLayers_.Count <= layerNumber-1)
            {
                var newTileMap = new TileMapLayer(layerNumber);
                tilemapLayers_.Add(newTileMap);
                tileMapLayer = newTileMap;
            }
            else
                tileMapLayer = GetTilemapLayer(layerNumber);
        }

        while (Input.GetButton("Primary"))
        {
            Vector3Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();

            TileBase mouseGrassTile = tileMapLayer.grassTileMap.GetTile(mouseTilePosition);
            TileBase mouseCliffTile = tileMapLayer.cliffTileMap.GetTile(mouseTilePosition);

            {
                if (mouseTilePosition != previousTilePosition)
                    previousTilePosition = mouseTilePosition;
                else
                    goto Skip;

                if (TerrainPlaceable(mouseTilePosition, out int ln))
                {
                    if (layerNumber != ln)
                        goto Skip;
                }
                else
                    goto Skip;
            }

            Vector3Int left = new Vector3Int(mouseTilePosition.x - 1, mouseTilePosition.y, 0);
            Vector3Int right = new Vector3Int(mouseTilePosition.x + 1, mouseTilePosition.y, 0);
            Vector3Int above = new Vector3Int(mouseTilePosition.x, mouseTilePosition.y + 1, 0);

            Vector3Int cliffLeft = left;
            Vector3Int cliffRight = right;
            Vector3Int grassLeft = new Vector3Int(mouseTilePosition.x - 1, mouseTilePosition.y + 1, 0);
            Vector3Int grassRight = new Vector3Int(mouseTilePosition.x + 1, mouseTilePosition.y + 1, 0);
            Vector3Int grassUp = new Vector3Int(mouseTilePosition.x, mouseTilePosition.y + 2, 0);

            //For setting sprite of cliff at mouse position
            {
                //Case when no tile existed at mouse
                if (mouseGrassTile == null && mouseCliffTile == null)
                {
                    Dictionary<Vector3Int, int> checkNeighboursForMouseCliff = new Dictionary<Vector3Int, int>()
                    {
                        { left,  0b1100}, //CliffLeft
                        { right, 0b1001}, //CliffRight
                    };
                    int resultCode = 0b1000;
                    foreach (KeyValuePair<Vector3Int, int> neighbour in checkNeighboursForMouseCliff)
                    {
                        if (LandTileExists(layerNumber, neighbour.Key))
                            resultCode = resultCode | neighbour.Value;
                    }
                    SetLandTile(tileMapLayer.cliffTileMap, mouseTilePosition, resultCode);
                    TileInformationManager.Instance.GetTileInformation(mouseTilePosition).layerNum = Constants.INVALID_TILE_LAYER;
                }
                //Case when mouse tile is grass, only change the sprite to connect to above tile
                else if (tileMapLayer.grassTileMap.GetTile(mouseTilePosition) != null)
                {
                    if (landTileNameToCode.TryGetValue(mouseGrassTile.name, out int code))
                        SetLandTile(tileMapLayer.grassTileMap, mouseTilePosition, code | 0b0010);

                    else
                        Debug.Log("No code associated with tile: " + mouseGrassTile.name);
                }
            }
            //For setting sprite of grass at above mouse position
            {
                Dictionary<Vector3Int, int> checkNeighboursForMouseGrass = new Dictionary<Vector3Int, int>()
                    {
                        { left,  0b0100}, //GrassLeft
                        { right, 0b0001}, //GrassRight
                        { above, 0b0010}, //GrassUp
                    };
                int resultCode = 0b0000;
                foreach (KeyValuePair<Vector3Int, int> neighbour in checkNeighboursForMouseGrass)
                {
                    if (LandTileExists(layerNumber, neighbour.Key))
                    {
                        resultCode = resultCode | neighbour.Value;
                    }
                }
                SetLandTile(tileMapLayer.grassTileMap, above, resultCode);
                SetLandTile(tileMapLayer.cliffTileMap, above, Constants.EMPTY_TILECODE); //Remove cliff if there was any
                TileInformationManager.Instance.GetTileInformation(above).layerNum = GetHighestGrassLandLayer(above);
            }
            //Change neighbour sprites
            {
                Dictionary<Vector3Int, int> neighbours = new Dictionary<Vector3Int, int>()
                    {
                        { cliffLeft,  0b1001 }, //CliffLeft
                        { cliffRight, 0b1100 }, //CliffRight
                        { grassLeft,  0b0001 }, //GrassLeft
                        { grassRight, 0b0100 }, //Grass right    
                        { grassUp,    0b0000 }  //Grass up
                    };

                //Set neighbour tiles
                foreach (KeyValuePair<Vector3Int, int> neighbour in neighbours)
                {
                    Tilemap neighbourTileMap;
                    TileBase neighbourTile;
                    if (tileMapLayer.grassTileMap.GetTile(neighbour.Key) != null)
                        neighbourTileMap = tileMapLayer.grassTileMap;

                    else if (tileMapLayer.cliffTileMap.GetTile(neighbour.Key) != null)
                        neighbourTileMap = tileMapLayer.cliffTileMap;

                    else
                        continue;

                    neighbourTile = neighbourTileMap.GetTile(neighbour.Key);

                    if (landTileNameToCode.TryGetValue(neighbourTile.name, out int code))
                    {

                        if (!(GetLandTileType(code) == GetLandTileType(neighbour.Value)))
                            continue;

                        code = code | neighbour.Value;
                        SetLandTile(neighbourTileMap, neighbour.Key, code);
                    }
                    else
                    {
                        Debug.Log("No code associated with tile name: " + neighbourTile.name);
                    }
                }
                {
                    ParticlesManager.Instance.PlaySmokeEffect(mouseTilePosition);
                }
                //Set new layers
                {
                    TileInformationManager.Instance.GetTileInformation(above).layerNum = GetHighestGrassLandLayer(above);
                    TileInformationManager.Instance.GetTileInformation(mouseTilePosition).layerNum = GetHighestGrassLandLayer(mouseTilePosition);
                }
            }

            Skip:
            yield return 0;
        }
        coroutineRunning = false;
    }

    IEnumerator RemoveLand(int layerNumber)
    {
        coroutineRunning = true;
        indicatorRenderer.sprite = null;
        Vector3Int previousTilePosition = new Vector3Int(-1, -1, -1);

        TileMapLayer tileMapLayer = GetTilemapLayer(layerNumber);


        while (Input.GetButton("Secondary"))
        {
            Vector3Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();
            Vector3Int mouseAbovePosition = new Vector3Int(mouseTilePosition.x, mouseTilePosition.y + 1, 0);

            TileBase mouseGrassTile = tileMapLayer.grassTileMap.GetTile(mouseTilePosition);
            TileBase mouseCliffTile = tileMapLayer.cliffTileMap.GetTile(mouseTilePosition);

            {
                if (mouseTilePosition != previousTilePosition)
                    previousTilePosition = mouseTilePosition;
                else
                    goto Skip;

                if (!TileInformationManager.Instance.PositionInMap(mouseTilePosition))
                    goto Skip;

                //Skip if there is there is no land at mouse
                if (TerrainRemoveable(mouseTilePosition, out int ln))
                {
                    if (ln != layerNumber)
                        goto Skip;
                }
                else
                    goto Skip;
            }
            {
                //Set mouse tile to null or update below grass
                if (!(LandTileExists(layerNumber, new Vector3Int(mouseTilePosition.x, mouseTilePosition.y - 1, 0))))
                {
                    SetLandTile(tileMapLayer.cliffTileMap, mouseTilePosition, Constants.EMPTY_TILECODE);
                }
                else //means there is grass at mouse
                {
                    if (landTileNameToCode.TryGetValue(mouseGrassTile.name, out int code))
                        SetLandTile(tileMapLayer.grassTileMap, mouseTilePosition, code ^ 0b0010);

                    else
                        Debug.Log("No code associated with tile name: " + mouseGrassTile.name);
                }


                //Set above (grass) tile to cliff or null
                if (!(LandTileExists(layerNumber, new Vector3Int(mouseTilePosition.x, mouseTilePosition.y + 1, 0))))
                {
                    SetLandTile(tileMapLayer.grassTileMap, mouseAbovePosition, Constants.EMPTY_TILECODE);
                }
                else
                {
                    Dictionary<Vector3Int, int> bitsToAddFromNeighbour = new Dictionary<Vector3Int, int>()
                    {
                        { new Vector3Int(mouseTilePosition.x - 1, mouseTilePosition.y+1, 0), 0b1100}, //GrassLeft
                        { new Vector3Int(mouseTilePosition.x + 1, mouseTilePosition.y+1, 0), 0b1001}, //GrassRight
                    };
                    int resultCode = 0b1000;
                    foreach (KeyValuePair<Vector3Int, int> neighbour in bitsToAddFromNeighbour)
                    {
                        if (LandTileExists(layerNumber, neighbour.Key))
                            resultCode = resultCode | neighbour.Value;
                    }
                    SetLandTile(tileMapLayer.cliffTileMap, mouseAbovePosition, resultCode);
                    SetLandTile(tileMapLayer.grassTileMap, mouseAbovePosition, Constants.EMPTY_TILECODE);  //Set grass tile to null
                }


                //Set cliff neighbours
                {
                    Dictionary<Vector3Int, int> subtractBitsFromTiles = new Dictionary<Vector3Int, int>()
                    {
                        { new Vector3Int(mouseTilePosition.x - 1, mouseTilePosition.y,     0), 0b0001}, //CliffLeft
                        { new Vector3Int(mouseTilePosition.x + 1, mouseTilePosition.y,     0), 0b0100}  //CliffRight
                    };

                    foreach (KeyValuePair<Vector3Int, int> neighbour in subtractBitsFromTiles)
                    {
                        TileBase tile = tileMapLayer.cliffTileMap.GetTile(neighbour.Key);
                        if (tile == null)
                            continue;

                        if (landTileNameToCode.TryGetValue(tile.name, out int code))
                            SetLandTile(tileMapLayer.cliffTileMap, neighbour.Key, code ^ neighbour.Value);

                        else
                            Debug.Log("No code associated with tile name: " + tile.name);
                    }
                }


                //Set grass neighbours
                {
                    Dictionary<Vector3Int, int> subtractBitsFromTiles = new Dictionary<Vector3Int, int>()
                    {
                        { new Vector3Int(mouseTilePosition.x - 1, mouseTilePosition.y + 1, 0), 0b0001}, //GrassLeft
                        { new Vector3Int(mouseTilePosition.x + 1, mouseTilePosition.y + 1, 0), 0b0100}, //GrassRight
                    };

                    foreach (KeyValuePair<Vector3Int, int> neighbour in subtractBitsFromTiles)
                    {
                        TileBase tile = tileMapLayer.grassTileMap.GetTile(neighbour.Key);

                        if (tile == null)
                            continue;

                        if (landTileNameToCode.TryGetValue(tile.name, out int code))
                            SetLandTile(tileMapLayer.grassTileMap, neighbour.Key, code ^ neighbour.Value);

                        else
                            Debug.Log("No code associated with tile name: " + tile.name);
                    }
                }

                //Play animation
                {
                    ParticlesManager.Instance.PlaySmokeEffect(mouseTilePosition);
                }
                //Set new layers
                {
                    TileInformationManager.Instance.GetTileInformation(mouseAbovePosition).layerNum = GetHighestGrassLandLayer(mouseAbovePosition);
                    TileInformationManager.Instance.GetTileInformation(mouseTilePosition).layerNum = GetHighestGrassLandLayer(mouseTilePosition);
                }
            }
            Skip:
            yield return 0;
        }
        coroutineRunning = false;
    }

    #region Helpers
    //Layer number 0 = sand      >1 = cliffsAndGrass
    public bool LandTileExists(int layerNumber, Vector3Int position)
    {
        if (layerNumber == 0)
        {
            TileBase sandAndWaterMouseTile = sandWaterTileMap.GetTile(position);

            if (sandAndWaterMouseTile == null)
                return false;

            bool sandExists = false;
            foreach (TileBase sandTile in fullSandTiles)
            {
                if (sandTile.name == sandAndWaterMouseTile.name)
                {
                    sandExists = true;
                    break;
                }
            }
            return sandExists;
        }

        else if (tilemapLayers_.Count < layerNumber)
        {
            return false;
        }
        return GetTilemapLayer(layerNumber).grassTileMap.GetTile(new Vector3Int(position.x, position.y + 1, 0)) != null;
    }

    private int GetLandTileType(int code)
    {
        return ((code >> (4 - 1)) & 1);
    }

    private int GetHighestGrassLandLayer(Vector3Int position)
    {
        for (int i = tilemapLayers_.Count; i >= 1; i--)
        {
            if (GetTilemapLayer(i).grassTileMap.GetTile(position) != null)
                return i;
            if (GetTilemapLayer(i).cliffTileMap.GetTile(position) != null)
                return Constants.INVALID_TILE_LAYER;
        }
        //Either water or sand will return 0
        return 0;
    }

    private void SetLandTile(Tilemap landTileMap, Vector3Int position, int code)
    {
        if (code == Constants.EMPTY_TILECODE)
            landTileMap.SetTile(position, null);
        else if (landCodeToTile.TryGetValue(code, out Tile tile))
            landTileMap.SetTile(position, tile);
        else
            Debug.Log("No tile associated with code: " + code);
    }

    private TileMapLayer GetTilemapLayer(int layerNum)
    {
        if (tilemapLayers_.Count < layerNum)
        {
            return null;
        }
        return tilemapLayers_[layerNum - 1];
    }

    //Returns 0 if sand is removeable 1> for land is removeable
    private bool TerrainRemoveable(Vector3Int position, out int layerNumber)
    {
        //No sand means no land
        if (!LandTileExists(0, position))
        {
            layerNumber = Constants.INVALID_TILE_LAYER;
            return false;
        }
        else
        {
            //Look for land and return layer number of land
            for (int i = tilemapLayers_.Count; i >= 1; i--)
            {
                //Find highest layer with land
                if (LandTileExists(i, position))
                {
                    //Check if tilemap above has land, exit if it does
                    if ((LandTileExists(i + 1, new Vector3Int(position.x, position.y, 0))) ||
                       (LandTileExists(i + 1, new Vector3Int(position.x, position.y + 1, 0))) ||
                       (LandTileExists(i + 1, new Vector3Int(position.x, position.y + 2, 0))) ||
                       (LandTileExists(i + 1, new Vector3Int(position.x - 1, position.y + 1, 0))) ||
                       (LandTileExists(i + 1, new Vector3Int(position.x + 1, position.y + 1, 0))))
                    {
                        layerNumber = Constants.INVALID_TILE_LAYER;
                        return false;
                    }

                    if (TileInformationManager.Instance.GetTileInformation(new Vector3Int(position.x, position.y + 1, 0)).TileIsEmpty() || TileInformationManager.Instance.GetTileInformation(new Vector3Int(position.x, position.y + 1, 0)).layerNum != i + 1)
                    {
                        layerNumber = i;
                        return true;
                    }
                    else
                    {
                        layerNumber = Constants.INVALID_TILE_LAYER;
                        return false;
                    }
                }
            }

            //If no land found, it means there is only sand
            if (TileInformationManager.Instance.GetTileInformation(position).TileIsEmpty() || TileInformationManager.Instance.GetTileInformation(new Vector3Int(position.x, position.y, 0)).layerNum != 0)
            {
                layerNumber = 0;
                return true;
            }
            else
            {
                layerNumber = Constants.INVALID_TILE_LAYER;
                return false;
            }
        }
    }

    //Returns 0 if sand is placeable 1> for land is placeable
    private bool TerrainPlaceable(Vector3Int position, out int layerNumber)
    {
        //Check for objects
        TileInformation tile = TileInformationManager.Instance.GetTileInformation(position);
        TileInformation aboveTile = TileInformationManager.Instance.GetTileInformation(new Vector3Int(position.x, position.y + 1, 0));
        if (tile == null || aboveTile == null)
        {
            layerNumber = Constants.INVALID_TILE_LAYER;
            return false;
        }

        //Sand is placeable
        if (!LandTileExists(0, position))
        {
            if (!tile.TileIsEmpty())
            {
                layerNumber = Constants.INVALID_TILE_LAYER;
                return false;
            }
            layerNumber = 0;
            return true;
        }

        for (int i = tilemapLayers_.Count; i >= 0; i--)
        {
            if (LandTileExists(i+1, position)) //If we find land on mouse, we should stop function because we know we cant place below either
            {
                layerNumber = Constants.INVALID_TILE_LAYER;
                return false;
            }

            //If layer is below land
            if (i > 0)
            {
                //Check if below tiles are grass
                Tilemap belowTileMap = GetTilemapLayer(i).grassTileMap;
                TileBase belowTile = belowTileMap.GetTile(position);
                TileBase belowLeftTile = belowTileMap.GetTile(new Vector3Int(position.x - 1, position.y, 0));
                TileBase belowRightTile = belowTileMap.GetTile(new Vector3Int(position.x + 1, position.y, 0));
                TileBase belowDownTile = belowTileMap.GetTile(new Vector3Int(position.x, position.y - 1, 0));
                TileBase belowUpTile = belowTileMap.GetTile(new Vector3Int(position.x, position.y + 1, 0));
                if (!(belowTile != null &&
                    belowLeftTile != null &&
                    belowRightTile != null &&
                    belowDownTile != null &&
                    belowUpTile != null))
                {
                    continue;
                }
            }

            if (tile.layerNum == i && !tile.TileIsEmpty())
                continue;

            if (!aboveTile.TileIsEmpty())
                continue;

            layerNumber = i + 1;
            return true;
        }

        layerNumber = Constants.INVALID_TILE_LAYER;
        return false;
    }
    #endregion
}
