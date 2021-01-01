using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatManager : MonoBehaviour
{
    [SerializeField] private GameObject touristBoat = null;

    public InGameTime BoatSpawnTime { get; private set; }
    public InGameTime BoatLeaveTime { get; private set; }

    private TouristBoat touristBoatInstance;

    private static BoatManager _instance;
    public static BoatManager Instance { get { return _instance; } }
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

        BoatLeaveTime = new InGameTime(17, 0);
        BoatSpawnTime = new InGameTime(9, 0);

        touristBoatInstance = Instantiate(touristBoat).GetComponent<TouristBoat>();
        touristBoatInstance.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        touristBoatInstance.OnBoatDespawnPointReached += DespawnTouristBoat;
    }

    private void OnDisable()
    {
        touristBoatInstance.OnBoatDespawnPointReached -= DespawnTouristBoat;
    }

    private void Start()
    {
        TimeManager.Instance.SubscribeToTime(BoatSpawnTime, SpawnTouristBoat);
    }

    private void SpawnTouristBoat(object[] args)
    {
        touristBoatInstance.gameObject.SetActive(true);
        touristBoatInstance.ResetBoat();
    }

    private void DespawnTouristBoat()
    {
        touristBoatInstance.gameObject.SetActive(false);
    }
}
