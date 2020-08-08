using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Terrain")]
public class TerrainState : PlayerState
{
    [SerializeField] private Sprite landIndicatorSprite = null;
    [SerializeField] private Sprite sandIndicatorSprite = null;
    private bool coroutineRunning = false;

    private TilesIndicatorManager indicatorManager;


    public override bool AllowMovement
    {
        get { return true; }
    }

    public override void StartState(object[] args)
    {
        indicatorManager = new TilesIndicatorManager();
    }

    public override bool TryEndState()
    {
        indicatorManager.ClearCurrentTiles();
        return !coroutineRunning;
    }

    public override void Execute()
    {
        if (coroutineRunning)
            return;

        Vector3Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();

        if (indicatorManager.SwapCurrentTiles(mouseTilePosition))
        {
            indicatorManager.SetSprite(mouseTilePosition, landIndicatorSprite);
            indicatorManager.SetColor(mouseTilePosition, ResourceManager.Instance.Red);
        }

        {
            if (TerrainManager.Instance.TerrainRemoveable(mouseTilePosition, out int layerNumber))
            {
                indicatorManager.SetColor(mouseTilePosition, ResourceManager.Instance.Yellow);

                if (CheckMouseOverUI.GetButtonDownAndNotOnUI("Secondary"))
                {
                    if (layerNumber == 0)
                    {
                        Coroutines.Instance.StartCoroutine(RemoveSand());
                        return;
                    }
                    else if (layerNumber >= 1)
                    {
                        Coroutines.Instance.StartCoroutine(RemoveLand(layerNumber));
                        return;
                    }
                }
            }
        }
        {
            if (TerrainManager.Instance.TerrainPlaceable(mouseTilePosition, out int layerNumber))
            {
                indicatorManager.SetColor(mouseTilePosition, ResourceManager.Instance.Green);

                if (layerNumber == 0)
                    indicatorManager.SetSprite(mouseTilePosition, sandIndicatorSprite);

                if (CheckMouseOverUI.GetButtonDownAndNotOnUI("Primary"))
                {
                    if (layerNumber == 0)
                    {
                        Coroutines.Instance.StartCoroutine(PlaceSand());
                        return;
                    }
                    else if (layerNumber >= 1)
                    {
                        Coroutines.Instance.StartCoroutine(PlaceLand(layerNumber));
                        return;
                    }
                }
            }
        }
    }

    IEnumerator PlaceSand()
    {
        coroutineRunning = true;
        indicatorManager.ClearCurrentTiles();
        Vector3Int previousTilePosition = new Vector3Int(-1, -1, -1);

        while (Input.GetButton("Primary"))
        {
            Vector3Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();

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

    IEnumerator RemoveSand()
    {
        coroutineRunning = true;
        indicatorManager.ClearCurrentTiles();
        Vector3Int previousTilePosition = new Vector3Int(-1, -1, -1);

        while (Input.GetButton("Secondary"))
        {
            Vector3Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();

            if (mouseTilePosition != previousTilePosition)
            {
                previousTilePosition = mouseTilePosition;
                if (TerrainManager.Instance.TryRemoveSand(mouseTilePosition))
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
        Vector3Int previousTilePosition = new Vector3Int(-1, -1, -1);

        while (Input.GetButton("Primary"))
        {
            Vector3Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();

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

    IEnumerator RemoveLand(int layerNumber)
    {
        coroutineRunning = true;
        indicatorManager.ClearCurrentTiles();
        Vector3Int previousTilePosition = new Vector3Int(-1, -1, -1);

        while (Input.GetButton("Secondary"))
        {
            Vector3Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();

            if (mouseTilePosition != previousTilePosition)
            {
                previousTilePosition = mouseTilePosition;
                if (TerrainManager.Instance.TryRemoveLand(mouseTilePosition, layerNumber))
                {
                    ParticlesManager.Instance.PlaySmokeEffect(mouseTilePosition);
                }
            }

            yield return 0;
        }
        coroutineRunning = false;
    }
}
