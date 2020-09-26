using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ReachIslandStatisticSubtask : Subtask
{
    [SerializeField] private StatisticInstance statistic = 0;

    [SerializeField] private int target = 0;

    public override void Initialize()
    {
        Statistic statisticInstance = IslandStatisticsManager.Instance.GetStatistic(statistic);
        statisticInstance.OnValueChanged += (int number) => {
            if (number >= target)
                SignalSubtaskCompleted();
        };
    }
}
