using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    private bool paused = false;
    private bool lockManualPause = false;

    private static PauseManager _instance;
    public static PauseManager Instance { get { return _instance; } }
    private void Awake()
    {
        //Singleton
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Pause") && !lockManualPause)
        {
            paused = !paused;
            Time.timeScale = paused ? 0 : 1;
        }
    }

    public void PauseGame(bool lockManualPause)
    {
        paused = true;
        this.lockManualPause = lockManualPause;
        Time.timeScale = 0;
    }

    public void UnPauseGame()
    {
        paused = false;
        lockManualPause = false;
        Time.timeScale = 1;
    }
}
