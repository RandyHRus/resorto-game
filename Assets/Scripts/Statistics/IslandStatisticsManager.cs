using System.Collections;
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

        NPCManager.OnTouristAdded += (TouristInstance instance) => GetStatistic(StatisticInstance.NumberOfTourists).Add(1);
    }

    public Statistic GetStatistic(StatisticInstance type)
    {
        return statistics[type];
    }
}

public enum StatisticInstance
{
    NumberOfTourists,
    IslandRating
}