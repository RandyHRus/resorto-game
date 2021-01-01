using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class SaveData
{
    [SerializeField] private PlayerData playerData = null;
    public PlayerData PlayerData_ => playerData;

    [SerializeField] private TouristsData touristsData = null;
    public TouristsData TouristsData_ => touristsData;

    [SerializeField] private MapData mapData = null;
    public MapData MapData_ => mapData;

    public SaveData()
    {
        playerData = new PlayerData();
        touristsData = new TouristsData();
        mapData = new MapData();
    }

    [System.Serializable]
    public class PlayerData
    {
        [SerializeField] private CharacterCustomization characterCustomization = null;
        public CharacterCustomization CharacterCustomization => characterCustomization;

        [SerializeField] private Vector2 position = default;
        public Vector2 Position => position;

        public PlayerData()
        {
            characterCustomization = PlayerCustomization.Character;
            position = PlayerMovement.Instance.transform.position;
        }
    }

    [System.Serializable]
    public class TouristsData
    {
        [SerializeField] private TouristData[] touristsData = null;
        public TouristData[] TouristsData_ => touristsData;

        public TouristsData()
        {
            touristsData = new TouristData[TouristsManager.Instance.touristsCount];
            List<TouristMonoBehaviour> Tourists = TouristsManager.Instance.Tourists;
            for (int i = 0; i < TouristsManager.Instance.touristsCount; i++)
            {
                touristsData[i] = new TouristData(Tourists[i]);
            }
        }


        [System.Serializable]
        public class TouristData
        {
            [SerializeField] private TouristInformation touristInformation = null;
            public TouristInformation TouristInformation => touristInformation;

            [SerializeField] private Vector2 position = default;
            public Vector2 Position => position;

            [SerializeField] private int happiness = default;
            public int Happiness => happiness;
            //TODO: interests, states, etc.

            public TouristData(TouristMonoBehaviour touristMono)
            {
                touristInformation = touristMono.TouristInformation;
                position = touristMono.transform.position;
                happiness = touristMono.TouristComponents.happiness.Value;

            }
        }
    }

    [System.Serializable]
    public class MapData
    {
        [SerializeField] private string islandName = default;
        public string IslandName => islandName;

        [SerializeField] private TileData[,] tilesData = null;
        public TileData[,] TilesData => tilesData;

        [SerializeField] private RegionData[] regionsData = null;
        public RegionData[] RegionsData => regionsData;

        public MapData()
        {
            islandName = "#PlaceHolder#";
        }

        [System.Serializable]
        public class TileData
        {
            [SerializeField] int layerNum = default;
            public int LayerNum => layerNum;
        }

        [System.Serializable]
        public class RegionData
        {
            [SerializeField] private Vector2Int[] positions = null;
            public Vector2Int[] Positions => positions;

            [SerializeField] private RegionInformation regionInfo = null;
            public RegionInformation RegionInfo => regionInfo;

            public RegionData()
            {

            }
        }
    }
}
