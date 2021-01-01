using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class SaveInfo
{
    [SerializeField] private string saveDataPath;
    public string SaveDataPath => saveDataPath;

    [SerializeField] private string saveTimeUnformatted;
    public string SaveTimeUnformatted => saveTimeUnformatted;

    [SerializeField] private string saveTimeFormatted;
    public string SaveTimeFormatted => saveTimeFormatted;

    [SerializeField] private string playerName;
    public string PlayerName => playerName;

    public SaveInfo(string saveDataPath)
    {
        DateTime timeNow = DateTime.Now;

        this.saveDataPath   = saveDataPath;
        saveTimeUnformatted = timeNow.ToString("yyyyMMddHHmmss");
        saveTimeFormatted   = timeNow.ToString("yyyy-MM-dd h:mm-ss tt");
        playerName = PlayerCustomization.PlayerName;
    }
}
