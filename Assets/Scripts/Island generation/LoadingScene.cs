using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.3f);
        SceneManager.LoadSceneAsync("Main");
    }
}
