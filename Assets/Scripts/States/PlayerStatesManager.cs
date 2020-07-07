using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatesManager : MonoBehaviour
{
    private IPlayerState currentState;

    private Dictionary<ToolState, IPlayerState> toolStatesMap = null;

    private static PlayerStatesManager _instance;
    public static PlayerStatesManager Instance { get { return _instance; } }
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
        currentState = DefaultState.Instance;

        //Initialize tool States Map
        {
            toolStatesMap = new Dictionary<ToolState, IPlayerState>()
            {
                //{ ToolState.breakMode, RemoveObjectsState.Instance },
                { ToolState.fishing, FishingState.Instance }
            };
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            TrySwitchState(DefaultState.Instance, null);
        }

        //Movement should go before current state execution
        //Thigns like changing plaeyr direction when fishing started will not work otherwise
        if (currentState.AllowMovement)
        {
            PlayerMovement.Instance.Execute();
        }

        currentState.Execute();
    }

    public void OnSelectedItemChanged(InventoryItemInformation newItem)
    {
        if (newItem == null)
            TrySwitchState(DefaultState.Instance, null);
        else
        {
            if (newItem.GetType().Equals(typeof(ToolItemInformation))) {
                TrySwitchState(GetToolState(((ToolItemInformation)newItem).StateWhenHeld), new object[] { newItem });
            }
            else if (newItem.GetType().Equals(typeof(ObjectItemInformation))) {
                TrySwitchState(CreateObjectsState.Instance, new object[] { newItem });
            }
        }
    }

    public void TrySwitchState(IPlayerState proposedState, object[] args)
    {
        if (!currentState.TryEndState())
            return;

        if (currentState.AllowMovement && !proposedState.AllowMovement)
        {
            PlayerMovement.Instance.StopMovement();
        }

        currentState = proposedState;
        currentState.StartState(args);
    }

    public IPlayerState GetToolState(ToolState toolState)
    {
        if (toolStatesMap.TryGetValue(toolState, out IPlayerState stateInstance))
            return stateInstance;
        else
        {
            Debug.Log("No state found for toolState: " + toolState.ToString());
            return null;
        }
    }
}
