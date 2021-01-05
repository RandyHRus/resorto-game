using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneLoader: MonoBehaviour
{
    public static MainSceneLoadOption loadOption = 0;
    public static string loadSaveDataPath;

    public delegate void LoadingComplete();
    public static event LoadingComplete OnLoadingComplete;

    public void Start()
    {
        switch (loadOption)
        {
            case (MainSceneLoadOption.NewIsland):
                IslandGenerationPipeline.GenerateIsland();
                break;
            case (MainSceneLoadOption.LoadSave):
                SaveManager.LoadSaveData(loadSaveDataPath);
                break;
            default:
                throw new System.NotImplementedException();
        }

        OnLoadingComplete?.Invoke();
    }
}

public enum MainSceneLoadOption
{
    NewIsland,
    LoadSave
}