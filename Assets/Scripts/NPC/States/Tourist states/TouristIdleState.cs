using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tourist States/Idle")]
public class TouristIdleState : NPCIdleState
{
    private readonly float minActivityTimer = 3f;
    private readonly float maxActivityTimer = 5f;

    private float nextActivityTimer = 0;

    private TouristInstance touristInstance => (TouristInstance)npcInstance;

    public override void StartState(object[] args)
    {
        base.StartState(args);
        nextActivityTimer = Random.Range(minActivityTimer, maxActivityTimer);
    }

    public override void Execute()
    {
        base.Execute();

        nextActivityTimer -= Time.deltaTime;
        if (nextActivityTimer <= 0)
        {
            nextActivityTimer = Random.Range(minActivityTimer, maxActivityTimer);
            TouristInterest randomInterest = touristInstance.interests[Random.Range(0, touristInstance.interests.Length)];
            Activity randomActivity = randomInterest.Activies[Random.Range(0, randomInterest.Activies.Length)];
            InvokeStartActivity(randomActivity);

            return;
        }
    }
}
