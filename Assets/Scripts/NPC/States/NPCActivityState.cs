using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class NPCActivityState : NPCState
{
    [SerializeField] Activity activity = null;

    private float timer;
    private float desiredActivityTime;

    private static readonly float desiredActivityTimeMin = 20f;
    private static readonly float desiredActivityTimeMax = 30f;

    public delegate void ActivityCompleted(Activity activity, float completenessFrac);
    public event ActivityCompleted OnActivityCompleted;

    public NPCActivityState(NPCComponents npcComponents): base(npcComponents) { }

    public override void StartState(object[] args)
    {
        timer = 0;
        desiredActivityTime = Random.Range(desiredActivityTimeMin, desiredActivityTimeMax);
    }

    public override void Execute()
    {
        timer += Time.deltaTime;
    }

    public override void EndState()
    {
        float completeFrac;

        if (timer >= desiredActivityTime)
            completeFrac = 1;
        else
            completeFrac = timer / desiredActivityTime;

        OnActivityCompleted?.Invoke(activity, completeFrac);
    }
}
