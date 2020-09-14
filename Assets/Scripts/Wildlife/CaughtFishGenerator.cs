using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaughtFishGenerator: MonoBehaviour
{
    [SerializeField] private FishItemInformation[] allFish = null;

    private static CaughtFishGenerator _instance;
    public static CaughtFishGenerator Instance { get { return _instance; } }
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
    }

    public void Update()
    {
        GetRandomFish();
    }

    public FishItemInstance GetRandomFish()
    {
        //TODO: Fish depending on weather/time etc
        FishItemInformation chosenFish = allFish[Random.Range(0, allFish.Length)];
        float length = RandomFromDistribution.RandomRangeExponential(chosenFish.MillimetresLowerBound, chosenFish.MillimetresUpperBound, 1, RandomFromDistribution.Direction_e.Left);
        return new FishItemInstance(chosenFish, Mathf.RoundToInt(length));
    }
}
