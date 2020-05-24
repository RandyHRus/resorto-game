using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatesManager : MonoBehaviour
{
    private IPlayerState currentState;

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

    public void OnSelectedItemChanged(InventoryItem newItem)
    {
        if (newItem == null)
            TrySwitchState(DefaultState.Instance, null);
        else
            TrySwitchState(newItem.onSelectStateInstance, new object[] { newItem });
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


    //TODO come up with better system maybe (looks ok for now tho)
    public void RegionsButtonClicked()
    {
        TrySwitchState(RegionsManager.Instance, null);
    }

    //TODO come up with better system (looks ok for now tho)
    public void TerrainButtonClicked()
    {
        TrySwitchState(TerrainManager.Instance, null);
    }
}
