using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainState : MonoBehaviour, IPlayerState
{
    [SerializeField] private Sprite landIndicatorSprite = null;
    [SerializeField] private Sprite sandIndicatorSprite = null;
    private bool coroutineRunning = false;
    private GameObject indicator;
    private SpriteRenderer indicatorRenderer;

    private static TerrainState _instance;
    public static TerrainState Instance { get { return _instance; } }
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
        //Setup indicator
        {
            indicator = new GameObject("Terrain Indicator");
            indicatorRenderer = indicator.AddComponent<SpriteRenderer>();
            indicatorRenderer.sortingLayerName = "Indicator";
            indicator.SetActive(false);
        }
    }

    public bool AllowMovement
    {
        get { return true; }
    }

    public void StartState(object[] args)
    {
        indicator.SetActive(true);
    }

    public bool TryEndState()
    {
        if (coroutineRunning)
        {
            return false;
        }
        else
        {
            indicator.SetActive(false);
            return true;
        }
    }

    public void Execute()
    {
        if (coroutineRunning)
            return;

        Vector3Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();

        indicator.transform.position = new Vector2(mouseTilePosition.x, mouseTilePosition.y);
        indicatorRenderer.sprite = landIndicatorSprite;
        indicatorRenderer.color = ResourceManager.Instance.Red;

        {
            if (TerrainManager.Instance.TerrainRemoveable(mouseTilePosition, out int layerNumber))
            {
                indicatorRenderer.color = ResourceManager.Instance.Yellow;

                if (Input.GetButtonDown("Secondary"))
                {
                    if (layerNumber == 0)
                    {
                        StartCoroutine(RemoveSand());
                        return;
                    }
                    else if (layerNumber >= 1)
                    {
                        StartCoroutine(RemoveLand(layerNumber));
                        return;
                    }
                }
            }
        }
        {
            if (TerrainManager.Instance.TerrainPlaceable(mouseTilePosition, out int layerNumber))
            {
                indicatorRenderer.color = ResourceManager.Instance.Green;

                if (layerNumber == 0)
                    indicatorRenderer.sprite = sandIndicatorSprite;

                if (Input.GetButtonDown("Primary"))
                {
                    if (layerNumber == 0)
                    {
                        StartCoroutine(PlaceSand());
                        return;
                    }
                    else if (layerNumber >= 1)
                    {
                        StartCoroutine(PlaceLand(layerNumber));
                        return;
                    }
                }
            }
        }
    }

    IEnumerator PlaceSand()
    {
        coroutineRunning = true;
        indicatorRenderer.sprite = null;

        Vector3Int previousTilePosition = new Vector3Int(-1, -1, -1);

        while (Input.GetButton("Primary"))
        {
            Vector3Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();

            if (mouseTilePosition != previousTilePosition)
            {
                previousTilePosition = mouseTilePosition;
                if (TerrainManager.Instance.TryCreateSand(mouseTilePosition))
                {
                    ParticlesManager.Instance.PlaySmokeEffect(mouseTilePosition);
                }
            }

            yield return 0;
        }
        coroutineRunning = false;
    }

    IEnumerator RemoveSand()
    {
        coroutineRunning = true;
        indicatorRenderer.sprite = null;
        Vector3Int previousTilePosition = new Vector3Int(-1, -1, -1);

        while (Input.GetButton("Secondary"))
        {
            Vector3Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();

            if (mouseTilePosition != previousTilePosition)
            {
                previousTilePosition = mouseTilePosition;
                if (TerrainManager.Instance.TryRemoveSand(mouseTilePosition))
                {
                    ParticlesManager.Instance.PlaySmokeEffect(mouseTilePosition);
                }
            }

            yield return 0;
        }
        coroutineRunning = false;
    }

    IEnumerator PlaceLand(int layerNumber)
    {
        coroutineRunning = true;
        indicatorRenderer.sprite = null;
        Vector3Int previousTilePosition = new Vector3Int(-1, -1, -1);

        while (Input.GetButton("Primary"))
        {
            Vector3Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();

            if (mouseTilePosition != previousTilePosition)
            {
                previousTilePosition = mouseTilePosition;
                if (TerrainManager.Instance.TryCreateLand(mouseTilePosition, layerNumber))
                {
                    ParticlesManager.Instance.PlaySmokeEffect(mouseTilePosition);
                }
            }

            yield return 0;
        }
        coroutineRunning = false;
    }

    IEnumerator RemoveLand(int layerNumber)
    {
        coroutineRunning = true;
        indicatorRenderer.sprite = null;
        Vector3Int previousTilePosition = new Vector3Int(-1, -1, -1);

        while (Input.GetButton("Secondary"))
        {
            Vector3Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();

            if (mouseTilePosition != previousTilePosition)
            {
                previousTilePosition = mouseTilePosition;
                if (TerrainManager.Instance.TryRemoveLand(mouseTilePosition, layerNumber))
                {
                    ParticlesManager.Instance.PlaySmokeEffect(mouseTilePosition);
                }
            }

            yield return 0;
        }
        coroutineRunning = false;
    }
}
