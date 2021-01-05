using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouristsManager : MonoBehaviour
{
    [SerializeField] private TouristScriptableObject[] touristsTemp = null;
    [SerializeField] private GameObject prefab_tourist = null;

    private List<TouristMonoBehaviour> tourists = new List<TouristMonoBehaviour>();
    public List<TouristMonoBehaviour> Tourists => new List<TouristMonoBehaviour>(tourists);
    public int touristsCount => tourists.Count;

    public delegate void TouristAdded(TouristMonoBehaviour touristMono);
    public event TouristAdded OnTouristAdded;

    public delegate void TouristRemoved(TouristMonoBehaviour touristMono);
    public event TouristRemoved OnTouristRemoved;

    public int NumberOfTouristsThatCanBeSpawned => HotelsManager.Instance.AvailableRoomsCount;

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

    private void OnDestroy()
    {
        IslandGenerationPipeline.IslandCompleted -= CreateStarterTourists;
    }

    private void CreateStarterTourists(Vector2Int playerStartingPosition)
    {
        //TODO remove
        /*
        foreach(TouristScriptableObject tourist in touristsTemp)
        {
            TouristInformation info = tourist.TouristInformation;
            CreateTourist(info, new Vector2Int(playerStartingPosition.x, playerStartingPosition.y));
        }
        */
        for (int i = 0; i < 1; i++)
        {
            //CreateTourist(TouristInformation.CreateRandomTouristInformation(), new Vector2Int(playerStartingPosition.x, playerStartingPosition.y));
        }
    }
    
    public void CreateTourist(TouristInformation touristInfo, Vector2 position)
    {
        float depth = DynamicZDepth.GetDynamicZDepth(position, DynamicZDepth.NPC_OFFSET);
        GameObject obj = GameObject.Instantiate(prefab_tourist, new Vector3(position.x, position.y, depth), Quaternion.identity);
        obj.GetComponent<CharacterCustomizationLoader>().LoadCustomization(touristInfo.characterCustomization);
        obj.layer = LayerMask.NameToLayer("Interactable");
        obj.tag = "NPC";

        TouristDialogue dialogue = TouristsGenerator.Instance.GenerateRandomDialogue(touristInfo);
        TouristInterest[] interests = TouristsGenerator.Instance.GetRandomInterestsList();
        TouristHappiness happiness = new TouristHappiness();
        TouristComponents touristComponents = new TouristComponents(touristInfo, obj.transform, dialogue, interests, happiness);

        TouristMonoBehaviour mono = obj.GetComponent<TouristMonoBehaviour>();
        mono.Initialize(touristInfo, touristComponents);

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