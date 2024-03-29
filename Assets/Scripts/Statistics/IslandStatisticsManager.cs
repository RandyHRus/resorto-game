﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandStatisticsManager: MonoBehaviour
{
    [EnumNamedArray(typeof(StatisticInstance)), SerializeField] string[] statisticsToCreateToName = null;

    private Dictionary<StatisticInstance, Statistic> statistics = new Dictionary<StatisticInstance, Statistic>();

    private static IslandStatisticsManager _instance;
    public static IslandStatisticsManager Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        for (int i = 0; i < statisticsToCreateToName.Length; i++)
        {
            statistics[(StatisticInstance)i] = new Statistic(statisticsToCreateToName[i]);
        }
    }

    private void Start()
    {
        TouristsManager.Instance.OnTouristAdded += OnTouristAddedHandler;
        TouristsManager.Instance.OnTouristRemoved += OnTouristRemovedHandler;
        HotelsManager.Instance.OnValidRoomsCountChanged += OnValidRoomCountChangedHandler;
        HotelsManager.Instance.OnAvailableRoomsCountChanged += OnAvailableRoomsCountChangedHandler;
    }

    private void OnDestroy()
    {
        TouristsManager.Instance.OnTouristAdded -= OnTouristAddedHandler;
        TouristsManager.Instance.OnTouristRemoved -= OnTouristRemovedHandler;
        HotelsManager.Instance.OnValidRoomsCountChanged -= OnValidRoomCountChangedHandler;
        HotelsManager.Instance.OnAvailableRoomsCountChanged -= OnAvailableRoomsCountChangedHandler;
    }

    private void OnTouristAddedHandler(TouristMonoBehaviour touristMono)
    {
        GetStatistic(StatisticInstance.NumberOfTourists).Set(TouristsManager.Instance.touristsCount);
    }

    private void OnTouristRemovedHandler(TouristMonoBehaviour touristMono)
    {
        GetStatistic(StatisticInstance.NumberOfTourists).Set(TouristsManager.Instance.touristsCount);
    }

    private void OnValidRoomCountChangedHandler(int newCount)
    {
        GetStatistic(StatisticInstance.NumberOfValidRooms).Set(newCount);
    }

    private void OnAvailableRoomsCountChangedHandler(int newCount)
    {
        GetStatistic(StatisticInstance.NumberOfAvailableRooms).Set(newCount);
    }

    public Statistic GetStatistic(StatisticInstance type)
    {
        return statistics[type];
    }
}

public enum StatisticInstance
{
    NumberOfTourists,
    IslandRating,
    NumberOfValidRooms,
    NumberOfAvailableRooms
}