﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/Create flooring")]
public class CreateFlooringState : PlayerState
{
    private FlooringVariantBase flooringVariant;
    private FlooringRotation rotation;
    private bool coroutineRunning = false;
    private Coroutine floorCoroutine;

    public override bool AllowMovement => false;
    public override bool AllowMouseDirectionChange => false;
    public override CameraMode CameraMode => CameraMode.Drag;

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

        Vector2Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();
        bool floorPlaceable = FlooringManager.Instance.FlooringPlaceable(flooringVariant, mouseTilePosition);

        //Indicator things
        {
            Sprite proposedSprite = flooringVariant.IndicatorSprites[(int)rotation];
            TilesIndicatorManager.Instance.SwapCurrentTiles(mouseTilePosition);
            TilesIndicatorManager.Instance.SetSprite(mouseTilePosition, proposedSprite);
            TilesIndicatorManager.Instance.SetColor(mouseTilePosition, floorPlaceable ? ResourceManager.Instance.Green : ResourceManager.Instance.Red);
        }

        //CreateFloor
        if (floorPlaceable && CheckMouseOverUI.GetButtonDownAndNotOnUI("Primary"))
        {
            TilesIndicatorManager.Instance.ClearCurrentTiles();
            floorCoroutine = Coroutines.Instance.StartCoroutine(PlaceFloor());
        }
    }

    IEnumerator PlaceFloor()
    {
        coroutineRunning = true;
        Vector2Int previousTilePosition = new Vector2Int(-1, -1);
        TilesIndicatorManager.Instance.ClearCurrentTiles();

        Vector2Int startPos = TileInformationManager.Instance.GetMouseTile();
        int[,] floorPlaceableCache = new int[TileInformationManager.mapSize, TileInformationManager.mapSize]; // 0 not visited, -1 not placeable, 1 placeable

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

                    int tilePlaceable = floorPlaceableCache[i, j];
                    if (tilePlaceable != 0)
                    {
                        tilePlaceable = (FlooringManager.Instance.FlooringPlaceable(flooringVariant, new Vector2Int(i, j))) ? 1 : 0;
                        floorPlaceableCache[i, j] = tilePlaceable;
                    }

                    if (tilePlaceable == -1)
                    {
                        placeable = false;
                    }

                    currentPositions.Add(new Vector2Int(i, j));
                }
            }

            //Show new indicators
            TilesIndicatorManager.Instance.SwapCurrentTiles(currentPositions);
            
            foreach (Vector2Int pos in currentPositions)
            {
                TilesIndicatorManager.Instance.SetSprite(pos, FlooringManager.Instance.GetSprite(flooringVariant, currentPositions, true, pos, rotation));
                TilesIndicatorManager.Instance.SetColor(pos, ResourceManager.Instance.Green);
            }

            yield return 0;
        }
        coroutineRunning = false;
        TilesIndicatorManager.Instance.ClearCurrentTiles();

        if (placeable)
        {       
            if (FlooringManager.Instance.TryPlaceFlooring(flooringVariant, currentPositions, rotation))
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
    }

    public override void EndState()
    {
        TilesIndicatorManager.Instance.ClearCurrentTiles();

        if (coroutineRunning)
        {
            Coroutines.Instance.StopCoroutine(floorCoroutine);
            coroutineRunning = false;
        }
    }
}
