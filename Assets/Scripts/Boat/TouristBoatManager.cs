using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouristBoatManager : MonoBehaviour
{
    [SerializeField] private GameObject touristBoat = null;

    private TouristBoat boatInstance;

    private void Awake()
    {
        boatInstance = Instantiate(touristBoat).GetComponent<TouristBoat>();
        boatInstance.gameObject.SetActive(false);
        boatInstance.OnBoatDespawnPointReached += () => boatInstance.gameObject.SetActive(false);
    }

    private void Start()
    {
        TimeManager.OnTurnedMidDay += SpawnTouristBoat;
    }

    void SpawnTouristBoat()
    {
        boatInstance.gameObject.SetActive(true);
        boatInstance.ResetBoat();
    }
}
