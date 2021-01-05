using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnNewGameClicked()
    {
        LoadingScene.sceneName = "Character Customization";
        SceneManager.LoadScene("Loading");
    }

    public void OnLoadGameClicked()
    {
        //Todo
    }
}
