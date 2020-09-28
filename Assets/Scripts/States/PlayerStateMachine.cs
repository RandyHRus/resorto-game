using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    private PlayerState currentState;
    private PlayerState comebackState;

    private static PlayerStateMachine _instance;
    public static PlayerStateMachine Instance { get { return _instance; } }
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

    [SerializeField] private PlayerState[] playerStates = null;
    Dictionary<Type, PlayerState> typeToStateInstance = new Dictionary<Type, PlayerState>();

    private void Start()
    {
        foreach (PlayerState state in playerStates)
        {
            Type stateType = state.GetType();
            PlayerState stateInstance = Instantiate(state);
            stateInstance.Initialize();
            typeToStateInstance.Add(stateType, stateInstance);
        }

        SwitchState<DefaultState>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            SwitchState<DefaultState>();
        }
        else if (Input.GetButtonDown("Inventory") && currentState != typeToStateInstance[typeof(UIState)])
        {
            SwitchState<UIState>();
        }

        //Movement should go before current state execution
        //Thigns like changing plaeyr direction when fishing started will not work otherwise
        if (currentState.AllowMovement)
        {
            PlayerMovement.Instance.Execute();
        }

        currentState.Execute();
    }

    private void LateUpdate()
    {
        currentState.LateExecute();
    }

    public PlayerState GetPlayerStateInstance<T>()
    {
        return typeToStateInstance[typeof(T)];
    }

    public void SwitchStateTemporary<T>(object[] args = null) where T: PlayerState
    {
        comebackState = currentState;
        SwitchState<T>();
    }

    public void EndCurrentState()
    {

    }

    public void SwitchState<T>(object[] args = null) where T: PlayerState
    {
        SwitchState(typeof(T), args);    
    }

    public void SwitchState(Type type, object[] args = null)
    {
        PlayerState proposedState;
        try
        {
            proposedState = typeToStateInstance[type];
        }
        catch (Exception)
        {
            throw new System.Exception("Unknown state!");
        }

        //Current state will be null when first started
        if (currentState != null)
        {
            currentState.EndState();

            if (currentState.AllowMovement && !proposedState.AllowMovement)
            {
                PlayerMovement.Instance.StopMovement();
            }
        }

        currentState = proposedState;
        currentState.StartState(args);
    }
}
