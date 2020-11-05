using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    [SerializeField] private TouristScriptableObject[] touristsTemp = null;

    private List<TouristMonoBehaviour> tourists = new List<TouristMonoBehaviour>();

    public delegate void TouristAdded(TouristMonoBehaviour touristMono);
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

    private void CreateStarterNPCs(Vector2Int playerStartingPosition)
    {
        //TODO remove
        foreach(TouristScriptableObject tourist in touristsTemp)
        {
            TouristInformation info = tourist.TouristInformation;
            CreateTourist(info, new Vector2Int(playerStartingPosition.x, playerStartingPosition.y));
        }

        for (int i = 0; i < 0; i++)
        {
            CreateTourist(TouristInformation.CreateRandomTouristInformation(), new Vector2Int(playerStartingPosition.x, playerStartingPosition.y));
        }
    }

    public void CreateTourist(TouristInformation touristInfo, Vector2 position)
    {
        TouristMonoBehaviour t = (TouristMonoBehaviour)touristInfo.CreateInScene(position);
        tourists.Add(t);
        OnTouristAdded?.Invoke(t);
    }
}