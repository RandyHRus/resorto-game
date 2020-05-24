using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolInformationManager : MonoBehaviour
{
    [SerializeField] private ToolInformation[] toolInformation = null;
    public Dictionary<int, ToolInformation> informationMap { get; private set; }
    private Dictionary<ToolState, IPlayerState> toolStatesMap = null;

    private static ToolInformationManager _instance;
    public static ToolInformationManager Instance { get { return _instance; } }

    private void Awake()
    {
        //Singleton
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
        {
            //Initialize dictionary
            informationMap = new Dictionary<int, ToolInformation>();
            foreach (ToolInformation info in toolInformation)
            {
                informationMap.Add(info.id, info);
            }
        }
    }

    public IPlayerState GetToolState(ToolState toolState)
    {
        if (toolStatesMap == null)
        {
            toolStatesMap = new Dictionary<ToolState, IPlayerState>()
            {
                { ToolState.breakMode, RemoveObjectsState.Instance },
                { ToolState.fishing, FishingState.Instance }
            };
        }

        if (toolStatesMap.TryGetValue(toolState, out IPlayerState stateInstance))
            return stateInstance;
        else
        {
            Debug.Log("No state found for toolState: " + toolState.ToString());
            return null;
        }
    }
}

[System.Serializable]
public class ToolInformation
{
    public int id;
    public string name;
    public ToolState toolState;
    public Sprite icon;
}

//Add IPlayerInstance states in toolStatesMap
public enum ToolState
{
    breakMode,
    fishing
}