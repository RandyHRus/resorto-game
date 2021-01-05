using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class SaveData
{
    public PlayerData PlayerData { get; private set;}
    public TouristsData TouristsData { get; private set; }
    public MapData MapData { get; private set; }

    public SaveData(PlayerData playerData, TouristsData touristsData, MapData mapData)
    {
        this.PlayerData = playerData;
        this.TouristsData = touristsData;
        this.MapData = mapData;
    }
}

[System.Serializable]
public class PlayerData
{
    public CharacterCustomization CharacterCustomization { get; private set; }
    public string PlayerName { get; private set; }
    public Vector2 Position { get; private set; }

    public PlayerData(CharacterCustomization characterCustomization, Vector2 position, string playerName)
    {
        this.CharacterCustomization = characterCustomization;
        this.PlayerName = playerName;
        this.Position = position;
    }
}

[System.Serializable]
public class TouristsData
{
    public TouristData[] TouristsData_ { get; private set; }

    public TouristsData(TouristData[] touristsData)
    {
        this.TouristsData_ = touristsData;
    }


    [System.Serializable]
    public class TouristData
    {
        public TouristInformation TouristInformation { get; private set; }
        public Vector2 Position { get; private set; }
        public int Happiness { get; private set; }
        //TODO: interests, states, etc.

        public TouristData(TouristInformation touristInformation, Vector2 position, int happiness)
        {
            this.TouristInformation = touristInformation;
            this.Position = position;
            this.Happiness = happiness;

        }
    }
}

[System.Serializable]
public class MapData
{
    public string IslandName { get; private set; }
    public TileData[,] TilesData { get; private set; }
    public RegionData[] RegionsData { get; private set; }

    public MapData(string islandName, TileData[,] tilesData)
    {
        this.IslandName = islandName;
        this.TilesData = tilesData;
    }

    [System.Serializable]
    public class TileData
    {
        public int LayerNum { get; private set; }

        public TileData(int layerNum)
        {
            this.LayerNum = layerNum;
        }
    }

    [System.Serializable]
    public class RegionData
    {
        public Vector2Int[] Positions { get; private set; }
        public RegionInformation RegionInfo { get; private set; }

        public RegionData()
        {

        }
    }
}