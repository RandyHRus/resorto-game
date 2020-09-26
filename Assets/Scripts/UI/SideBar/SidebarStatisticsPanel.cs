using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidebarStatisticsPanel : SidebarPanel
{
    [SerializeField] StatisticInstance[] statisticsToShow = null;

    private void Start()
    {
        ComponentsListPanel<StatisticComponentUI> statisticsList = new ComponentsListPanel<StatisticComponentUI>(transform.Find("Statistics list").gameObject);

        foreach (StatisticInstance i in statisticsToShow)
        {
            statisticsList.InsertListComponent(new StatisticComponentUI(IslandStatisticsManager.Instance.GetStatistic(i), statisticsList.ObjectInScene.transform));
        }
    }
}
