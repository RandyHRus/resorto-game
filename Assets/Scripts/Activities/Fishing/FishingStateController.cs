using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FishingStateController
{
    private Dictionary<Type, FishingPhase> typeToStateInstance;
    public FishingPhase CurrentPhase { get; private set; }

    private FishingResources resources;
    private Animator animator;

    public delegate void FishingEnd();
    public event FishingEnd OnFishingEnd;

    public FishingStateController(FishingDefaultPhase defaultPhase, FishingChargingPhase chargingPhase, FishingCastingPhase castingPhase,
            FishingBobbingPhase bobbingPhase, FishingHookedPhase hookedPhase, FishingResources resources)
    {
        this.resources = resources;
        this.animator = resources.animator;

        //Initialize states
        typeToStateInstance = new Dictionary<Type, FishingPhase>()
        {
            { typeof(FishingDefaultPhase),  defaultPhase },
            { typeof(FishingChargingPhase), chargingPhase },
            { typeof(FishingCastingPhase),  castingPhase },
            { typeof(FishingBobbingPhase),  bobbingPhase },
            { typeof(FishingHookedPhase),   hookedPhase }
        };
    }

    public void Execute()
    {
        CurrentPhase.Execute();
    }

    public void LateExecute()
    {
        CurrentPhase.LateExecute();
    }

    public void StartState()
    {
        SwitchPhase<FishingDefaultPhase>();
        animator.SetLayerWeight(animator.GetLayerIndex("Fishing"), 1);
    }

    public void EndState()
    {
        ResetFishing();

        CurrentPhase.EndState();
        CurrentPhase = null;
        animator.SetLayerWeight(animator.GetLayerIndex("Fishing"), 0);
    }

    public void SwitchPhase<T>(object[] args = null) where T: FishingPhase
    {
        SwitchPhase(typeof(T), args);
    }

    //Switch to null to end fishing
    public void SwitchPhase(Type t, object[] args = null)
    {
        if (CurrentPhase != null)
        {
            CurrentPhase.OnChangePhase -= SwitchPhase;
            CurrentPhase.EndState();
        }

        if (t == null)
        {
            OnFishingEnd?.Invoke();
            return;
        }
        else if (t.IsAssignableFrom(typeof(FishingDefaultPhase)))
        {
            ResetFishing();
        }

        CurrentPhase = typeToStateInstance[t];
        CurrentPhase.OnChangePhase += SwitchPhase;
        CurrentPhase.StartState(args);
    }

    private void ResetFishing()
    {
        animator.SetBool("LineOut", false);
        animator.SetBool("LineCastFinished", false);
        animator.SetBool("Hooked", false);
        resources.fishinglineInstance.SetActive(false);
    }
}