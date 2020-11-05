﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Create Terrain")]
public class CreateTerrainState : PlayerState
{
    [SerializeField] private Sprite landIndicatorSprite = null;
    [SerializeField] private Sprite sandIndicatorSprite = null;
    [SerializeField] private Sprite noneIndicatorSprite = null;
    private bool coroutineRunning = false;
    private Coroutine currentCoroutine;

    private TilesIndicatorManager indicatorManager;

    public override bool AllowMovement => false;
    public override bool AllowMouseDirectionChange => false;
    public override CameraMode CameraMode => CameraMode.Drag;

    public override void StartState(object[] args)
    {
        indicatorManager = new TilesIndicatorManager();
    }

    public override void EndState()
    {
        if (coroutineRunning)
            Coroutines.Instance.StopCoroutine(currentCoroutine);

        indicatorManager.ClearCurrentTiles();
    }

    public override void Execute()
    {
        if (coroutineRunning)
            return;

        Vector2Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();

        indicatorManager.SwapCurrentTiles(mouseTilePosition);

        if (TerrainManager.Instance.TerrainPlaceable(mouseTilePosition, out int layerNumber))
        {
            indicatorManager.SetColor(mouseTilePosition, ResourceManager.Instance.Green);

            if (layerNumber == 0)
                indicatorManager.SetSprite(mouseTilePosition, sandIndicatorSprite);
            else
                indicatorManager.SetSprite(mouseTilePosition, landIndicatorSprite);

            if (CheckMouseOverUI.GetButtonDownAndNotOnUI("Primary"))
            {
                if (layerNumber == 0)
                {
                    currentCoroutine = Coroutines.Instance.StartCoroutine(PlaceSand());
                    return;
                }
                else if (layerNumber >= 1)
                {
                    currentCoroutine = Coroutines.Instance.StartCoroutine(PlaceLand(layerNumber));
                    return;
                }
            }
        }
        else
        {
            indicatorManager.SetSprite(mouseTilePosition, noneIndicatorSprite);
            indicatorManager.SetColor(mouseTilePosition, ResourceManager.Instance.Red);
        }
    }

    IEnumerator PlaceSand()
    {
        coroutineRunning = true;
        indicatorManager.ClearCurrentTiles();
        Vector2Int previousTilePosition = new Vector2Int(-1, -1);

        while (Input.GetButton("Primary"))
        {
            Vector2Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();

            if (mouseTilePosition != previousTilePosition)
            {
                previousTilePosition = mouseTilePosition;
                if (TerrainManager.Instance.TryCreateSand(mouseTilePosition))
                {
                    ParticlesManager.Instance.PlaySmokeEffect(mouseTilePosition);
                }
            }

            yield return 0;
        }
        coroutineRunning = false;
    }

    IEnumerator PlaceLand(int layerNumber)
    {
        coroutineRunning = true;
        indicatorManager.ClearCurrentTiles();
        Vector2Int previousTilePosition = new Vector2Int(-1, -1);

        while (Input.GetButton("Primary"))
        {
            Vector2Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();

            if (mouseTilePosition != previousTilePosition)
            {
                previousTilePosition = mouseTilePosition;
                if (TerrainManager.Instance.TryCreateLand(mouseTilePosition, layerNumber))
                {
                    ParticlesManager.Instance.PlaySmokeEffect(mouseTilePosition);
                }
            }

            yield return 0;
        }
        coroutineRunning = false;
    }
}
