using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildlifeManager : MonoBehaviour
{
    [SerializeField] private WildlifeInformation[] commonWildLifeList = null;
    [SerializeField] private WildlifeInformation fishInformation = null;

    [Range(0.0f, 1.0f), SerializeField] private float commonWildLifeDensity = 0f;
    [Range(0.0f, 1.0f), SerializeField] private float fishDensity = 0f;

    private float startleRadius = 2f;

    private WildLifeSpawner[] spawnerList;

    private static WildlifeManager _instance;
    public static WildlifeManager Instance { get { return _instance; } }
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

        MainSceneLoader.OnLoadingComplete += OnLoadingCompleteHandler;
    }

    private void Start()
    {
        PlayerMovement.Instance.PlayerMoved += OnPlayerMovedHandler;
    }

    private void OnDestroy()
    {
        MainSceneLoader.OnLoadingComplete -= OnLoadingCompleteHandler;
        PlayerMovement.Instance.PlayerMoved -= OnPlayerMovedHandler;
    }

    private void OnLoadingCompleteHandler()
    {
        Initialize();
    }

    private void OnPlayerMovedHandler(Vector2 playerPosition, bool slow, Vector2 directionVector)
    {
        StartleNearbyAnimals(playerPosition, slow);
    }

    private void Initialize()
    {
        spawnerList = new WildLifeSpawner[2];
        spawnerList[0] = new WildLifeSpawner(commonWildLifeList, commonWildLifeDensity);
        spawnerList[1] = new WildLifeSpawner(new WildlifeInformation[] { fishInformation }, fishDensity);
    }

    private void Update()
    {
        foreach (WildLifeSpawner s in spawnerList)
        {
            s.Execute();
        }   
    }

    private void StartleNearbyAnimals(Vector2 playerPosition, bool slow)
    {
        //Dont startle if slow walking
        if (slow)
            return;

        Collider2D[] nearbyWildlife = Physics2D.OverlapCircleAll(playerPosition, startleRadius, 1 << LayerMask.NameToLayer("Wildlife"));
        foreach (Collider2D c in nearbyWildlife)
        {
            GameObject wildlife = c.gameObject;

            WildlifeBehaviour behaviour = wildlife.GetComponentInParent<WildlifeBehaviour>();
            if (behaviour == null) behaviour = wildlife.GetComponentInChildren<WildlifeBehaviour>();
            behaviour.Startle();
        }
    }

    private class WildLifeInGame
    {
        public WildlifeInformation Info { get; private set; }
        public WildlifeBehaviour BehaviourScript { get; private set; }

        public WildLifeInGame(WildlifeInformation info, WildlifeBehaviour behaviourScript)
        {
            this.Info = info;
            this.BehaviourScript = behaviourScript;
        }
    }

    private class WildLifeSpawner {

        private Queue<WildLifeInGame> queue = new Queue<WildLifeInGame>();
        private WildlifeInformation[] wildlifeList;
        private int maxCount;
        private float timer;

        private float lifeTimeSecondsLowerBound = 30f;
        private float lifeTimeSecondsUpperBound = 120f;

        private float lengthBetweenNextQueueSwitch;

        public WildLifeSpawner(WildlifeInformation[] wildlifeList, float density)
        {
            this.wildlifeList = wildlifeList;
            maxCount = Mathf.RoundToInt(TileInformationManager.mapSize * TileInformationManager.mapSize * density);
            RestartTimer();

            while (queue.Count < maxCount) {
                if (TrySpawnWildlife(out WildlifeInformation spawnedInfo, out WildlifeBehaviour behaviourScript))
                    queue.Enqueue(new WildLifeInGame(spawnedInfo, behaviourScript));
                else
                    queue.Enqueue(null);
            }
        }

        public void Execute()
        {
            timer += Time.deltaTime;

            if (timer >= lengthBetweenNextQueueSwitch)
            {
                RestartTimer();
                SwitchOut();
            }
        }

        private void RestartTimer()
        {
            timer = 0;
            lengthBetweenNextQueueSwitch = Random.Range(lifeTimeSecondsLowerBound, lifeTimeSecondsUpperBound) / maxCount;
        }

        private void SwitchOut()
        {
            WildLifeInGame toDespawn = queue.Peek();
            if (toDespawn?.BehaviourScript != null)
            {
                toDespawn.BehaviourScript.Despawn();
            }
            queue.Dequeue();

            while (queue.Count < maxCount)
            {
                if (TrySpawnWildlife(out WildlifeInformation spawnedInfo, out WildlifeBehaviour behaviourScript))
                    queue.Enqueue(new WildLifeInGame(spawnedInfo, behaviourScript));
                else
                    queue.Enqueue(null);
            }
        }

        private bool TrySpawnWildlife(out WildlifeInformation spawnedInfo, out WildlifeBehaviour behaviourScript)
        {
            WildlifeInformation randomWildlife = wildlifeList[Random.Range(0, wildlifeList.Length)];

            float randomX = Random.Range(0f, TileInformationManager.mapSize - 1);
            float randomY = Random.Range(0f, TileInformationManager.mapSize - 1);
            Vector2 pos = new Vector2(randomX, randomY);

            spawnedInfo = randomWildlife;
            return randomWildlife.TrySpawn(pos, out behaviourScript);
        }
    }
}
