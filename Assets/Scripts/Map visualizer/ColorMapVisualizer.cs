using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class ColorMapVisualizer : MapVisualizer
{
    public abstract Color32 GetColor(Vector2Int position);

    private Tilemap colorTileMap => ResourceManager.Instance.MapVisualizerTilemapInstance;
    private Tile tileSprite => ResourceManager.Instance.MapVisualizerColorTile;

    public override void ShowVisualizer()
    {
        colorTileMap.gameObject.SetActive(true);

        for (int i = 0; i < TileInformationManager.mapSize; i++)
        {
            for (int j = 0; j < TileInformationManager.mapSize; j++)
            {
                Vector2Int pos = new Vector2Int(i, j);
                Color32 color = GetColor(pos);
                color.a = (byte)(0.8f * 255);

                colorTileMap.SetTile((Vector3Int)pos, tileSprite);
                colorTileMap.SetTileFlags((Vector3Int)pos, TileFlags.None);
                colorTileMap.SetColor((Vector3Int)pos, color);

                //Debug.Log(color);
            }
        }
    }

    public void OverrideColor(Vector2Int overridePosition, Color32 color)
    {
        colorTileMap.SetTile((Vector3Int)overridePosition, tileSprite);
        colorTileMap.SetTileFlags((Vector3Int)overridePosition, TileFlags.None);
        colorTileMap.SetColor((Vector3Int)overridePosition, color);
    }

    public override void HideVisualizer()
    {
        colorTileMap.ClearAllTiles();
        colorTileMap.gameObject.SetActive(false);
    }
}
