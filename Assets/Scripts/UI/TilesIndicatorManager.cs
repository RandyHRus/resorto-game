using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesIndicatorManager
{
    public TilesIndicatorManager()
    {
        currentTileIndicatorPositions = new HashSet<Vector3Int>();
    }

    private class TileIndicator
    {
        public TileIndicator(Vector2Int position)
        {
            Object = new GameObject("Tile Indicator");
            Renderer = Object.AddComponent<SpriteRenderer>();
            Transform = Object.transform;

            defaultPosition = new Vector3(position.x, position.y, position.y);
            Transform.position = defaultPosition;
            Renderer.sortingLayerName = "Indicator";
            Object.SetActive(false);
        }

        public void Toggle(bool show)
        {
            //Reset and show
            if (show)
            {
                Transform.position = defaultPosition;
            }

            Object.SetActive(show);
        }

        private Vector3 defaultPosition;

        public Transform Transform { get; private set; }
        public SpriteRenderer Renderer { get; private set; }
        public GameObject Object { get; private set; }
    }

    private static TileIndicator[,] tileIndicators;
    private static Vector3Int defaultPos = new Vector3Int(-1, -1, -1);

    private HashSet<Vector3Int> currentTileIndicatorPositions;

    //Initializer
    static TilesIndicatorManager()
    {
        tileIndicators = new TileIndicator[TileInformationManager.mapSize, TileInformationManager.mapSize];

        for (int i = 0; i < TileInformationManager.mapSize; i++)
        {
            for (int j = 0; j < TileInformationManager.mapSize; j++)
            {
                tileIndicators[i, j] = new TileIndicator(new Vector2Int(i, j));
            }
        }
    }

    public void SetColor(Vector3Int pos, Color32 color)
    {
        if (TileInformationManager.Instance.PositionInMap(pos))
            tileIndicators[pos.x, pos.y].Renderer.color = color;
    }

    public void SetSprite(Vector3Int pos, Sprite sprite)
    {
        if (TileInformationManager.Instance.PositionInMap(pos))
            tileIndicators[pos.x, pos.y].Renderer.sprite = sprite;
    }

    public void Offset(Vector3Int pos, Vector2 offset)
    {
        if (TileInformationManager.Instance.PositionInMap(pos))
            tileIndicators[pos.x, pos.y].Transform.position = new Vector2(pos.x, pos.y) + offset;
    }

    // Hides current tiles, show new current tiles
    // Returns a list of new showing tiles
    public List<Vector3Int> SwapCurrentTiles(HashSet<Vector3Int> newTilePositions)
    {
        List<Vector3Int> newlyShownTiles = new List<Vector3Int>();

        //Show new indicators
        foreach (Vector3Int pos in newTilePositions)
        {
            if (!currentTileIndicatorPositions.Contains(pos))
            {
                Toggle(pos, true);
                newlyShownTiles.Add(pos);
            }
        }

        //Hide old indicators
        foreach (Vector3Int pos in currentTileIndicatorPositions)
        {
            if (!newTilePositions.Contains(pos))
                Toggle(pos, false);
        }

        currentTileIndicatorPositions = new HashSet<Vector3Int>(newTilePositions);

        return newlyShownTiles;
    }

    //Hides current tiles, show new tile
    //Returns true if actually swapped
    public bool SwapCurrentTiles(Vector3Int newPos)
    {
        bool isNewPosition = true;

        foreach (Vector3Int pos in currentTileIndicatorPositions)
        {
            if (pos == newPos)
            {
                isNewPosition = false;
            }
            else
            {
                Toggle(pos, false);
            }
        }

        if (isNewPosition)
        {
            Toggle(newPos, true);

            currentTileIndicatorPositions = new HashSet<Vector3Int>()
            {
                newPos
            };
        }

        return isNewPosition;
    }

    private void Toggle(Vector3Int pos, bool show)
    {
        if (TileInformationManager.Instance.PositionInMap(pos))
            tileIndicators[pos.x, pos.y].Toggle(show);
    }

    //Remember to hide tiles before object destruction
    public void HideCurrentTiles()
    {
        foreach (Vector3Int pos in currentTileIndicatorPositions)
        {
            Toggle(pos, false);
        }

        currentTileIndicatorPositions.Clear();
    }
}
