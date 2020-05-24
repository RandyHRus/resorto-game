using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShowElevation : MonoBehaviour
{
    [SerializeField] private Tilemap elevationTilemap = null;
    [SerializeField] private Tile indicatorTile = null;

    private bool showElevation = false;

    private void Update()
    {
        if (Input.GetButtonDown("Dev"))
        {
            ToggleElevation(!showElevation);
        }
    }

    public void ToggleElevation(bool show)
    {
        showElevation = show;

        elevationTilemap.gameObject.SetActive(show);

        int elevationColorSize = ResourceManager.Instance.elevationColors.Length;

        if (show)
        {
            for (int i = 0; i < TileInformationManager.tileCountX; i++)
            {
                for (int j = 0; j < TileInformationManager.tileCountX; j++)
                {
                    Vector3Int pos = (new Vector3Int(i, j, 0));
                    int layerNum = TileInformationManager.Instance.GetTileInformation(pos).layerNum;

                    if (layerNum != Constants.INVALID_TILE_LAYER)
                    {
                        elevationTilemap.SetTile(pos, indicatorTile);

                        if (layerNum < elevationColorSize) {
                            elevationTilemap.SetTileFlags(pos, TileFlags.None);
                            elevationTilemap.SetColor(pos, ResourceManager.Instance.elevationColors[layerNum]);
                        }
                        else
                        {
                            Debug.Log("No color set for elevation layer");
                        }
                    }
                    else
                    {
                        elevationTilemap.SetTile(pos, null);
                    }
                }
            }
        }
    }
}
