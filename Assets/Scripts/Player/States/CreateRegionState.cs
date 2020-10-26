using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "States/Region")]
public class CreateRegionState : PlayerState
{
    private Tilemap showRegionTilemap;
    [SerializeField] private Tile showRegionTile = null;

    private RegionInformation selectedRegion;
    private bool coroutineRunning = false;
    private Coroutine currentCoroutine;

    private OutlineIndicatorManager indicatorManager;

    public override bool AllowMovement => true;
    public override bool AllowMouseDirectionChange => true;

    public override void Initialize()
    {
        base.Initialize();

        showRegionTilemap = GameObject.FindGameObjectWithTag("ShowRegionTilemap").GetComponent<Tilemap>();
        ShowRegions(false);
    }

    public override void StartState(object[] args)
    {
        selectedRegion = (RegionInformation)args[0];
        ShowRegions(true);

        indicatorManager = new OutlineIndicatorManager();
        indicatorManager.Toggle(true);
    }

    public override void EndState()
    {
        if (coroutineRunning)
        {
            Coroutines.Instance.StopCoroutine(currentCoroutine);
            coroutineRunning = false;
        }

        indicatorManager.Toggle(false);
        ShowRegions(false);
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

        Vector2Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();
        TileInformation mouseTile = TileInformationManager.Instance.GetTileInformation(mouseTilePosition);

        bool regionPlaceable = RegionManager.RegionPlaceable(selectedRegion, mouseTilePosition);
        bool regionRemoveable = (mouseTile != null && mouseTile.region != null);

        //Indicator things
        {
            indicatorManager.SetSizeAndPosition((Vector2Int)mouseTilePosition, (Vector2Int)mouseTilePosition);

            if (regionPlaceable)
                indicatorManager.SetColor(selectedRegion.ShowColor);
            else if (regionRemoveable) //Region is removeable
                indicatorManager.SetColor(ResourceManager.Instance.Yellow);
            else
                indicatorManager.SetColor(ResourceManager.Instance.Red);
        }

        if (regionPlaceable && CheckMouseOverUI.GetButtonDownAndNotOnUI("Primary"))
        {
            currentCoroutine = Coroutines.Instance.StartCoroutine(PlaceRegion(selectedRegion));
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
                    Vector2Int pos = (new Vector2Int(i, j));
                    if (TileInformationManager.Instance.GetTileInformation(pos).region != null)
                    {
                        showRegionTilemap.SetTile((Vector3Int)pos, showRegionTile);
                        showRegionTilemap.SetTileFlags((Vector3Int)pos, TileFlags.None);
                        showRegionTilemap.SetColor((Vector3Int)pos, TileInformationManager.Instance.GetTileInformation(pos).region.regionInformation.ShowColor);
                    }
                    else
                    {
                        showRegionTilemap.SetTile((Vector3Int)  pos, null);
                    }
                }
            }
        }
    }

    IEnumerator PlaceRegion(RegionInformation info)
    {
        coroutineRunning = true;
        Vector2Int previousTilePosition = new Vector2Int(-1, -1);

        Vector2Int startPos = TileInformationManager.Instance.GetMouseTile();
        int[,] regionPlaceableCache = new int[TileInformationManager.mapSize, TileInformationManager.mapSize]; // 0 not visited, -1 not placeable, 1 placeable

        int minX = -1, maxX = -1, minY = -1, maxY = -1;

        bool placeable = false;

        HashSet<Vector2Int> currentPositions = new HashSet<Vector2Int>();

        while (Input.GetButton("Primary"))
        {
            currentPositions.Clear();

            Vector2Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();

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
                    Vector2Int pos = new Vector2Int(i, j);
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

            indicatorManager.SetSizeAndPosition(new Vector2Int(minX, minY), new Vector2Int(maxX, maxY));
            indicatorManager.SetColor(selectedRegion.ShowColor);

            yield return 0;
        }

        coroutineRunning = false;

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
