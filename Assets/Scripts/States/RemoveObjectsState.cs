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

        bool objectOnTileExists = (mouseTile != null && !(mouseTile.TileIsEmpty()));

        //Indicator things
        {
            indicator.transform.position = new Vector2(mouseTilePosition.x, mouseTilePosition.y);

            if (objectOnTileExists)
                indicatorRenderer.color = ResourceManager.Instance.yellow;
            else
                indicatorRenderer.color = ResourceManager.Instance.red;
        }

        //Remove
        if (objectOnTileExists && Input.GetButtonDown("Primary"))
        {
            int removedId = TileInformationManager.Instance.GetTileInformation(mouseTilePosition).RemoveTileObject();
            if (removedId != Constants.INVALID_ID)
            {
                InventoryManager.Instance.AddItem(new ObjectItem(removedId), 1);
            }
        }
    }

    public bool TryEndState()
    {
        indicator.SetActive(false);
        return true;
    }
}
