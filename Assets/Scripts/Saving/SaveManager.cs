using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;

public static class SaveManager
{
    public static readonly string INFO_EXTENSION = ".info";
    public static readonly string DATA_EXTENSION = ".data";

    public static void SaveGameNewFile()
    {
        string fileName = "Save_" + DateTime.Now.ToString("yyyyMMddHHmmss");
        SaveGame(fileName);
    }

    public static void SaveGame(string fileName)
    {
        string saveInfoPath = Path.Combine(Application.persistentDataPath, fileName + INFO_EXTENSION);
        string saveDataPath = Path.Combine(Application.persistentDataPath, fileName + DATA_EXTENSION);

        SaveInfo saveInfo = new SaveInfo(saveDataPath);
        string saveInfoJson = JsonUtility.ToJson(saveInfo);

        SaveData saveData = new SaveData();
        string saveDataJson = JsonUtility.ToJson(saveData);

        Debug.Log("Saving to:" + saveInfoPath);

        try
        {
            //If the file exists, this overwrites it.
            //If the file does not exist, this creates it.
            File.WriteAllText(saveInfoPath, saveInfoJson);
            File.WriteAllText(saveDataPath, saveDataJson);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.StackTrace);
        }
    }

    public static IEnumerable<string> EnumerateSaveFiles(SaveFileType type)
    {
        List<string> paths = new List<string>();

        string extension = type == SaveFileType.Info ? INFO_EXTENSION : DATA_EXTENSION;

        foreach (string file in Directory.EnumerateFiles(Application.persistentDataPath, "*" + extension))
        {
            paths.Add(file);
        }

        return paths;
    }

    public static void LoadGame(string saveDataPath)
    {

    }
}

public enum SaveFileType
{
    Info,
    Data
}