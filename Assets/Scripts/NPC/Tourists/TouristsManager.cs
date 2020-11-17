using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouristsManager : MonoBehaviour
{
    [SerializeField] private TouristScriptableObject[] touristsTemp = null;

    private List<TouristMonoBehaviour> tourists = new List<TouristMonoBehaviour>();
    public int touristsCount => tourists.Count;

    public delegate void TouristAdded(TouristMonoBehaviour touristMono);
    public static event TouristAdded OnTouristAdded;

    public delegate void TouristRemoved(TouristMonoBehaviour touristMono);
    public static event TouristRemoved OnTouristRemoved;

    public int NumberOfTouristsThatCanBeSpawned => HotelsManager.AvailableRoomsCount;

    private static TouristsManager _instance;
    public static TouristsManager Instance { get { return _instance; } }
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

        IslandGenerationPipeline.IslandCompleted += CreateStarterTourists;
    }

    private void CreateStarterTourists(Vector2Int playerStartingPosition)
    {
        //TODO remove
        foreach(TouristScriptableObject tourist in touristsTemp)
        {
            TouristInformation info = tourist.TouristInformation;
            CreateTourist(info, new Vector2Int(playerStartingPosition.x, playerStartingPosition.y));
        }

        for (int i = 0; i < 3; i++)
        {
            CreateTourist(TouristInformation.CreateRandomTouristInformation(), new Vector2Int(playerStartingPosition.x, playerStartingPosition.y));
        }
    }

    public void CreateTourist(TouristInformation touristInfo, Vector2 position)
    {
        TouristMonoBehaviour mono = (TouristMonoBehaviour)touristInfo.CreateInScene(position);
        tourists.Add(mono);
        OnTouristAdded?.Invoke(mono);

        mono.OnTouristDeleting += RemoveTourist;
    }

    public void RemoveTourist(TouristMonoBehaviour mono)
    {
        mono.OnTouristDeleting -= RemoveTourist;
        tourists.Remove(mono);
        OnTouristRemoved?.Invoke(mono);
    }
}