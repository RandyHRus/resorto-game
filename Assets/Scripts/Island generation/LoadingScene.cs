using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadSceneAsync("Main");
    }
}
