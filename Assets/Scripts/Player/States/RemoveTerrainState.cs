using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/Remove Terrain")]
public class RemoveTerrainState : PlayerState
{
    public override bool AllowMovement => false;
    public override bool AllowMouseDirectionChange => false;
    public override CameraMode CameraMode => CameraMode.Drag;

    private bool coroutineRunning = false;
    private Coroutine currentCoroutine;

    public override void StartState(object[] args)
    {

    }

    public override void Execute()
    {
        if (coroutineRunning)
            return;

        OutlineIndicatorManager.Instance.Toggle(true);

        Vector2Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();

        if (TerrainManager.Instance.TerrainRemoveable(mouseTilePosition, out int layerNumber))
        {
            OutlineIndicatorManager.Instance.SetSizeAndPosition(mouseTilePosition, mouseTilePosition);
            OutlineIndicatorManager.Instance.SetColor(ResourceManager.Instance.Green);

            if (CheckMouseOverUI.GetButtonDownAndNotOnUI("Primary"))
            {
                OutlineIndicatorManager.Instance.Toggle(false);

                if (layerNumber == 0)
                {
                    currentCoroutine = Coroutines.Instance.StartCoroutine(RemoveSand());
                    return;
                }
                else if (layerNumber >= 1)
                {
                    currentCoroutine = Coroutines.Instance.StartCoroutine(RemoveLand(layerNumber));
                    return;
                }
            }
        }
        else
        {
            OutlineIndicatorManager.Instance.SetSizeAndPosition(mouseTilePosition, mouseTilePosition);
            OutlineIndicatorManager.Instance.SetColor(ResourceManager.Instance.Red);
        }
    }
    
    public override void EndState()
    {
        if (coroutineRunning)
            Coroutines.Instance.StopCoroutine(currentCoroutine);

        OutlineIndicatorManager.Instance.Toggle(false);
    }

    IEnumerator RemoveSand()
    {
        coroutineRunning = true;
        Vector2Int previousTilePosition = new Vector2Int(-1, -1);

        while (Input.GetButton("Primary"))
        {
            Vector2Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();

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

    IEnumerator RemoveLand(int layerNumber)
    {
        coroutineRunning = true;
        Vector2Int previousTilePosition = new Vector2Int(-1, -1);

        while (Input.GetButton("Primary"))
        {
            Vector2Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();

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
