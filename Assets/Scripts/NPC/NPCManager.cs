using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    [SerializeField] private List<TouristScriptableObject> tourists = null;

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

        IslandGenerator.OnMapLoad += CreateNPCs;
    }

    private void CreateNPCs()
    {
        //TODO remove
        foreach(TouristScriptableObject tourist in tourists)
        {
            tourist.CreateInScene(new Vector2Int(31, 30));
        }
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