using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "States/Region")]
public class CreateRegionState : PlayerState
{
    private Tilemap showRegionTilemap;
    [SerializeField] private Tile showRegionTile = null;
    [SerializeField] private Sprite showRegionSprite = null;

    private RegionInformation selectedRegion;
    private bool coroutineRunning = false;

    private TilesIndicatorManager indicatorManager;

    public override bool AllowMovement { get { return true; } }

    public override void Initialize()
    {
        showRegionTilemap = GameObject.FindGameObjectWithTag("ShowRegionTilemap").GetComponent<Tilemap>();
        ShowRegions(false);
    }

    public override void StartState(object[] args)
    {
        selectedRegion = (RegionInformation)args[0];
        ShowRegions(true);

        indicatorManager = new TilesIndicatorManager();
    }

    public override bool TryEndState()
    {
        if (coroutineRunning)
        {
            return false;
        }
        else
        {
            indicatorManager.ClearCurrentTiles();
            ShowRegions(false);
            return true;
        }
    }

    public override void Execute()
    {
        if (coroutineRunning)
            return;

        if (selectedRegion == null)
        {
            Debug.Log("No region selected.");
            return;
        }

        Vector3Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();
        TileInformation mouseTile = TileInformationManager.Instance.GetTileInformation(mouseTilePosition);

        bool regionPlaceable = RegionManager.RegionPlaceable(selectedRegion, mouseTilePosition);
        bool regionRemoveable = (mouseTile != null && mouseTile.region != null);

        //Indicator things
        {
            indicatorManager.SwapCurrentTiles(mouseTilePosition);
            indicatorManager.SetSprite(mouseTilePosition, showRegionSprite);

            if (regionPlaceable)
                indicatorManager.SetColor(mouseTilePosition, selectedRegion.ShowColor);
            else if (regionRemoveable) //Region is removeable
                indicatorManager.SetColor(mouseTilePosition, ResourceManager.Instance.Yellow);
            else
                indicatorManager.SetColor(mouseTilePosition, ResourceManager.Instance.Red);
        }

        if (regionPlaceable && CheckMouseOverUI.GetButtonDownAndNotOnUI("Primary"))
        {
            indicatorManager.ClearCurrentTiles();
            Coroutines.Instance.StartCoroutine(PlaceRegion(selectedRegion));
        }
    }

    private void ShowRegions(bool show)
    {
        showRegionTilemap.gameObject.SetActive(show);

        if (show)
        {
            for (int i = 0; i < TileInformationManager.mapSize; i++)
            {
                for (int j = 0; j < TileInformationManager.mapSize; j++)
                {
                    Vector3Int pos = (new Vector3Int(i, j, 0));
                    if (TileInformationManager.Instance.GetTileInformation(pos).region != null)
                    {
                        showRegionTilemap.SetTile(pos, showRegionTile);
                        showRegionTilemap.SetTileFlags(pos, TileFlags.None);
                        showRegionTilemap.SetColor(pos, TileInformationManager.Instance.GetTileInformation(pos).region.ShowColor);
                    }
                    else
                    {
                        showRegionTilemap.SetTile(pos, null);
                    }
                }
            }
        }
    }

    IEnumerator PlaceRegion(RegionInformation info)
    {
        coroutineRunning = true;
        Vector3Int previousTilePosition = new Vector3Int(-1, -1, -1);
        indicatorManager = new TilesIndicatorManager(); //We need to refresh the previous tiles

        Vector3Int startPos = TileInformationManager.Instance.GetMouseTile();
        int[,] regionPlaceableCache = new int[TileInformationManager.mapSize, TileInformationManager.mapSize]; // 0 not visited, -1 not placeable, 1 placeable

        int minX = -1, maxX = -1, minY = -1, maxY = -1;

        bool placeable = false;

        HashSet<Vector3Int> currentPositions = new HashSet<Vector3Int>();

        while (Input.GetButton("Primary"))
        {
            currentPositions.Clear();

            Vector3Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();

            if (mouseTilePosition != previousTilePosition)
                previousTilePosition = mouseTilePosition;
            else
                yield return 0;

            minX = (startPos.x >= mouseTilePosition.x) ? mouseTilePosition.x : startPos.x;
            maxX = (startPos.x >= mouseTilePosition.x) ? startPos.x : mouseTilePosition.x;
            minY = (startPos.y >= mouseTilePosition.y) ? mouseTilePosition.y : startPos.y;
            maxY = (startPos.y >= mouseTilePosition.y) ? startPos.y : mouseTilePosition.y;

            placeable = true;

            for (int i = minX; i <= maxX; i++)
            {
                for (int j = minY; j <= maxY; j++)
                {
                    Vector3Int pos = new Vector3Int(i, j, 0);
                    if (!TileInformationManager.Instance.PositionInMap(pos))
                        continue;

                    int tilePlaceable = regionPlaceableCache[i, j];
                    if (tilePlaceable == 0)
                    {
                        tilePlaceable = (RegionManager.RegionPlaceable(info, pos)) ? 1 : 0;
                        regionPlaceableCache[i, j] = tilePlaceable;
                    }

                    if (tilePlaceable == -1)
                    {
                        placeable = false;
                    }

                    currentPositions.Add(pos);
                }
            }

            //Show new indicators
            List<Vector3Int> newlyShownTiles = indicatorManager.SwapCurrentTiles(currentPositions);
            foreach (Vector3Int pos in newlyShownTiles)
            {
                indicatorManager.SetSprite(pos, showRegionSprite);
                indicatorManager.SetColor(pos, selectedRegion.ShowColor);
            }

            yield return 0;
        }

        coroutineRunning = false;
        indicatorManager.ClearCurrentTiles();

        if (placeable)
        {
            if (RegionManager.TryCreateRegion(info, currentPositions))
            {
                foreach (Vector3Int pos in currentPositions)
                {
                    showRegionTilemap.SetTile(pos, showRegionTile);
                    showRegionTilemap.SetTileFlags(pos, TileFlags.None);
                    showRegionTilemap.SetColor(pos, info.ShowColor);
                }
            }
        }
    }

    IEnumerator RemoveRegion()
    {
        //TODO
        /*
        coroutineRunning = true;
        Vector3Int previousTilePosition = new Vector3Int(-1, -1, -1);

        while (Input.GetButton("Primary"))
        {
            Vector3Int mouseTilePosition = GetMouseTile();

            if (mouseTilePosition != previousTilePosition)
                previousTilePosition = mouseTilePosition;
            else
                goto Skip;

            Skip:
            yield return 0;
        }
        */
        yield return 0;
    }

}
