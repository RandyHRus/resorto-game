using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Boat : MonoBehaviour
{
    [SerializeField] private RegionInformation boatUnloadingRegionInfo = null;

    private StateMachine<BoatState> stateMachine;

    public delegate void BoatDespawnPointReached();
    public event BoatDespawnPointReached OnBoatDespawnPointReached;

    public delegate void BoatUnloadingPointReached();
    public event BoatUnloadingPointReached OnBoatUnloadingPointReached;

    protected RegionInstance boatUnloadingRegionInstance;

    private void Awake()
    {
        IslandGenerationPipeline.IslandCompleted += (Vector2Int playerStartingPosition) => Initialize();
    }

    public virtual void Initialize()
    {
        boatUnloadingRegionInstance = RegionManager.GetRandomRegionInstanceOfType(boatUnloadingRegionInfo);
        List<Vector2Int> regionPositions = boatUnloadingRegionInstance.GetRegionPositionsAsList();
        int boatTargetXPosition = regionPositions[0].x;

        //Initialize stateMachine
        {
            BoatState[] stateInstances = {
                new BoatMovingToUnloadingPositionState(transform, boatTargetXPosition),
                new BoatStoppedState(transform),
                new BoatLeavingState(transform)
            };

            stateMachine = new StateMachine<BoatState>(typeof(BoatMovingToUnloadingPositionState), stateInstances);
            stateMachine.OnStateChanged += OnStateChanged;
        }
    }

    private void Update()
    {
        stateMachine.RunExecute();
    }

    private void LateUpdate()
    {
        stateMachine.RunLateExecute();
    }

    public virtual void ResetBoat()
    {
        stateMachine.SwitchDefaultState();
    }

    private void OnStateChanged(BoatState previousState, BoatState newState)
    {
        if (previousState != null)
        {
            previousState.OnBoatDespawnPointReached -= InvokeOnBoatSpawnPointReached;
            previousState.OnBoatUnloadingPointReached -= InvokeOnBoatUnloadingPointReached;
        }

        newState.OnBoatDespawnPointReached += InvokeOnBoatSpawnPointReached;
        newState.OnBoatUnloadingPointReached += InvokeOnBoatUnloadingPointReached;
    }

    private void InvokeOnBoatSpawnPointReached()
    {
        OnBoatDespawnPointReached?.Invoke();
    }

    private void InvokeOnBoatUnloadingPointReached()
    {
        OnBoatUnloadingPointReached?.Invoke();
    }
}

public abstract class BoatState : IStateMachineState
{
    public delegate void BoatDespawnPointReached();
    public event BoatDespawnPointReached OnBoatDespawnPointReached;

    public delegate void BoatUnloadingPointReached();
    public event BoatUnloadingPointReached OnBoatUnloadingPointReached;

    protected readonly float maxSpeed = 10f;
    protected readonly int boatAccelerationDistance = 5;

    protected Vector3 boatSpawnPosition = new Vector2(-5, TileInformationManager.mapSize + 2);
    protected Vector3 boatDespawnPosition = new Vector2(TileInformationManager.mapSize + 5, TileInformationManager.mapSize + 2);

    public event ChangeState OnChangeState;
    public virtual event EndState OnEndState;

    protected readonly Transform boatTransform;

    public BoatState(Transform boatTransform)
    {
        this.boatTransform = boatTransform;
    }

    protected void InvokeChangeState(Type stateType, object[] args = null)
    {
        OnChangeState?.Invoke(stateType, args);
    }

    protected void InvokeOnDespawnPointReached()
    {
        OnBoatDespawnPointReached?.Invoke();
    }

    protected void InvokeOnBoatUnloadingPointReached()
    {
        OnBoatUnloadingPointReached?.Invoke();
    }

    public abstract void EndState();

    public abstract void Execute();

    public virtual void LateExecute() { }

    public abstract void StartState(object[] args);
}

public class BoatMovingToUnloadingPositionState : BoatState
{
    private float currentSpeed;
    private int boatTargetXPosition;

    public BoatMovingToUnloadingPositionState(Transform boatTransform, int boatTargetXPosition) : base(boatTransform)
    {
        this.boatTargetXPosition = boatTargetXPosition;
    }

    public override void StartState(object[] args)
    {
        boatTransform.position = boatSpawnPosition;
        currentSpeed = maxSpeed;
    }

    public override void Execute()
    {
        //Decellerate
        if (boatTransform.position.x > boatTargetXPosition - boatAccelerationDistance)
        {
            float speedFrac = (boatTargetXPosition - boatTransform.position.x) / boatAccelerationDistance; //Between 0 and 1
            if (speedFrac < 0)
                speedFrac = 0;

            currentSpeed = speedFrac * maxSpeed; //Will be 0 when distance = 0
        }

        boatTransform.position += new Vector3(Time.deltaTime * currentSpeed, 0);

        if (boatTransform.position.x > boatTargetXPosition-0.1) //0.1 so it doesn't move forever (As speed keeps decreasing)
        {
            boatTransform.position = new Vector3(boatTargetXPosition, boatTransform.position.y);

            InvokeOnBoatUnloadingPointReached();
            InvokeChangeState(typeof(BoatStoppedState), null);
        }
    }

    public override void EndState()
    {
        
    }
}

public class BoatStoppedState : BoatState
{
    public BoatStoppedState(Transform boatTransform) : base(boatTransform) { }

    public override void StartState(object[] args)
    {
        TimeManager.Instance.SubscribeToTime(BoatManager.Instance.BoatLeaveTime, OnBoatLeaveTime);
    }

    public override void Execute()
    {

    }

    public override void EndState()
    {
        TimeManager.Instance.UnsubscribeFromTime(BoatManager.Instance.BoatLeaveTime, OnBoatLeaveTime);
    }

    private void OnBoatLeaveTime(object[] args)
    {
        InvokeChangeState(typeof(BoatLeavingState), null);
    }
}

public class BoatLeavingState: BoatState
{
    private float currentSpeed;

    private float startMovingXPosition;

    public BoatLeavingState(Transform boatTransform) : base(boatTransform) { }

    public override void StartState(object[] args)
    {
        startMovingXPosition = boatTransform.position.x;
        currentSpeed = 0;
    }

    public override void Execute()
    {
        //Accelerate
        if (boatTransform.position.x < startMovingXPosition + boatAccelerationDistance)
        {
            float speedFrac = Mathf.Abs(boatTransform.position.x - startMovingXPosition) / boatAccelerationDistance; //Between 0 and 1
            if (speedFrac > 1)
                speedFrac = 1;
            else if (speedFrac < 0.05f)
                speedFrac = 0.05f;

            currentSpeed = speedFrac * maxSpeed; //Will be 0 when distance = 0
        }
        else
        {
            currentSpeed = maxSpeed;
        }

        boatTransform.position += new Vector3(Time.deltaTime * currentSpeed, 0);

        if (boatTransform.position.x > boatDespawnPosition.x)
            InvokeOnDespawnPointReached();
    }

    public override void EndState()
    {
    }
}