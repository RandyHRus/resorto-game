﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerStateMachineManager : MonoBehaviour
{
    [SerializeField] private PlayerState defaultState = null;

    [SerializeField] private PlayerState[] playerStates = null;
    private StateMachine<PlayerState> stateMachine;

    private static PlayerStateMachineManager _instance;
    public static PlayerStateMachineManager Instance { get { return _instance; } }
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
    }

    private void Start()
    {
        PlayerState[] stateInstancesCopy = new PlayerState[playerStates.Length];
        for (int i = 0; i < playerStates.Length; i++)
        {
            PlayerState copy = Instantiate(playerStates[i]);
            copy.Initialize();
            stateInstancesCopy[i] = copy;
        }

        stateMachine = new StateMachineWithDefaultState<PlayerState>(defaultState.GetType(), stateInstancesCopy);

        SwitchState<DefaultState>();

        stateMachine.OnStateChanged += OnStateChanged;

        CameraFollow.Instance.ChangeFollowTarget(ResourceManager.Instance.Player);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (!(stateMachine.CurrentState is DefaultState))
            {
                stateMachine.CurrentState.OnCancelButtonPressed();
            }
            else
            {
                UIManager.Instance.CloseAllUI(false);
            }
        }
        else if (Input.GetButtonDown("ToggleUI"))
        {
            if (!UIManager.Instance.CloseAllUI(true))
            {
                UIManager.Instance.ReloadSavedUI();
            }
        }

        //Movement should go before current state execution
        //Things like changing player direction when fishing started will not work otherwise
        if (stateMachine.CurrentState.AllowMovement)
        {
            PlayerMovement.Instance.Execute();
        }
        if (stateMachine.CurrentState.AllowMouseDirectionChange)
        {
            PlayerDirection.Instance.Execute();
        }

        stateMachine.RunExecute();
    }

    private void LateUpdate()
    {
        //Camera stuff should go in late execute because player (And other follow targets) will be moved in regular update
        switch (stateMachine.CurrentState.CameraMode)
        {
            case (CameraMode.Follow):
                CameraFollow.Instance.ExecuteLateUpdate();
                break;
            case (CameraMode.Drag):
                CameraDrag.Instance.ExecuteLateUpdate();
                break;
            default:
                throw new System.NotImplementedException();
        }

        stateMachine.RunLateExecute();
    }

    public void SwitchState<T>(object[] args = null) where T: PlayerState => stateMachine.SwitchState<T>(args);

    public void SwitchState(Type type, object[] args = null) => stateMachine.SwitchState(type, args);

    public void EndCurrentState() => stateMachine.EndCurrentState();

    public PlayerState CurrentState => stateMachine.CurrentState;


    private void OnStateChanged(PlayerState previousState, PlayerState newState)
    {
        if (previousState != null && previousState.AllowMovement && !newState.AllowMovement)
        {
            PlayerMovement.Instance.StopMovement();
        }
    }
}
