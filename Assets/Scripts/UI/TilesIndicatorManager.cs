using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesIndicatorManager
{
    private static TileIndicator[,] tileIndicators;
    private static Vector2Int defaultPos = new Vector2Int(-1, -1);
    private HashSet<Vector2Int> currentTileIndicatorPositions;

    public TilesIndicatorManager()
    {
        currentTileIndicatorPositions = new HashSet<Vector2Int>();
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

    public void SetColor(Vector2Int pos, Color32 color)
    {
        if (TileInformationManager.Instance.PositionInMap(pos))
            tileIndicators[pos.x, pos.y].Renderer.color = color;
    }

    public void SetSprite(Vector2Int pos, Sprite sprite)
    {
        if (TileInformationManager.Instance.PositionInMap(pos))
            tileIndicators[pos.x, pos.y].Renderer.sprite = sprite;
    }

    public void Offset(Vector2Int pos, Vector2 offset)
    {
        if (TileInformationManager.Instance.PositionInMap(pos))
            tileIndicators[pos.x, pos.y].Transform.position = new Vector2(pos.x, pos.y) + offset;
    }

    // Hides current tiles, show new current tiles
    // Returns a list of new showing tiles
    public List<Vector2Int> SwapCurrentTiles(HashSet<Vector2Int> newTilePositions)
    {
        List<Vector2Int> newlyShownTiles = new List<Vector2Int>(newTilePositions.Count);

        //Show new indicators
        foreach (Vector2Int pos in newTilePositions)
        {
            if (!currentTileIndicatorPositions.Contains(pos))
            {
                Toggle(pos, true);
                newlyShownTiles.Add(pos);
            }
        }

        //Hide old indicators
        foreach (Vector2Int pos in currentTileIndicatorPositions)
        {
            if (!newTilePositions.Contains(pos))
                Toggle(pos, false);
        }

        currentTileIndicatorPositions = new HashSet<Vector2Int>(newTilePositions);

        return newlyShownTiles;
    }

    //Hides current tiles, show new tile
    //Returns true if actually swapped
    public bool SwapCurrentTiles(Vector2Int newPos)
    {
        bool isNewPosition = true;

        foreach (Vector2Int pos in currentTileIndicatorPositions)
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

            currentTileIndicatorPositions = new HashSet<Vector2Int>()
            {
                newPos
            };
        }

        return isNewPosition;
    }

    private void Toggle(Vector2Int pos, bool show)
    {
        if (TileInformationManager.Instance.PositionInMap(pos))
            tileIndicators[pos.x, pos.y].Toggle(show);
    }

    //Remember to hide tiles before object destruction
    public void ClearCurrentTiles()
    {
        foreach (Vector2Int pos in currentTileIndicatorPositions)
        {
            Toggle(pos, false);
        }

        currentTileIndicatorPositions.Clear();
    }
}
