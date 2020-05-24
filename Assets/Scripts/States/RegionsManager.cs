using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RegionsManager : MonoBehaviour, IPlayerState
{
    [SerializeField] private Tilemap showRegionTilemap = null;
    [SerializeField] private Tile showRegionTile = null;
    [SerializeField] private Sprite regionIndicatorSprite = null;

    private GameObject indicator;
    private SpriteRenderer indicatorRenderer;

    private bool coroutineRunning = false;

    private RegionInformation selectedRegion;


    private static RegionsManager _instance;
    public static RegionsManager Instance { get { return _instance; } }

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
        {
            indicator = new GameObject("Indicator");
            indicatorRenderer = indicator.AddComponent<SpriteRenderer>();
            indicatorRenderer.sortingLayerName = "Indicator";
            indicator.SetActive(false);
            indicatorRenderer.sprite = regionIndicatorSprite;
        }
        {
            showRegionTilemap.gameObject.SetActive(false);
        }
    }

    public bool AllowMovement
    {
        get { return true; }
    }

    public void StartState(object[] args)
    {
        showRegionTilemap.gameObject.SetActive(true);
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
            showRegionTilemap.gameObject.SetActive(false);
            indicator.SetActive(false);
            return true;
        }
    }

    public void Execute()
    {
        if (coroutineRunning)
            return;

        if (selectedRegion == null)
        {
            Debug.Log("No region selected.");
            return;
        }

        Vector3Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();
        TileInformation mouseTile = TileInformationManager.Instance.GetTileInformation(mouseTilePosition);

        indicator.transform.position = new Vector2(mouseTilePosition.x, mouseTilePosition.y);

        bool regionPlaceable = RegionPlaceable(selectedRegion, mouseTilePosition);
        bool regionRemoveable = (mouseTile != null && mouseTile.GetRegionInformation().id != RegionInformationManager.DEFAULT_REGION_ID);

        //Indicator things
        {
            if (regionPlaceable)
                indicatorRenderer.color = selectedRegion.color;

            else if (regionRemoveable) //Region is removeable
                indicatorRenderer.color = ResourceManager.Instance.yellow;

            else
                indicatorRenderer.color = ResourceManager.Instance.red;
        }

        {
            if (regionPlaceable && Input.GetButtonDown("Primary"))
            {
                StartCoroutine(PlaceRegion(selectedRegion));
            }
        }

    }


    #region Helpers
    public bool RegionPlaceable(RegionInformation info, Vector3Int position)
    {
        return true;

    }

    public bool RegionRemoveable(Vector3Int position)
    {
        return true;
    }

    public void ShowRegions(bool show)
    {
        showRegionTilemap.gameObject.SetActive(show);

        if (show)
        {
            for (int i = 0; i < TileInformationManager.tileCountX; i++)
            {
                for (int j = 0; j < TileInformationManager.tileCountY; j++)
                {
                    Vector3Int pos = (new Vector3Int(i, j, 0));
                    if (TileInformationManager.Instance.GetTileInformation(pos).GetRegionInformation().id != RegionInformationManager.DEFAULT_REGION_ID)
                    {
                        showRegionTilemap.SetTile(pos, showRegionTile);
                        showRegionTilemap.SetTileFlags(pos, TileFlags.None);
                        showRegionTilemap.SetColor(pos, TileInformationManager.Instance.GetTileInformation(pos).GetRegionInformation().color);
                    }
                }
            }
        }
        else
        {
            showRegionTilemap.ClearAllTiles();
        }
    }

    public void SetSelectedRegion(int id)
    {
        if (RegionInformationManager.Instance.regionInformationMap.TryGetValue(id, out RegionInformation info))
            selectedRegion = info;
        else
            Debug.Log("Could not find region with id: " + id);
    }

    #endregion

    IEnumerator PlaceRegion(RegionInformation region)
    {
        coroutineRunning = true;
        Vector3Int previousTilePosition = new Vector3Int(-1, -1, -1);

        Vector3Int startPos = TileInformationManager.Instance.GetMouseTile();
        int[,] regionPlaceableCache = new int[TileInformationManager.tileCountX, TileInformationManager.tileCountY]; // 0 not visited, -1 not placeable, 1 placeable

        indicatorRenderer.drawMode = SpriteDrawMode.Tiled;

        int minX = -1, maxX = -1, minY = -1, maxY = -1;

        while (Input.GetButton("Primary"))
        {
            Vector3Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();

            if (mouseTilePosition != previousTilePosition)
                previousTilePosition = mouseTilePosition;
            else
                goto Skip;

            if (region == null || region.id == RegionInformationManager.DEFAULT_REGION_ID)
                goto Skip;


            minX = (startPos.x >= mouseTilePosition.x) ? mouseTilePosition.x : startPos.x;
            maxX = (startPos.x >= mouseTilePosition.x) ? startPos.x : mouseTilePosition.x;
            minY = (startPos.y >= mouseTilePosition.y) ? mouseTilePosition.y : startPos.y;
            maxY = (startPos.y >= mouseTilePosition.y) ? startPos.y : mouseTilePosition.y;

            indicator.transform.position = new Vector2(minX + (maxX - minX) / 2f, minY + (maxY - minY) / 2f);
            indicatorRenderer.size = new Vector2(maxX - minX + 1, maxY - minY + 1);

            bool placeable = true;

            for (int i = minX; i <= maxX; i++)
            {
                for (int j = minY; j <= maxY; j++)
                {
                    int tilePlaceable = regionPlaceableCache[i, j];
                    if (tilePlaceable != 0)
                    {
                        if (tilePlaceable == -1)
                        {
                            placeable = false;
                            goto BreakLoop;
                        }
                    }
                    else
                        regionPlaceableCache[i, j] = (RegionPlaceable(region, new Vector3Int(i, j, 0))) ? 1 : 0;
                }
            }
            BreakLoop:

            if (placeable)
                indicatorRenderer.color = region.color;
            else
                indicatorRenderer.color = ResourceManager.Instance.red;

            Skip:
            yield return 0;
        }

        //Create region here
        {
            if (!(region == null || region.id == RegionInformationManager.DEFAULT_REGION_ID))
            {
                for (int i = minX; i <= maxX; i++)
                {
                    for (int j = minY; j <= maxY; j++)
                    {
                        Vector3Int pos = new Vector3Int(i, j, 0);
                        TileInformationManager.Instance.GetTileInformation(pos).SetRegion(region.id);
                        showRegionTilemap.SetTile(pos, showRegionTile);
                        showRegionTilemap.SetTileFlags(pos, TileFlags.None);
                        showRegionTilemap.SetColor(pos, region.color);
                    }
                }
            }
        }

        indicatorRenderer.size = new Vector2(1, 1);
        indicatorRenderer.drawMode = SpriteDrawMode.Simple;
        coroutineRunning = false;
    }

    IEnumerator RemoveRegion()
    {
        /*
        coroutineRunning = true;
        Vector3Int previousTilePosition = new Vector3Int(-1, -1, -1);

        while (Input.GetButton("Primary"))
        {
            Vector3Int mouseTilePosition = GetMouseTile();

            if (mouseTilePosition != previousTilePosition)
                previousTilePosition = mouseTilePosition;
            else
                goto Skip;

            Skip:
            yield return 0;
        }
        */
        yield return 0;
    }
}

