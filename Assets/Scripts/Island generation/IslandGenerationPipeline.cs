using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IslandGenerationPipeline
{
    public delegate void IslandGenerationCompleted(Vector2Int playerStartingPosition);
    public static event IslandGenerationCompleted IslandCompleted;

    public static void GenerateIsland()
    {
        try
        {
            IslandTerrainGenerator.Instance.GenerateIsland();
            IslandStartingDockGenerator.Instance.CreateStartingDock(out Vector2Int unloadingPosition);
            IslandObjectsGenerator.Instance.GenerateIslandObjects();

            Vector2Int playerStartingPosition = unloadingPosition;

            IslandCompleted?.Invoke(playerStartingPosition);
        }
        // If something fails after generating island, will try to create another one
        // Things that may fails example: If it could not find valid position to spawn player
        catch (IslandGenerationException e)
        {
            Debug.Log("Generation Failed. Requesting new island! " + e);
            SceneManager.LoadScene("Loading");
        }
    }
}
