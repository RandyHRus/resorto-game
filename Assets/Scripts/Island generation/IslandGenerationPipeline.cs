using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IslandGenerationPipeline : MonoBehaviour
{
    int tryCount = 5;

    public delegate void IslandGenerationCompleted(Vector2Int playerStartingPosition);
    public static event IslandGenerationCompleted IslandCompleted;

    private void Start()
    {
        bool completed = false;
        int currentTry = 1;

        while (!completed)
        {
            if (currentTry > tryCount)
            {
                #if UNITY_EDITOR
                // Application.Quit() does not work in the editor so
                // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
                UnityEditor.EditorApplication.isPlaying = false;
                #else
                Application.Quit();
                #endif

                break;
            }
            else
            {
                try
                {
                    IslandTerrainGenerator.Instance.GenerateIsland();
                    //IslandStartingPosition startingPosition = IslandStartingPositionGenerator.Instance.GetRandomStartingPosition();
                    IslandStartingDockGenerator.Instance.CreateStartingDock(out Vector2Int unloadingPosition);
                    //StartingChestGenerator.Instance.CreateStartingChest(startingPosition);
                    IslandObjectsGenerator.Instance.GenerateIslandObjects();

                    Vector2Int playerStartingPosition = unloadingPosition;

                    completed = true;
                    IslandCompleted?.Invoke(playerStartingPosition);
                }
                // If something fails after generating island, will try to create another one
                // Things that may fails example: If it could not find valid position to spawn player
                catch (IslandGenerationException e)
                {
                    //TODO request new island if something fails
                    Debug.Log("Try: " + currentTry + " Failed. Requesting new island! " + e);

                    IslandObjectsGenerator.Instance.RemoveAllBuilds();
                    IslandTerrainGenerator.Instance.ClearAllTerrain();

                    currentTry++;
                }
            }
        }
    }
}
