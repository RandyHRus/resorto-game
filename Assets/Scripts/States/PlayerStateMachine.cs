using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    private PlayerState currentState;

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

        TrySwitchState<DefaultState>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            TrySwitchState<DefaultState>();
        }
        else if (Input.GetButtonDown("Inventory") && currentState != typeToStateInstance[typeof(InventoryState)])
        {
            TrySwitchState<InventoryState>();
        }

        //Movement should go before current state execution
        //Thigns like changing plaeyr direction when fishing started will not work otherwise
        if (currentState.AllowMovement)
        {
            PlayerMovement.Instance.Execute();
        }

        currentState.Execute();
    }

    public PlayerState GetPlayerStateInstance<T>()
    {
        return typeToStateInstance[typeof(T)];
    }

    public void TrySwitchState<T>(object[] args = null) where T: PlayerState
    {
        TrySwitchState(typeof(T), args);    
    }

    public void TrySwitchState(Type type, object[] args = null)
    {
        PlayerState proposedState;
        try
        {
            proposedState = typeToStateInstance[type];
        }
        catch (Exception)
        {
            Debug.LogError("Unknown state!");
            return;
        }

        //Current state will be null when first started
        if (currentState != null)
        {
            if (!currentState.TryEndState())
                return;

            if (currentState.AllowMovement && !proposedState.AllowMovement)
            {
                PlayerMovement.Instance.StopMovement();
            }
        }

        currentState = proposedState;
        currentState.StartState(args);
    }
}
