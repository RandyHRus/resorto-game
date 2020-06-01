using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour
{
    public static float FISH_SEEING_DISTANCE = 3f;
    public static float FISH_SEEING_ANGLE_DOT_PRODUCT = 0.707f; //0.707 is 45 degrees
    public static float FISH_WORLD_WIDTH;

    [SerializeField] private GameObject fishPrefab = null;

    private Queue<GameObject> fishQueue;

    private float timer;
    private int maxFishCount = 10; //This is the max number of fish locations, could be less if location is not water 
    private float lengthBetweenSwitchOut = 5f; //Length between changing location for 1 particle at a time

    private static FishManager _instance;
    public static FishManager Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        fishQueue = new Queue<GameObject>();

        FISH_WORLD_WIDTH = fishPrefab.GetComponent<SpriteRenderer>().sprite.rect.width / 16f;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= lengthBetweenSwitchOut) {
            timer = 0;

            SwitchOutSpawnedFish();
        }
    }

    private void SwitchOutSpawnedFish()
    {
        //Removing a fish. Should not happen during the first 30 initializations
        if (fishQueue.Count >= maxFishCount)
        {
            GameObject fish = fishQueue.Peek();

            if (fish != null)
            {
                fish.GetComponent<FishBehaviour>().TryStartFadeOut();
            }
            fishQueue.Dequeue();
        }
        //Adding new fish 
        {
            float randomX = Random.Range(0f, TileInformationManager.tileCountX - 1);
            float randomY = Random.Range(0f, TileInformationManager.tileCountY - 1);
            Vector2 pos = new Vector2(randomX, randomY);

            TileInformation tileInfo = TileInformationManager.Instance.GetTileInformation(new Vector3Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), 0));

            if (tileInfo.isWater)
            {
                GameObject fish = Instantiate(fishPrefab, pos, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));
                fishQueue.Enqueue(fish);
            }
            else
            {
                fishQueue.Enqueue(null);
            }
        }
    }
}
