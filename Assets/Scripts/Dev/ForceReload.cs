using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ForceReload : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.O))
        {
            SceneManager.LoadScene("Loading");
        }
        #endif
    }
}
