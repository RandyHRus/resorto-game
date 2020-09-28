using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    [SerializeField] private TouristScriptableObject[] touristsTemp = null;

    private List<TouristInstance> tourists = new List<TouristInstance>();

    public delegate void TouristAdded(TouristInstance instance);
    public static event TouristAdded OnTouristAdded;

    private static NPCManager _instance;
    public static NPCManager Instance { get { return _instance; } }
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

        IslandGenerationPipeline.IslandCompleted += CreateStarterNPCs;
    }

    private void CreateStarterNPCs(IslandStartingPosition startingPosition)
    {
        //TODO remove
        foreach(TouristScriptableObject tourist in touristsTemp)
        {
            CreateNPC(tourist, new Vector2Int(startingPosition.ActualStartingPosition.x, startingPosition.ActualStartingPosition.y));
        }
    }

    public void CreateNPC(TouristScriptableObject tourist, Vector2 position)
    {
        TouristInstance t = new TouristInstance(tourist, position);
        tourists.Add(t);
        OnTouristAdded?.Invoke(t);
    }
}

public enum RelationshipLevel
{
    Strangers,
    Acquaintance,
    Dislike,
    Friends1,
    Friends2,
    Friends3,
    Intimate,
    Engaged,
    Married
}