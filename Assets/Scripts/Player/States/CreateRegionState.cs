using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "States/Player/CreateRegion")]
public class CreateRegionState : PlayerState
{
    [SerializeField] MapVisualizer regionVisualizer = null;

    private RegionInformation selectedRegion;
    private bool coroutineRunning = false;
    private Coroutine currentCoroutine;

    private OutlineIndicatorManager indicatorManager;

    public override bool AllowMovement => false;
    public override bool AllowMouseDirectionChange => false;
    public override CameraMode CameraMode => CameraMode.Drag;

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void StartState(object[] args)
    {
        selectedRegion = (RegionInformation)args[0];
        regionVisualizer.ShowVisualizer();

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
        regionVisualizer.HideVisualizer();
    }

    public override void Execute()
    {
        if (coroutineRunning)
            return;

        Vector2Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();
        TileInformationManager.Instance.TryGetTileInformation(mouseTilePosition, out TileInformation mouseTile);

        bool regionPlaceable = RegionManager.RegionPlaceable(selectedRegion, mouseTilePosition);
        bool regionRemoveable = (mouseTile != null && mouseTile.Region != null);

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
            RegionManager.TryCreateRegion(info, currentPositions);
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

    public override void OnCancelButtonPressed()
    {
        if (selectedRegion != null)
        {
            InvokeChangeState(typeof(SelectRegionState), null);
        }
        else
            base.OnCancelButtonPressed();
    }
}
