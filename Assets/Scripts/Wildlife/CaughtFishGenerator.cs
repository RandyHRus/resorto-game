using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class CaughtFishGenerator: MonoBehaviour
{
    [SerializeField] private AssetReference[] allFish = null;

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
    }

    public FishItemInstance GetRandomFish()
    {
        //TODO: Fish depending on weather/time etc
        AssetReference chosenFish = allFish[Random.Range(0, allFish.Length)];
        FishItemInformation fishInfo = AssetsManager.GetAsset<FishItemInformation>(chosenFish);

        float length = RandomFromDistribution.RandomRangeExponential(fishInfo.MillimetresLowerBound, fishInfo.MillimetresUpperBound, 1, RandomFromDistribution.Direction_e.Left);
        return new FishItemInstance(chosenFish, Mathf.RoundToInt(length));
    }
}
