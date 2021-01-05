using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Threading.Tasks;

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
        string saveInfoJson = JsonConvert.SerializeObject(saveInfo);

        SaveData saveData = CreateSaveData();
        string saveDataJson = JsonConvert.SerializeObject(saveData);

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

    private static SaveData CreateSaveData()
    {
        PlayerData playerData = CreatePlayerData();
        TouristsData touristsData = CreateTouristsData();
        MapData mapData = CreateMapData();
        return new SaveData(playerData, touristsData, mapData);
    }

    public static void LoadSaveData(string saveDataPath)
    {
        SaveData saveData = JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(saveDataPath));
        LoadPlayerData(saveData.PlayerData);
        LoadTouristsData(saveData.TouristsData);
        LoadMapData(saveData.MapData);
    }

    private static PlayerData CreatePlayerData()
    {
        CharacterCustomization playerCharacterCustomization = PlayerCustomization.Character;
        string playerName = PlayerCustomization.PlayerName;
        Vector2 playerPosition = PlayerMovement.Instance.transform.position;
        PlayerData playerData = new PlayerData(playerCharacterCustomization, playerPosition, playerName);
        return playerData;
    }

    private static void LoadPlayerData(PlayerData playerData)
    {
        PlayerCustomization.Character = playerData.CharacterCustomization;
        PlayerMovement.Instance.transform.position = playerData.Position;
    }

    private static TouristsData CreateTouristsData()
    {
        TouristsData.TouristData[] touristsData = new TouristsData.TouristData[TouristsManager.Instance.touristsCount];
        List<TouristMonoBehaviour> Tourists = TouristsManager.Instance.Tourists;
        for (int i = 0; i < TouristsManager.Instance.touristsCount; i++)
        {
            TouristMonoBehaviour mono = Tourists[i];
            touristsData[i] = new TouristsData.TouristData(mono.TouristInformation, mono.transform.position, mono.TouristComponents.happiness.Value);
        }
        return new TouristsData(touristsData);
    }

    private static void LoadTouristsData(TouristsData touristsData)
    {

    }

    private static MapData CreateMapData()
    {
        string islandName = "#PlaceHolder#";

        int mapSize = TileInformationManager.mapSize;
        MapData.TileData[,] tilesData = new MapData.TileData[mapSize, mapSize];
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                TileInformationManager.Instance.TryGetTileInformation(new Vector2Int(i, j), out TileInformation tileInfo);
                int layerNum = tileInfo.layerNum;

                //Need to check if water because 0 could mean either sand or water
                if (layerNum == 0)
                {
                    if (TileLocation.Water.HasFlag(tileInfo.tileLocation))
                        layerNum = -1;
                }

                tilesData[i, j] = new MapData.TileData(layerNum);
            }
        }

        return new MapData(islandName, tilesData);
    }

    private static void LoadMapData(MapData mapData)
    {
        //Create terrain
        {
            int mapSize = TileInformationManager.mapSize;
            int[,] layerHeightMap = new int[mapSize, mapSize];
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    layerHeightMap[i, j] = mapData.TilesData[i, j].LayerNum;
                }
            }
            IslandTerrainGenerator.Instance.GenerateIslandTerrain(layerHeightMap);
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
}

public enum SaveFileType
{
    Info,
    Data
}