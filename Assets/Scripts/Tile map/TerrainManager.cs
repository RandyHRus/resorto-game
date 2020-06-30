using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainManager : MonoBehaviour
{
    [SerializeField] private GameObject tileGrid = null;
    [SerializeField] private Tilemap sandTilemap = null;
    [SerializeField] private Tilemap landTileMapPrefab = null;
    [SerializeField] private Tilemap waterBGTilemap = null;

    private List<Tilemap> tilemapLayers_ = new List<Tilemap>(); //Starts at 0, which is the first layer of land

    private class TilemapInformation {
        public Dictionary<string, int> tileNameToCode = new Dictionary<string, int>();
        public Dictionary<int, TileBase> codeToTile = new Dictionary<int, TileBase>();
        
        public TilemapInformation(TileToCode[] tileToCodeList)
        {
            foreach (TileToCode tileCode in tileToCodeList)
            {
                if (tileCode.tile == null)
                {
                    tileNameToCode.Add("", System.Convert.ToInt32(tileCode.code, 2));
                    codeToTile.Add(System.Convert.ToInt32(tileCode.code, 2), null);
                }
                else
                {
                    tileNameToCode.Add(tileCode.tile.name, System.Convert.ToInt32(tileCode.code, 2));
                    codeToTile.Add(System.Convert.ToInt32(tileCode.code, 2), tileCode.tile);
                }
            }
        }
    }

    [System.Serializable]
    public struct TileToCode
    {
        public string code;
        public TileBase tile;
    }

    #region Terrain Tile Codes
    /*
     *  string represents sides and corners of sprite
     *  _______
     *  |1 2 3|
     *  |8 0 4|   - Numbers are indexes of string
     *  |7_6_5|   - 0 for false, 1 for true
     *            - Example code "01001000"
    */
    [SerializeField] private TileToCode[] sandWaterTileCodes = null;
    private TilemapInformation sandWaterTilemapInfo;

    /*
     *  string represents sides and corners of sprite
     *  _______
     *  |1   2|
     *  |     |   - 1 = grass
     *  |4___3|   - 0 = cliff
    */
    [SerializeField] private TileToCode[] landTileCodes = null;
    private TilemapInformation landTilemapInfo;

    /*
    *  water back ground tiles
    *  _______
    *  |1   2|
    *  |     |
    *  |4___3|
   */
    [SerializeField] private TileToCode[] waterBGTileCodes = null;
    private TilemapInformation waterBGTilemapInfo;


    #endregion

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
        //Initialize Tilemap informations
        {
            sandWaterTilemapInfo = new TilemapInformation(sandWaterTileCodes);
            landTilemapInfo      = new TilemapInformation(landTileCodes);
            waterBGTilemapInfo   = new TilemapInformation(waterBGTileCodes);
        }
    }

    public bool TryCreateSand(Vector3Int position)
    {
        if (TerrainPlaceable(position, out int ln))
        {
            if (ln != 0)
                return false;
        }
        else
            return false;

        //Sand
        {
            //Self tile
            {
                AddOrRemoveTileCode(position, 0b111111111, sandWaterTilemapInfo, sandTilemap, true, out int newCode);
                TileInformationManager.Instance.GetTileInformation(position).tileLocation = TileLocation.Sand;
            }

            //Neighbours
            Dictionary<Vector3Int, int> codesToAddToNeighbours = new Dictionary<Vector3Int, int>()
                {
                    {  new Vector3Int(position.x-1, position.y+1, 0), 0b000001000 }, //UpLeft    
                    {  new Vector3Int(position.x,   position.y+1, 0), 0b000001110 }, //Up        
                    {  new Vector3Int(position.x+1, position.y+1, 0), 0b000000010 }, //UpRight   
                    {  new Vector3Int(position.x+1, position.y,   0), 0b010000011 }, //Right    
                    {  new Vector3Int(position.x+1, position.y-1, 0), 0b010000000 }, //DownRight
                    {  new Vector3Int(position.x,   position.y-1, 0), 0b011100000 }, //Down      
                    {  new Vector3Int(position.x-1, position.y-1, 0), 0b000100000 }, //DownLeft 
                    {  new Vector3Int(position.x-1, position.y,   0), 0b000111000 }  //Left      
                };
            foreach (KeyValuePair<Vector3Int, int> neighbour in codesToAddToNeighbours)
            {
                AddOrRemoveTileCode(neighbour.Key, neighbour.Value, sandWaterTilemapInfo, sandTilemap, true, out int newCode);
                TileInformation neighbourInfo = TileInformationManager.Instance.GetTileInformation(neighbour.Key);
                if (TileLocationManager.isWater.HasFlag(neighbourInfo.tileLocation))
                    neighbourInfo.tileLocation = TileLocation.WaterEdge;
            }
        }
        //Water background
        {
            HashSet<Vector3Int> visitedPositions = new HashSet<Vector3Int>();

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    Vector3Int neighbourPos = new Vector3Int(position.x + i, position.y + j, 0);
                    Dictionary<Vector3Int, int> neigbourOfNeighbours = new Dictionary<Vector3Int, int>()
                        {
                            {  new Vector3Int(neighbourPos.x-1, neighbourPos.y+1, 0), 0b0010 }, //UpLeft    
                            {  new Vector3Int(neighbourPos.x,   neighbourPos.y+1, 0), 0b0011 }, //Up        
                            {  new Vector3Int(neighbourPos.x+1, neighbourPos.y+1, 0), 0b0001 }, //UpRight   
                            {  new Vector3Int(neighbourPos.x+1, neighbourPos.y,   0), 0b1001 }, //Right    
                            {  new Vector3Int(neighbourPos.x+1, neighbourPos.y-1, 0), 0b1000 }, //DownRight
                            {  new Vector3Int(neighbourPos.x,   neighbourPos.y-1, 0), 0b1100 }, //Down      
                            {  new Vector3Int(neighbourPos.x-1, neighbourPos.y-1, 0), 0b0100 }, //DownLeft 
                            {  new Vector3Int(neighbourPos.x-1, neighbourPos.y,   0), 0b0110 }  //Left      
                        };

                    AddCodeToWaterBackgroundTile(neighbourPos, 0b1111);
                    foreach (KeyValuePair<Vector3Int, int> neighbourOfNeighbour in neigbourOfNeighbours)
                    {
                        AddCodeToWaterBackgroundTile(neighbourOfNeighbour.Key, neighbourOfNeighbour.Value);
                    }
                }
            }
        }
        return true;

        void AddCodeToWaterBackgroundTile(Vector3Int pos, int code)
        {
            if (AddOrRemoveTileCode(pos, code, waterBGTilemapInfo, waterBGTilemap, true, out int newCode))
            {
                int[] tileTracker = TileInformationManager.Instance.GetTileInformation(pos).waterBGTracker;
                for (int i = 0; i < 4; i++)
                    tileTracker[i] += (code >> i & 1); //Add 1 or 0
            }
        }
    }

    public bool TryRemoveSand(Vector3Int position)
    {
        if (TerrainRemoveable(position, out int ln))
        {
            if (ln != 0)
                return false;
        }
        else
            return false;

        Dictionary<Vector3Int, int> mouseTileBitsToAddFromNeighbours = new Dictionary<Vector3Int, int>()
            {
                {  new Vector3Int(position.x-1, position.y+1, 0), 0b010000000 }, //UpLeft    
                {  new Vector3Int(position.x,   position.y+1, 0), 0b011100000 }, //Up        
                {  new Vector3Int(position.x+1, position.y+1, 0), 0b000100000 }, //UpRight   
                {  new Vector3Int(position.x+1, position.y,   0), 0b000111000 }, //Right    
                {  new Vector3Int(position.x+1, position.y-1, 0), 0b000001000 }, //DownRight
                {  new Vector3Int(position.x,   position.y-1, 0), 0b000001110 }, //Down      
                {  new Vector3Int(position.x-1, position.y-1, 0), 0b000000010 }, //DownLeft 
                {  new Vector3Int(position.x-1, position.y,   0), 0b010000011 }  //Left      
            };

        //Change self tile
        {
            int newTileCode = 0b000000000;
            foreach (KeyValuePair<Vector3Int, int> neighbour in mouseTileBitsToAddFromNeighbours)
            {
                if (TileInformationManager.Instance.GetTileInformation(neighbour.Key).tileLocation != TileLocation.WaterEdge)
                {
                    newTileCode = newTileCode | neighbour.Value;
                }
            }
            SetTileCode(position, newTileCode, sandWaterTilemapInfo, sandTilemap);
            if (sandTilemap.GetTile(position) == null)
                TileInformationManager.Instance.GetTileInformation(position).tileLocation = TileLocation.DeepWater;
            else
                TileInformationManager.Instance.GetTileInformation(position).tileLocation = TileLocation.WaterEdge;
        }
        //Change neighbours tile
        {
            foreach (KeyValuePair<Vector3Int, int> neighbour in mouseTileBitsToAddFromNeighbours)
            {
                //If neighbour is not water, we don't need to change it.
                if (TileInformationManager.Instance.GetTileInformation(neighbour.Key).tileLocation != TileLocation.WaterEdge)
                {
                    continue;
                }

                Dictionary<Vector3Int, int> neighboursTileBitsToAddFromNeighbours = new Dictionary<Vector3Int, int>()
                    {
                        {  new Vector3Int(neighbour.Key.x-1, neighbour.Key.y+1, 0), 0b010000000 }, //UpLeft    
                        {  new Vector3Int(neighbour.Key.x,   neighbour.Key.y+1, 0), 0b011100000 }, //Up        
                        {  new Vector3Int(neighbour.Key.x+1, neighbour.Key.y+1, 0), 0b000100000 }, //UpRight   
                        {  new Vector3Int(neighbour.Key.x+1, neighbour.Key.y,   0), 0b000111000 }, //Right    
                        {  new Vector3Int(neighbour.Key.x+1, neighbour.Key.y-1, 0), 0b000001000 }, //DownRight
                        {  new Vector3Int(neighbour.Key.x,   neighbour.Key.y-1, 0), 0b000001110 }, //Down      
                        {  new Vector3Int(neighbour.Key.x-1, neighbour.Key.y-1, 0), 0b000000010 }, //DownLeft 
                        {  new Vector3Int(neighbour.Key.x-1, neighbour.Key.y,   0), 0b010000011 }  //Left      
                    };

                int neighbourTileCode = 0b000000000;
                foreach (KeyValuePair<Vector3Int, int> neighboursNeighbour in neighboursTileBitsToAddFromNeighbours)
                {
                    TileLocation tileLocation = TileInformationManager.Instance.GetTileInformation(neighboursNeighbour.Key).tileLocation;
                    if (!TileLocationManager.isWater.HasFlag(tileLocation))
                    {
                        neighbourTileCode = neighbourTileCode | neighboursNeighbour.Value;
                    }
                }
                SetTileCode(neighbour.Key, neighbourTileCode, sandWaterTilemapInfo, sandTilemap);
            }
        }
        //Water background
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    Vector3Int neighbourPos = new Vector3Int(position.x + i, position.y + j, 0);
                    Dictionary<Vector3Int, int> neigbourOfNeighbours = new Dictionary<Vector3Int, int>()
                        {
                            {  new Vector3Int(neighbourPos.x-1, neighbourPos.y+1, 0), 0b0010 }, //UpLeft    
                            {  new Vector3Int(neighbourPos.x,   neighbourPos.y+1, 0), 0b0011 }, //Up        
                            {  new Vector3Int(neighbourPos.x+1, neighbourPos.y+1, 0), 0b0001 }, //UpRight   
                            {  new Vector3Int(neighbourPos.x+1, neighbourPos.y,   0), 0b1001 }, //Right    
                            {  new Vector3Int(neighbourPos.x+1, neighbourPos.y-1, 0), 0b1000 }, //DownRight
                            {  new Vector3Int(neighbourPos.x,   neighbourPos.y-1, 0), 0b1100 }, //Down      
                            {  new Vector3Int(neighbourPos.x-1, neighbourPos.y-1, 0), 0b0100 }, //DownLeft 
                            {  new Vector3Int(neighbourPos.x-1, neighbourPos.y,   0), 0b0110 }  //Left      
                        };

                    RemoveCodeFromWaterBackgroundTile(neighbourPos, 0b1111);
                    foreach (KeyValuePair<Vector3Int, int> neighbourOfNeighbour in neigbourOfNeighbours)
                    {
                        RemoveCodeFromWaterBackgroundTile(neighbourOfNeighbour.Key, neighbourOfNeighbour.Value);
                    }

                }
            }
        }

        return true;

        void RemoveCodeFromWaterBackgroundTile(Vector3Int toRemovePosition, int code)
        {
            if (!TileInformationManager.Instance.PositionInMap(toRemovePosition))
                return;

            int[] tileTracker = TileInformationManager.Instance.GetTileInformation(toRemovePosition).waterBGTracker;

            string bitsToKeepBinaryString = "";
            for (int i = 3; i >= 0; i--)
            {
                tileTracker[i] -= (code >> i & 1); //Add 1 or 0

                if (tileTracker[i] <= 0)
                {
                    tileTracker[i] = 0;
                    bitsToKeepBinaryString += "0";
                }
                else
                    bitsToKeepBinaryString += "1";
            }

            int bitsToKeep = Convert.ToInt32(bitsToKeepBinaryString, 2);
            AddOrRemoveTileCode(toRemovePosition, bitsToKeep, waterBGTilemapInfo, waterBGTilemap, false, out int newCode);
        }
    }

    public bool TryCreateLand(Vector3Int position, int layerNumber) {

        if (TerrainPlaceable(position, out int ln))
        {
            if (layerNumber != ln)
                return false;
        }
        else
            return false;

        Tilemap tilemapLayer;
        {
            //Create or get new land tilemap layer
            if (tilemapLayers_.Count <= layerNumber - 1)
            {
                var newTileMap = Instantiate(landTileMapPrefab);
                newTileMap.transform.SetParent(tileGrid.transform);
                tilemapLayers_.Add(newTileMap);
                tilemapLayer = newTileMap;
            }
            else
                tilemapLayer = GetTilemapLayer(layerNumber);
        }

        Dictionary<Vector3Int, int> codesToAddToTiles = new Dictionary<Vector3Int, int>()
        {
            { position,                                            0b1100 },
            { new Vector3Int(position.x - 1, position.y, 0),       0b0100 },
            { new Vector3Int(position.x + 1, position.y, 0),       0b1000 },
            { new Vector3Int(position.x, position.y + 1, 0),       0b0011 },
            { new Vector3Int(position.x - 1, position.y + 1, 0),   0b0010 },
            { new Vector3Int(position.x + 1, position.y + 1, 0),   0b0001 },
        };

        foreach(KeyValuePair<Vector3Int, int> pair in codesToAddToTiles)
        {
            bool existingTileIsGrass = TileIsGrass(tilemapLayer.GetTile(pair.Key), tilemapLayer);

            if (AddOrRemoveTileCode(pair.Key, pair.Value, landTilemapInfo, tilemapLayer, true, out int newCode))
            {
                //Set new layers
                TileInformation tile = TileInformationManager.Instance.GetTileInformation(pair.Key);
                bool newTileIsGrass = TileIsGrass(tilemapLayer.GetTile(pair.Key), tilemapLayer);

                if (newTileIsGrass && !existingTileIsGrass)
                    tile.layerNum++;

                if (newTileIsGrass) //Full grass
                {
                    tile.tileLocation = TileLocation.Land;
                }
                else
                {
                    if ((newCode & 0b1100) == 0b1100 || newCode == 0b0100 || newCode == 0b1000)
                        tile.tileLocation = TileLocation.CliffFront;
                    else
                        tile.tileLocation = TileLocation.CliffBack;
                }
            }
        }
        return true;
    }

    public bool TryRemoveLand(Vector3Int position, int layerNumber)
    {
        //Skip if there is there is no land at position
        {
            if (TerrainRemoveable(position, out int ln))
            {
                if (ln != layerNumber)
                    return false;
            }
            else
                return false;
        }

        Tilemap tilemapLayer = GetTilemapLayer(layerNumber);

        Dictionary<Vector3Int, int> bitsToRemoveFromTiles = new Dictionary<Vector3Int, int>()
        {
            { position,                                           0b0011 },
            { new Vector3Int(position.x - 1, position.y, 0),      0b1011 },
            { new Vector3Int(position.x + 1, position.y, 0),      0b0111 },
            { new Vector3Int(position.x, position.y + 1, 0),      0b1100 },
            { new Vector3Int(position.x - 1, position.y + 1, 0),  0b1101 },
            { new Vector3Int(position.x + 1, position.y + 1, 0),  0b1110 },
        };

        foreach (KeyValuePair<Vector3Int, int> pair in bitsToRemoveFromTiles)
        {
            bool existingTileIsGrass = TileIsGrass(tilemapLayer.GetTile(pair.Key), tilemapLayer);

            if (AddOrRemoveTileCode(pair.Key, pair.Value, landTilemapInfo, tilemapLayer, false, out int newCode))
            {
                //Set new layers
                TileInformation tile = TileInformationManager.Instance.GetTileInformation(pair.Key);
                bool newTileIsGrass = TileIsGrass(tilemapLayer.GetTile(pair.Key), tilemapLayer);

                if (!newTileIsGrass && existingTileIsGrass)
                    tile.layerNum--;

                TileBase newTile = tilemapLayer.GetTile(pair.Key);
                //bool newTileIsGrass = TileIsGrass(newTile, tilemapLayer);

                if (newTile == null)
                {
                    if (tile.layerNum == 0)
                    {
                        if (!TileLocationManager.isWater.HasFlag(tile.tileLocation))
                            tile.tileLocation = TileLocation.Sand;
                    }
                    else
                    {
                        tile.tileLocation = TileLocation.Land;
                    }
                }
                else if (newTileIsGrass)
                {
                    tile.tileLocation = TileLocation.Land;
                }
                else
                {
                    if ((newCode & 0b1100) == 0b1100 || newCode == 0b0100 || newCode == 0b1000)
                        tile.tileLocation = TileLocation.CliffFront;
                    else
                        tile.tileLocation = TileLocation.CliffBack;
                }
            }

        }
        return true;
    }

    private bool TileIsGrass(TileBase tile, Tilemap tilemap)
    {
        if (tile == null)
            return false;

        if (landTilemapInfo.tileNameToCode.TryGetValue(tile.name, out int code))
            return (code == 0b1111);
        else
            return false;
    }

    private Tilemap GetTilemapLayer(int layerNum)
    {
        if (tilemapLayers_.Count < layerNum)
        {
            return null;
        }
        return tilemapLayers_[layerNum - 1];
    }

    //Returns 0 if sand is removeable 1> for land is removeable
    public bool TerrainRemoveable(Vector3Int position, out int layerNumber)
    {
        Vector3Int aboveTilePosition = new Vector3Int(position.x, position.y + 1, 0);
        if (!TileInformationManager.Instance.PositionInMap(position) ||
            !TileInformationManager.Instance.PositionInMap(aboveTilePosition))
        {
            layerNumber = Constants.INVALID_TILE_LAYER;
            return false;
        }

        TileInformation mainTile = TileInformationManager.Instance.GetTileInformation(position);
        TileLocation mainTileLocation = mainTile.tileLocation;
        TileInformation aboveTileInformation = TileInformationManager.Instance.GetTileInformation(aboveTilePosition);

        if (mainTileLocation == TileLocation.Sand)
        {
            //If tile is sand, simple return
            layerNumber = 0;
            return true;
        }
        else
        {
            int proposedLayerNumber;
            if (mainTile.tileLocation == TileLocation.CliffFront)
            {
                proposedLayerNumber = mainTile.layerNum + 1;
            }
            else if (mainTile.tileLocation == TileLocation.CliffBack)
            {
                if (aboveTileInformation.tileLocation == TileLocation.CliffBack)
                    proposedLayerNumber = aboveTileInformation.layerNum + 1;
                else
                {
                    layerNumber = Constants.INVALID_TILE_LAYER;
                    return false;
                }
            }
            //Below
            else if (aboveTileInformation.tileLocation == TileLocation.Land)
            {
                proposedLayerNumber = aboveTileInformation.layerNum;
            }
            else if (aboveTileInformation.tileLocation == TileLocation.CliffBack && aboveTileInformation.layerNum + 1 == mainTile.layerNum)
            {
                proposedLayerNumber = proposedLayerNumber = mainTile.layerNum;
            }
            else
            {
                layerNumber = Constants.INVALID_TILE_LAYER;
                return false;
            }

            //Check layer up
            {
                Tilemap tilemapLayerToCheck = GetTilemapLayer(proposedLayerNumber + 1);
                Vector3Int[] positionsToCheck = new Vector3Int[]
                {
                    new Vector3Int(aboveTilePosition.x,     aboveTilePosition.y, 0),
                    new Vector3Int(aboveTilePosition.x + 1, aboveTilePosition.y, 0),
                    new Vector3Int(aboveTilePosition.x - 1, aboveTilePosition.y, 0),
                    new Vector3Int(position.x,     position.y, 0),
                    new Vector3Int(position.x + 1, position.y, 0),
                    new Vector3Int(position.x - 1, position.y, 0)
                };

                //Check for objects
                foreach (Vector3Int positionToCheck in positionsToCheck)
                {
                    if (!TileInformationManager.Instance.GetTileInformation(positionToCheck).TileIsEmpty())
                    {
                        layerNumber = Constants.INVALID_TILE_LAYER;
                        return false;
                    }
                }

                //Check for land
                if (tilemapLayerToCheck != null)
                {
                    foreach (Vector3Int positionToCheck in positionsToCheck)
                    {
                        if (tilemapLayerToCheck.GetTile(positionToCheck) != null)
                        {
                            layerNumber = Constants.INVALID_TILE_LAYER;
                            return false;
                        }
                    }
                }
            }

            layerNumber = proposedLayerNumber;
            return true;
        }
    }

    //Returns 0 if sand is placeable 1> for land is placeable
    public bool TerrainPlaceable(Vector3Int position, out int layerNumber)
    {
        Vector3Int aboveTilePosition = new Vector3Int(position.x, position.y + 1, 0);
        if (!TileInformationManager.Instance.PositionInMap(position) ||
            !TileInformationManager.Instance.PositionInMap(aboveTilePosition))
        {
            layerNumber = Constants.INVALID_TILE_LAYER;
            return false;
        }

        TileInformation mainTile = TileInformationManager.Instance.GetTileInformation(position);
        TileLocation mainTileLocation = mainTile.tileLocation;
        TileInformation aboveTileInformation = TileInformationManager.Instance.GetTileInformation(aboveTilePosition);

        //Sand is placeable
        if (TileLocationManager.isWater.HasFlag(mainTileLocation))
        {
            //Scan for outside map
            for (int i = -2; i <= 2; i++) {
                for (int j = -2; j <= 2; j++) {
                    if (!TileInformationManager.Instance.PositionInMap(new Vector3Int(position.x + i, position.y + j, 0)))
                    {
                        layerNumber = Constants.INVALID_TILE_LAYER;
                        return false;
                    }
                }
            }
            layerNumber = 0;
            return true;
        }
        //Check if land is placeable
        else
        {
            Vector3Int[] positionsToCheck = new Vector3Int[]
            {
                new Vector3Int(position.x,     position.y, 0),
                new Vector3Int(position.x + 1, position.y, 0),
                new Vector3Int(position.x - 1, position.y, 0),
                new Vector3Int(position.x,     position.y + 1, 0),
                new Vector3Int(position.x + 1, position.y + 1, 0),
                new Vector3Int(position.x - 1, position.y + 1, 0),
            };
            //Check if land already there
            if (mainTile.layerNum != aboveTileInformation.layerNum)
            {
                layerNumber = Constants.INVALID_TILE_LAYER;
                return false;
            }
            //First check easy stuff
            foreach (Vector3Int positionToCheck in positionsToCheck)
            {
                //Check if outside map
                if (!TileInformationManager.Instance.PositionInMap(positionToCheck)) {
                    layerNumber = Constants.INVALID_TILE_LAYER;
                    return false;
                }
                //Check if any of the tiles are water
                TileInformation tile = TileInformationManager.Instance.GetTileInformation(positionToCheck);
                if (TileLocationManager.isWater.HasFlag(tile.tileLocation)) {
                    layerNumber = Constants.INVALID_TILE_LAYER;
                    return false;
                }
            }
            //Check if placeable above
            bool placeableAbove = true;
            foreach (Vector3Int positionToCheck in positionsToCheck) {
                TileInformation tile = TileInformationManager.Instance.GetTileInformation(positionToCheck);
                TileLocation location = tile.tileLocation;

                //Check if on correct location
                if (tile.layerNum < mainTile.layerNum)
                    placeableAbove = false;

                //Check for objects
                if (!tile.TileIsEmpty())
                    placeableAbove = false;
            }
            //Placeable above
            if (placeableAbove) {
                layerNumber = mainTile.layerNum + 1;
                return true;
            }
            //Check if placeable below
            bool placeableBelow = true;
            foreach (Vector3Int positionToCheck in positionsToCheck)
            {
                TileInformation tile = TileInformationManager.Instance.GetTileInformation(positionToCheck);
                TileLocation location = tile.tileLocation;

                //Check if on correct location
                if (tile.layerNum < aboveTileInformation.layerNum)
                    placeableBelow = false;

                //Check for objects (but only if they are on below)
                if (!tile.TileIsEmpty())
                {
                    if (aboveTileInformation.layerNum == tile.layerNum)
                        placeableBelow = false;
                }
            }
            //Placeable below
            if (placeableBelow) {
                layerNumber = aboveTileInformation.layerNum + 1;
                return true;
            }

            //If not placeable above or below
            layerNumber = Constants.INVALID_TILE_LAYER;
            return false;
        }
    }

    // On addMode    - 1 in code is bits to add
    // On removeMode - 1 in code is bits to keep
    private bool AddOrRemoveTileCode(Vector3Int position, int code, TilemapInformation tilemapInfo, Tilemap tilemap, bool addMode, out int newCode)
    {
        newCode = Constants.EMPTY_TILECODE;

        if (!TileInformationManager.Instance.PositionInMap(position))
        {
            return false;
        }

        TileBase existingTile = tilemap.GetTile(position);

        int existingCode;
        if (existingTile == null)
        {
            existingCode = 0;
        }
        else if (tilemapInfo.tileNameToCode.TryGetValue(existingTile.name, out int c))
        {
            existingCode = c;
        }
        else
        {
            Debug.LogError("No code associated with tile: " + existingTile.name);
            return false;
        }

        if (addMode)
            newCode = code | existingCode;
        else
            newCode = code & existingCode;

        if (tilemapInfo.codeToTile.TryGetValue(newCode, out TileBase tile))
        {
            tilemap.SetTile(position, tile);
            return true;
        }
        else
        {
            Debug.LogError("No tile associated with code: " + newCode);
            return false;
        }
    }

    private bool SetTileCode(Vector3Int position, int code, TilemapInformation tilemapInfo, Tilemap tilemap)
    {
        if (!TileInformationManager.Instance.PositionInMap(position))
            return false;

        if (tilemapInfo.codeToTile.TryGetValue(code, out TileBase tile))
        {
            tilemap.SetTile(position, tile);
            return true;
        }
        else
        {
            Debug.Log("No tile associated with code: " + code);
            return false;
        }
    }
}
