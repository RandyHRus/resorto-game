using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IslandGenerationPipeline : MonoBehaviour
{
    public delegate void IslandGenerationCompleted();
    public static event IslandGenerationCompleted IslandCompleted;

    private void Start()
    {
        try
        {
            IslandTerrainGenerator.Instance.GenerateIsland();
            IslandObjectsGenerator.Instance.GenerateIslandObjects();

            IslandCompleted?.Invoke();
        }
        // If something fails after generating island, will try to create another one
        // Things that may fails example: If it could not find valid position to spawn player
        catch (IslandGenerationException e)
        {
            //TODO request new island if something fails
            Debug.Log(e);
            Debug.Log("Requesting new island!");
            SceneManager.LoadScene("Main");
        }
    }
}
