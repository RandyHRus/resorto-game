using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Create flooring")]
public class CreateFlooringState : PlayerState
{
    private FlooringVariantBase flooringVariant;
    private FlooringRotation rotation;
    private bool coroutineRunning = false;
    private Coroutine floorCoroutine;

    private TilesIndicatorManager indicatorManager;

    public override bool AllowMovement => true;
    public override bool AllowMouseDirectionChange => true;

    public override void Execute()
    {
        if (coroutineRunning)
            return;

        //Rotate
        if (Input.GetButtonDown("RotateObject"))
        {
            rotation += 1;
            if ((int)rotation == Enum.GetNames(typeof(FlooringRotation)).Length)
                rotation = 0;
        }

        Vector3Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();
        bool floorPlaceable = FlooringManager.FlooringPlaceable(flooringVariant, mouseTilePosition);

        //Indicator things
        {
            Sprite proposedSprite = flooringVariant.IndicatorSprites[(int)rotation];
            indicatorManager.SwapCurrentTiles(mouseTilePosition);
            indicatorManager.SetSprite(mouseTilePosition, proposedSprite);
            indicatorManager.SetColor(mouseTilePosition, floorPlaceable ? ResourceManager.Instance.Green : ResourceManager.Instance.Red);
        }

        //CreateFloor
        if (floorPlaceable && CheckMouseOverUI.GetButtonDownAndNotOnUI("Primary"))
        {
            indicatorManager.ClearCurrentTiles();
            floorCoroutine = Coroutines.Instance.StartCoroutine(PlaceFloor());
        }
    }

    IEnumerator PlaceFloor()
    {
        coroutineRunning = true;
        Vector3Int previousTilePosition = new Vector3Int(-1, -1, -1);
        indicatorManager = new TilesIndicatorManager(); //We need to refresh the previous tiles

        Vector3Int startPos = TileInformationManager.Instance.GetMouseTile();
        int[,] floorPlaceableCache = new int[TileInformationManager.mapSize, TileInformationManager.mapSize]; // 0 not visited, -1 not placeable, 1 placeable

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

                    int tilePlaceable = floorPlaceableCache[i, j];
                    if (tilePlaceable != 0)
                    {
                        tilePlaceable = (FlooringManager.FlooringPlaceable(flooringVariant, new Vector3Int(i, j, 0))) ? 1 : 0;
                        floorPlaceableCache[i, j] = tilePlaceable;
                    }

                    if (tilePlaceable == -1)
                    {
                        placeable = false;
                    }

                    currentPositions.Add(new Vector3Int(i, j, 0));
                }
            }

            //Show new indicators
            indicatorManager.SwapCurrentTiles(currentPositions);
            
            foreach (Vector3Int pos in currentPositions)
            {
                indicatorManager.SetSprite(pos, FlooringManager.GetSprite(flooringVariant, currentPositions, true, pos, rotation));
                indicatorManager.SetColor(pos, ResourceManager.Instance.Green);
            }

            yield return 0;
        }
        coroutineRunning = false;
        indicatorManager.ClearCurrentTiles();

        if (placeable)
        {       
            if (FlooringManager.TryPlaceFlooring(flooringVariant, currentPositions, rotation))
            {
                //TODO Remove money or something
            }
        }
    }

    public override void StartState(object[] args)
    {
        rotation = FlooringRotation.Horizontal;

        flooringVariant = (FlooringVariantBase)args[0];
        if (flooringVariant == null)
            Debug.LogError("No flooring selected! This should not happen!");

        coroutineRunning = false;

        indicatorManager = new TilesIndicatorManager();
    }

    public override void EndState()
    {
        indicatorManager.ClearCurrentTiles();

        if (coroutineRunning)
        {
            Coroutines.Instance.StopCoroutine(floorCoroutine);
            coroutineRunning = false;
        }
    }
}
