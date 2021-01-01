using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneLoader: MonoBehaviour
{
    public static SceneLoadOption loadOption = 0;
    public static string loadSaveDataPath;

    public void Start()
    {
        switch (loadOption)
        {
            case (SceneLoadOption.NewIsland):
                IslandGenerationPipeline.GenerateIsland();
                break;
            case (SceneLoadOption.LoadSave):
                SaveManager.LoadGame(loadSaveDataPath);
                break;
            default:
                throw new System.NotImplementedException();
        }
    }
}

public enum SceneLoadOption
{
    NewIsland,
    LoadSave
}