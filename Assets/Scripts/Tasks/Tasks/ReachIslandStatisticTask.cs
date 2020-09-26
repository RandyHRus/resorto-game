using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Task/ReachStatistic")]
public class ReachIslandStatisticTask : Task
{
    [SerializeField] ReachIslandStatisticSubtask reachStatistic = null;

    public override List<Subtask> SubTasks
    {
        get
        {
            return new List<Subtask>
            {
                reachStatistic
            };
        }
    }
}
