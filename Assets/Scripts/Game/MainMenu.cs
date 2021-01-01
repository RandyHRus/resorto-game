using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnNewGameClicked()
    {
        SceneManager.LoadScene("Character Customization");
    }

    public void OnLoadGameClicked()
    {
        //Todo
    }
}
