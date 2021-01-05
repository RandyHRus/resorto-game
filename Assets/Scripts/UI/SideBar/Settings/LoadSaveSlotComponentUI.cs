using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;

public class LoadSaveSlotComponentUI : SaveSlotComponentUI
{
    public LoadSaveSlotComponentUI(Transform parent, string saveInfoPath) : base(parent, saveInfoPath)
    {

    }

    public override void OnClick()
    {
        base.OnClick();
        MainSceneLoader.loadOption = MainSceneLoadOption.LoadSave;
        MainSceneLoader.loadSaveDataPath = saveInfo.SaveDataPath;

        LoadingScene.sceneName = "Main";
        SceneManager.LoadScene("Loading");
    }
}
