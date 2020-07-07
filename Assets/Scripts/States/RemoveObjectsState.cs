using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveObjectsState : MonoBehaviour, IPlayerState
{
    [SerializeField] private Sprite indicatorSprite = null;

    private GameObject indicator;
    private SpriteRenderer indicatorRenderer;

    private static RemoveObjectsState _instance;
    public static RemoveObjectsState Instance { get { return _instance; } }

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
            indicatorRenderer.sprite = indicatorSprite;
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

    public void Execute()
    {
        Vector3Int mouseTilePosition = TileInformationManager.Instance.GetMouseTile();
        TileInformation mouseTile = TileInformationManager.Instance.GetTileInformation(mouseTilePosition);

        bool tileIsNotEmpty = (mouseTile != null && !(mouseTile.TileIsEmpty()));

        //Indicator things
        {
            indicator.transform.position = new Vector2(mouseTilePosition.x, mouseTilePosition.y);

            if (tileIsNotEmpty)
                indicatorRenderer.color = ResourceManager.Instance.Green;
            else
                indicatorRenderer.color = ResourceManager.Instance.Red;
        }

        //Remove
        if (tileIsNotEmpty && Input.GetButtonDown("Primary"))
        {
            TileInformation info = TileInformationManager.Instance.GetTileInformation(mouseTilePosition);
            ObjectType highestObject = info.GetTopMostObjectType();

            TileObjectsManager.Instance.RemoveObject(info, highestObject);

        }
    }

    public bool TryEndState()
    {
        indicator.SetActive(false);
        return true;
    }
}
