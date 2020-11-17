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

    private List<Tilemap> tilemapLayers_ = new List<Tilemap>(32); //Starts at 0, which is the first layer of land

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

    private class TilemapInformationWithLocation: TilemapInformation
    {
        public Dictionary<int, TileLocation> codeToLocation = new Dictionary<int, TileLocation>();

        public TilemapInformationWithLocation(TileToCodeWithLocation[] tileToCodeList): base(tileToCodeList)
        {
            foreach (TileToCodeWithLocation t in tileToCodeList)
            {
                codeToLocation.Add(System.Convert.ToInt32(t.code, 2), t.tileLocation);
            }
        }
    }

    [System.Serializable]
    public class TileToCode
    {
        public string code;
        public TileBase tile;
    }

    [System.Serializable]
    public class TileToCodeWithLocation : TileToCode
    {
        public TileLocation tileLocation;
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
    [SerializeField] private TileToCodeWithLocation[] landTileCodes = null;
    private TilemapInformationWithLocation landTilemapInfo;

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
            landTilemapInfo      = new TilemapInformationWithLocation(landTileCodes);
            waterBGTilemapInfo   = new TilemapInformation(waterBGTileCodes);
        }
    }

    public bool TryCreateSand(Vector2Int position)
    {
        void AddCodeToWaterBackgroundTile(Vector2Int pos, int code)
        {
            if (AddOrRemoveTileCode(pos, code, waterBGTilemapInfo, waterBGTilemap, true, out int newCode))
            {
                TileInformationManager.Instance.TryGetTileInformation(pos, out TileInformation t);
                int[] tileTracker = t.waterBGTracker;
                for (int i = 0; i < 4; i++)
                    tileTracker[i] += (code >> i & 1); //Add 1 or 0
            }
        }

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

                TileInformationManager.Instance.TryGetTileInformation(position, out TileInformation t);
                t.tileLocation = TileLocation.Sand;
            }

            //Neighbours
            Dictionary<Vector2Int, int> codesToAddToNeighbours = new Dictionary<Vector2Int, int>()
                {
                    {  new Vector2Int(position.x-1, position.y+1), 0b000001000 }, //UpLeft    
                    {  new Vector2Int(position.x,   position.y+1), 0b000001110 }, //Up        
                    {  new Vector2Int(position.x+1, position.y+1), 0b000000010 }, //UpRight   
                    {  new Vector2Int(position.x+1, position.y),   0b010000011 }, //Right    
                    {  new Vector2Int(position.x+1, position.y-1), 0b010000000 }, //DownRight
                    {  new Vector2Int(position.x,   position.y-1), 0b011100000 }, //Down      
                    {  new Vector2Int(position.x-1, position.y-1), 0b000100000 }, //DownLeft 
                    {  new Vector2Int(position.x-1, position.y),   0b000111000 }  //Left      
                };
            foreach (KeyValuePair<Vector2Int, int> neighbour in codesToAddToNeighbours)
            {
                AddOrRemoveTileCode(neighbour.Key, neighbour.Value, sandWaterTilemapInfo, sandTilemap, true, out int newCode);
                TileInformationManager.Instance.TryGetTileInformation(neighbour.Key, out TileInformation neighbourInfo);
                if (TileLocation.Water.HasFlag(neighbourInfo.tileLocation))
                    neighbourInfo.tileLocation = TileLocation.WaterEdge;
            }
        }
        //Water background
        {
            HashSet<Vector2Int> visitedPositions = new HashSet<Vector2Int>();

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    Vector2Int neighbourPos = new Vector2Int(position.x + i, position.y + j);
                    Dictionary<Vector2Int, int> neigbourOfNeighbours = new Dictionary<Vector2Int, int>()
                        {
                            {  new Vector2Int(neighbourPos.x-1, neighbourPos.y+1), 0b0010 }, //UpLeft    
                            {  new Vector2Int(neighbourPos.x,   neighbourPos.y+1), 0b0011 }, //Up        
                            {  new Vector2Int(neighbourPos.x+1, neighbourPos.y+1), 0b0001 }, //UpRight   
                            {  new Vector2Int(neighbourPos.x+1, neighbourPos.y),   0b1001 }, //Right    
                            {  new Vector2Int(neighbourPos.x+1, neighbourPos.y-1), 0b1000 }, //DownRight
                            {  new Vector2Int(neighbourPos.x,   neighbourPos.y-1), 0b1100 }, //Down      
                            {  new Vector2Int(neighbourPos.x-1, neighbourPos.y-1), 0b0100 }, //DownLeft 
                            {  new Vector2Int(neighbourPos.x-1, neighbourPos.y),   0b0110 }  //Left      
                        };

                    AddCodeToWaterBackgroundTile(neighbourPos, 0b1111);
                    foreach (KeyValuePair<Vector2Int, int> neighbourOfNeighbour in neigbourOfNeighbours)
                    {
                        AddCodeToWaterBackgroundTile(neighbourOfNeighbour.Key, neighbourOfNeighbour.Value);
                    }
                }
            }
        }

        return true;
    }

    public bool TryRemoveSand(Vector2Int position)
    {
        if (TerrainRemoveable(position, out int ln))
        {
            if (ln != 0)
                return false;
        }
        else
            return false;

        Dictionary<Vector2Int, int> mouseTileBitsToAddFromNeighbours = new Dictionary<Vector2Int, int>()
            {
                {  new Vector2Int(position.x-1, position.y+1), 0b010000000 }, //UpLeft    
                {  new Vector2Int(position.x,   position.y+1), 0b011100000 }, //Up        
                {  new Vector2Int(position.x+1, position.y+1), 0b000100000 }, //UpRight   
                {  new Vector2Int(position.x+1, position.y),   0b000111000 }, //Right    
                {  new Vector2Int(position.x+1, position.y-1), 0b000001000 }, //DownRight
                {  new Vector2Int(position.x,   position.y-1), 0b000001110 }, //Down      
                {  new Vector2Int(position.x-1, position.y-1), 0b000000010 }, //DownLeft 
                {  new Vector2Int(position.x-1, position.y),   0b010000011 }  //Left      
            };

        //Change self tile
        {
            int newTileCode = 0b000000000;
            foreach (KeyValuePair<Vector2Int, int> neighbour in mouseTileBitsToAddFromNeighbours)
            {
                TileInformationManager.Instance.TryGetTileInformation(neighbour.Key, out TileInformation t);
                if (t.tileLocation != TileLocation.WaterEdge)
                {
                    newTileCode = newTileCode | neighbour.Value;
                }
            }
            SetTileCode(position, newTileCode, sandWaterTilemapInfo, sandTilemap);

            TileInformationManager.Instance.TryGetTileInformation(position, out TileInformation positionTile);

            if (sandTilemap.GetTile((Vector3Int)position) == null)
                positionTile.tileLocation = TileLocation.DeepWater;
            else
                positionTile.tileLocation = TileLocation.WaterEdge;
        }
        //Change neighbours tile
        {
            foreach (KeyValuePair<Vector2Int, int> neighbour in mouseTileBitsToAddFromNeighbours)
            {
                //If neighbour is not water, we don't need to change it.
                TileInformationManager.Instance.TryGetTileInformation(neighbour.Key, out TileInformation t);
                if (t.tileLocation != TileLocation.WaterEdge)
                {
                    continue;
                }

                Dictionary<Vector2Int, int> neighboursTileBitsToAddFromNeighbours = new Dictionary<Vector2Int, int>()
                    {
                        {  new Vector2Int(neighbour.Key.x-1, neighbour.Key.y+1), 0b010000000 }, //UpLeft    
                        {  new Vector2Int(neighbour.Key.x,   neighbour.Key.y+1), 0b011100000 }, //Up        
                        {  new Vector2Int(neighbour.Key.x+1, neighbour.Key.y+1), 0b000100000 }, //UpRight   
                        {  new Vector2Int(neighbour.Key.x+1, neighbour.Key.y),   0b000111000 }, //Right    
                        {  new Vector2Int(neighbour.Key.x+1, neighbour.Key.y-1), 0b000001000 }, //DownRight
                        {  new Vector2Int(neighbour.Key.x,   neighbour.Key.y-1), 0b000001110 }, //Down      
                        {  new Vector2Int(neighbour.Key.x-1, neighbour.Key.y-1), 0b000000010 }, //DownLeft 
                        {  new Vector2Int(neighbour.Key.x-1, neighbour.Key.y),   0b010000011 }  //Left      
                    };

                int neighbourTileCode = 0b000000000;
                foreach (KeyValuePair<Vector2Int, int> neighboursNeighbour in neighboursTileBitsToAddFromNeighbours)
                {
                    TileInformationManager.Instance.TryGetTileInformation(neighboursNeighbour.Key, out TileInformation nnt);
                    TileLocation tileLocation = nnt.tileLocation;
                    if (!TileLocation.Water.HasFlag(tileLocation))
                    {
                        neighbourTileCode = neighbourTileCode | neighboursNeighbour.Value;
                    }
                }
                SetTileCode(neighbour.Key, neighbourTileCode, sandWaterTilemapInfo, sandTilemap);
                if (neighbourTileCode == 0b000000000)
                {
                    TileInformationManager.Instance.TryGetTileInformation(neighbour.Key, out TileInformation nt);
                    nt.tileLocation = TileLocation.DeepWater;
                }
            }
        }
        //Water background
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    Vector2Int neighbourPos = new Vector2Int(position.x + i, position.y + j);
                    Dictionary<Vector2Int, int> neigbourOfNeighbours = new Dictionary<Vector2Int, int>()
                        {
                            {  new Vector2Int(neighbourPos.x-1, neighbourPos.y+1), 0b0010 }, //UpLeft    
                            {  new Vector2Int(neighbourPos.x,   neighbourPos.y+1), 0b0011 }, //Up        
                            {  new Vector2Int(neighbourPos.x+1, neighbourPos.y+1), 0b0001 }, //UpRight   
                            {  new Vector2Int(neighbourPos.x+1, neighbourPos.y),   0b1001 }, //Right    
                            {  new Vector2Int(neighbourPos.x+1, neighbourPos.y-1), 0b1000 }, //DownRight
                            {  new Vector2Int(neighbourPos.x,   neighbourPos.y-1), 0b1100 }, //Down      
                            {  new Vector2Int(neighbourPos.x-1, neighbourPos.y-1), 0b0100 }, //DownLeft 
                            {  new Vector2Int(neighbourPos.x-1, neighbourPos.y),   0b0110 }  //Left      
                        };

                    RemoveCodeFromWaterBackgroundTile(neighbourPos, 0b1111);
                    foreach (KeyValuePair<Vector2Int, int> neighbourOfNeighbour in neigbourOfNeighbours)
                    {
                        RemoveCodeFromWaterBackgroundTile(neighbourOfNeighbour.Key, neighbourOfNeighbour.Value);
                    }

                }
            }
        }

        return true;

        void RemoveCodeFromWaterBackgroundTile(Vector2Int toRemovePosition, int code)
        {
            if (!TileInformationManager.Instance.PositionInMap(toRemovePosition))
                return;

            TileInformationManager.Instance.TryGetTileInformation(toRemovePosition, out TileInformation t);
            int[] tileTracker = t.waterBGTracker;

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

    public bool TryCreateLand(Vector2Int position, int layerNumber) {

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

        Dictionary<Vector2Int, int> codesToAddToTiles = new Dictionary<Vector2Int, int>()
        {
            { position,                                         0b1100 },
            { new Vector2Int(position.x - 1, position.y),       0b0100 },
            { new Vector2Int(position.x + 1, position.y),       0b1000 },
            { new Vector2Int(position.x, position.y + 1),       0b0011 },
            { new Vector2Int(position.x - 1, position.y + 1),   0b0010 },
            { new Vector2Int(position.x + 1, position.y + 1),   0b0001 },
        };

        foreach(KeyValuePair<Vector2Int, int> pair in codesToAddToTiles)
        {
            bool existingTileIsGrass = TileIsGrass(tilemapLayer.GetTile((Vector3Int)pair.Key), tilemapLayer);

            AddOrRemoveTileCode(pair.Key, pair.Value, landTilemapInfo, tilemapLayer, true, out int newCode);

            //Set new layers
            TileInformationManager.Instance.TryGetTileInformation(pair.Key, out TileInformation tile);
            bool newTileIsGrass = TileIsGrass(tilemapLayer.GetTile((Vector3Int)pair.Key), tilemapLayer);

            if (newTileIsGrass && !existingTileIsGrass)
                tile.layerNum++;

            tile.tileLocation = landTilemapInfo.codeToLocation[newCode];
        }
        return true;
    }

    public bool TryRemoveLand(Vector2Int position, int layerNumber)
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

        Dictionary<Vector2Int, int> bitsToRemoveFromTiles = new Dictionary<Vector2Int, int>()
        {
            { position,                                        0b0011 },
            { new Vector2Int(position.x - 1, position.y),      0b1011 },
            { new Vector2Int(position.x + 1, position.y),      0b0111 },
            { new Vector2Int(position.x, position.y + 1),      0b1100 },
            { new Vector2Int(position.x - 1, position.y + 1),  0b1101 },
            { new Vector2Int(position.x + 1, position.y + 1),  0b1110 },
        };

        foreach (KeyValuePair<Vector2Int, int> pair in bitsToRemoveFromTiles)
        {
            TileBase existingTile = tilemapLayer.GetTile((Vector3Int)pair.Key);
            bool existingTileIsGrass = TileIsGrass(existingTile, tilemapLayer);

            if (AddOrRemoveTileCode(pair.Key, pair.Value, landTilemapInfo, tilemapLayer, false, out int newCode))
            {
                //Set new layers
                TileInformationManager.Instance.TryGetTileInformation(pair.Key, out TileInformation tile);
                bool newTileIsGrass = TileIsGrass(tilemapLayer.GetTile((Vector3Int)pair.Key), tilemapLayer);

                if (!newTileIsGrass && existingTileIsGrass)
                    tile.layerNum--;

                TileBase newTile = tilemapLayer.GetTile((Vector3Int)pair.Key);

                if (newTile == null && existingTile == null)
                {
                    //Nothing changed
                }
                else if (newTile == null)
                {
                    if (tile.layerNum == 0)
                    {
                        if (!TileLocation.Water.HasFlag(tile.tileLocation))
                            tile.tileLocation = TileLocation.Sand;
                    }
                    else
                    {
                        tile.tileLocation = TileLocation.Grass;
                    }
                }
                else
                {
                    tile.tileLocation = landTilemapInfo.codeToLocation[newCode];
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
    public bool TerrainRemoveable(Vector2Int position, out int layerNumber)
    {
        Vector2Int aboveTilePosition = new Vector2Int(position.x, position.y + 1);
        if (!TileInformationManager.Instance.PositionInMap(position) ||
            !TileInformationManager.Instance.PositionInMap(aboveTilePosition))
        {
            layerNumber = Constants.INVALID_TILE_LAYER;
            return false;
        }

        TileInformationManager.Instance.TryGetTileInformation(position, out TileInformation mainTile);
        TileLocation mainTileLocation = mainTile.tileLocation;
        TileInformationManager.Instance.TryGetTileInformation(aboveTilePosition, out TileInformation aboveTileInformation);

        if (mainTileLocation == TileLocation.Sand)
        {
            if (checkForBuildAtPosition(position, 0)) {
                layerNumber = Constants.INVALID_TILE_LAYER;
                return false;
            }

            //If tile is sand, simple return
            layerNumber = 0;
            return true;
        }
        else
        {
            int proposedLayerNumber;
            if (TileLocation.Cliff.HasFlag(mainTile.tileLocation))
            {
                proposedLayerNumber = mainTile.layerNum + 1;
            }
            //Below
            else if (aboveTileInformation.tileLocation == TileLocation.Grass)
            {
                proposedLayerNumber = aboveTileInformation.layerNum;
            }
            else if (TileLocation.Cliff.HasFlag(aboveTileInformation.tileLocation) && aboveTileInformation.layerNum + 1 == mainTile.layerNum)
            {
                proposedLayerNumber = proposedLayerNumber = mainTile.layerNum;
            }
            else
            {
                layerNumber = Constants.INVALID_TILE_LAYER;
                return false;
            }

            // Check If Removing codes actually results in change
            {
                Dictionary<Vector2Int, int> codesToKeepInTiles = new Dictionary<Vector2Int, int>()
                {
                    { position,                                        0b0011 },
                    { new Vector2Int(position.x - 1, position.y),      0b1011 },
                    { new Vector2Int(position.x + 1, position.y),      0b0111 },
                    { new Vector2Int(position.x, position.y + 1),      0b1100 },
                    { new Vector2Int(position.x - 1, position.y + 1),  0b1101 },
                    { new Vector2Int(position.x + 1, position.y + 1),  0b1110 },
                };

                Tilemap tilemap = GetTilemapLayer(proposedLayerNumber);
                if (tilemap == null)
                {
                    layerNumber = Constants.INVALID_TILE_LAYER;
                    return false;
                }

                bool codeChangeFound = false;
                foreach (KeyValuePair<Vector2Int, int> pair in codesToKeepInTiles)
                {
                    TileBase existingTile = tilemap.GetTile((Vector3Int)pair.Key);
                    if (existingTile == null)
                        continue;

                    int existingTileCode = landTilemapInfo.tileNameToCode[existingTile.name];
                    if ((existingTileCode & pair.Value) != existingTileCode)
                        codeChangeFound = true;
                }
                if (!codeChangeFound)
                {
                    layerNumber = Constants.INVALID_TILE_LAYER;
                    return false;
                }
            }

            //Check layer up
            {
                int objectLayerNumToCheck = proposedLayerNumber;
                Vector2Int[] positionsToCheck = new Vector2Int[]
                {
                    new Vector2Int(aboveTilePosition.x,     aboveTilePosition.y),
                    new Vector2Int(aboveTilePosition.x + 1, aboveTilePosition.y),
                    new Vector2Int(aboveTilePosition.x - 1, aboveTilePosition.y),
                    new Vector2Int(position.x,     position.y),
                    new Vector2Int(position.x + 1, position.y),
                    new Vector2Int(position.x - 1, position.y)
                };

                //Check for objects
                foreach (Vector2Int positionToCheck in positionsToCheck)
                {
                    if (checkForBuildAtPosition(positionToCheck, objectLayerNumToCheck)) {
                        layerNumber = Constants.INVALID_TILE_LAYER;
                        return false;
                    }
                }

                //Check for land
                int terrainLayerNumToCheck = proposedLayerNumber + 1;
                Tilemap tilemapLayerToCheck = GetTilemapLayer(terrainLayerNumToCheck);
                if (tilemapLayerToCheck != null)
                {
                    foreach (Vector2Int positionToCheck in positionsToCheck)
                    {
                        if (tilemapLayerToCheck.GetTile((Vector3Int)positionToCheck) != null)
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

        bool checkForBuildAtPosition(Vector2Int pos, int layerNum)
        {
            TileInformationManager.Instance.TryGetTileInformation(pos, out TileInformation info);
            if (info.layerNum == layerNum && info.TopMostBuild != null)
            {
                return true;
            }
            return false;
        }
    }

    //Returns 0 if sand is placeable 1> for land is placeable
    public bool TerrainPlaceable(Vector2Int position, out int layerNumber)
    {
        Vector2Int aboveTilePosition = new Vector2Int(position.x, position.y + 1);
        if (!TileInformationManager.Instance.PositionInMap(position) ||
            !TileInformationManager.Instance.PositionInMap(aboveTilePosition))
        {
            layerNumber = Constants.INVALID_TILE_LAYER;
            return false;
        }

        TileInformationManager.Instance.TryGetTileInformation(position, out TileInformation mainTile);
        TileLocation mainTileLocation = mainTile.tileLocation;
        TileInformationManager.Instance.TryGetTileInformation(aboveTilePosition, out TileInformation aboveTileInformation);

        //Sand is placeable
        if (TileLocation.Water.HasFlag(mainTileLocation))
        {
            //Look for docks
            if (mainTile.TopFlooringGroup != null)
            {
                layerNumber = Constants.INVALID_TILE_LAYER;
                return false;
            }

            //Scan for outside map
            for (int i = -2; i <= 2; i++) {
                for (int j = -2; j <= 2; j++) {

                    TileInformationManager.Instance.TryGetTileInformation(new Vector2Int(position.x + i, position.y + j), out TileInformation tileInfo);
                    if (tileInfo == null)
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
            Vector2Int[] positionsToCheck = new Vector2Int[]
            {
                new Vector2Int(position.x,     position.y),
                new Vector2Int(position.x + 1, position.y),
                new Vector2Int(position.x - 1, position.y),
                new Vector2Int(position.x,     position.y + 1),
                new Vector2Int(position.x + 1, position.y + 1),
                new Vector2Int(position.x - 1, position.y + 1),
            };
            //First check easy stuff
            foreach (Vector2Int positionToCheck in positionsToCheck)
            {
                //Check if outside map
                if (!TileInformationManager.Instance.PositionInMap(positionToCheck)) {
                    layerNumber = Constants.INVALID_TILE_LAYER;
                    return false;
                }
                //Check if any of the tiles are water
                TileInformationManager.Instance.TryGetTileInformation(positionToCheck, out TileInformation tile);
                if (TileLocation.Water.HasFlag(tile.tileLocation)) {
                    layerNumber = Constants.INVALID_TILE_LAYER;
                    return false;
                }
            }

            //Check if placeable above
            bool placeableAbove = true;
            foreach (Vector2Int positionToCheck in positionsToCheck) {
                TileInformationManager.Instance.TryGetTileInformation(positionToCheck, out TileInformation tile);

                //Check if on correct location
                if (tile.layerNum < mainTile.layerNum)
                    placeableAbove = false;

                //Check for objects
                if (tile.TopMostBuild != null || tile.TopFlooringGroup != null)
                    placeableAbove = false;
            }
            //Placeable above
            if (placeableAbove) {
                layerNumber = mainTile.layerNum + 1;
                if (CheckIfAddingCodesResultsInChange(layerNumber))
                    return true;
            }
            //Check if placeable below
            bool placeableBelow = true;
            foreach (Vector2Int positionToCheck in positionsToCheck)
            {
                TileInformationManager.Instance.TryGetTileInformation(positionToCheck, out TileInformation tile);
                TileLocation location = tile.tileLocation;

                //Check if on correct location
                if (tile.layerNum < aboveTileInformation.layerNum)
                    placeableBelow = false;

                //Check for objects (but only if they are on below)
                if (tile.TopMostBuild != null || tile.TopFlooringGroup != null)
                {
                    if (aboveTileInformation.layerNum == tile.layerNum)
                        placeableBelow = false;
                }
            }
            //Placeable below
            if (placeableBelow) {
                layerNumber = aboveTileInformation.layerNum + 1;
                if (CheckIfAddingCodesResultsInChange(layerNumber))
                    return true;
            }

            //If not placeable above or below
            layerNumber = Constants.INVALID_TILE_LAYER;
            return false;

            bool CheckIfAddingCodesResultsInChange(int layerNum)
            {
                Dictionary<Vector2Int, int> codesToAddToTiles = new Dictionary<Vector2Int, int>()
                {
                    { position,                                            0b1100 },
                    { new Vector2Int(position.x - 1, position.y),       0b0100 },
                    { new Vector2Int(position.x + 1, position.y),       0b1000 },
                    { new Vector2Int(position.x, position.y + 1),       0b0011 },
                    { new Vector2Int(position.x - 1, position.y + 1),   0b0010 },
                    { new Vector2Int(position.x + 1, position.y + 1),   0b0001 },
                };

                Tilemap tilemap = GetTilemapLayer(layerNum);
                if (tilemap == null)
                    return true;

                foreach (KeyValuePair<Vector2Int, int> pair in codesToAddToTiles)
                {
                    TileBase existingTile = tilemap.GetTile((Vector3Int)pair.Key);
                    if (existingTile == null)
                        return true;

                    int existingTileCode = landTilemapInfo.tileNameToCode[existingTile.name];
                    if ((existingTileCode | pair.Value) != existingTileCode)
                        return true;
                }
                return false;
            }
        }
    }

    // On addMode    - 1 in code is bits to add
    // On removeMode - 1 in code is bits to keep
    private bool AddOrRemoveTileCode(Vector2Int position, int code, TilemapInformation tilemapInfo, Tilemap tilemap, bool addMode, out int newCode)
    {
        TileInformationManager.Instance.TryGetTileInformation(position, out TileInformation tileInfo);

        if (tileInfo == null)
        {
            newCode = Constants.EMPTY_TILECODE;
            return false;
        }

        TileBase existingTile = tilemap.GetTile((Vector3Int)position);
        int existingCode = (existingTile == null) ? 0 : tilemapInfo.tileNameToCode[existingTile.name];

        newCode = addMode ? code | existingCode : code & existingCode;

        tilemap.SetTile((Vector3Int)position, tilemapInfo.codeToTile[newCode]);
        tileInfo.InvokeTerrainModified();

        return true;
    }

    private bool SetTileCode(Vector2Int position, int code, TilemapInformation tilemapInfo, Tilemap tilemap)
    {
        TileInformationManager.Instance.TryGetTileInformation(position, out TileInformation tileInfo);
        if (tileInfo == null)
            return false;

        tilemap.SetTile((Vector3Int)position, tilemapInfo.codeToTile[code]);
        tileInfo.InvokeTerrainModified();
        return true;
    }

    public void ClearAllTerrain()
    {
        sandTilemap.ClearAllTiles();
        waterBGTilemap.ClearAllTiles();

        foreach (Tilemap t in tilemapLayers_)
        {
            Destroy(t.gameObject);
        }
        tilemapLayers_.Clear();

        int mapSize = TileInformationManager.mapSize;

        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                TileInformationManager.Instance.TryGetTileInformation(new Vector2Int(i, j), out TileInformation t);
                t.ResetTerrainInformation();
            }
        }
    }
}