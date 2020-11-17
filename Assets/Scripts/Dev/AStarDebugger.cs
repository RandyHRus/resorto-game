using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

//Remove in production
public class AStarDebugger : MonoBehaviour
{
    [SerializeField] private Tilemap colorTilemap = null;
    [SerializeField] private Tile indicatorTile = null;
    [SerializeField] private GameObject tileText = null;
    [SerializeField] private Canvas devCanvas = null;
    [SerializeField] private Dictionary<Vector2Int, Text> positionToTextObjectMap = new Dictionary<Vector2Int, Text>();

    bool debugging = false;
    Vector2Int? startPoint;
    Vector2Int? endPoint;
    AStar.AStarPathFinder pathFinder;

    private void Update()
    {
        if (Input.GetButtonDown("Dev2"))
        {
            debugging = !debugging;
            colorTilemap.gameObject.SetActive(debugging);

            if (!debugging)
            {
                StopDebugging();
            }
        }
        else if (CheckMouseOverUI.GetButtonDownAndNotOnUI("Primary") && debugging) {
            Vector2Int mouseTilePos = TileInformationManager.Instance.GetMouseTile();
            if (TileInformationManager.Instance.TryGetTileInformation(mouseTilePos, out TileInformation tileInfo))
            {
                if (startPoint == null)
                {
                    startPoint = (Vector2Int)mouseTilePos;
                    colorTilemap.SetTile((Vector3Int)mouseTilePos, indicatorTile);
                    colorTilemap.SetTileFlags((Vector3Int)mouseTilePos, TileFlags.None);
                    colorTilemap.SetColor((Vector3Int)mouseTilePos, ResourceManager.Instance.Yellow);
                }
                else if (endPoint == null)
                {
                    endPoint = (Vector2Int)mouseTilePos;
                    colorTilemap.SetTile((Vector3Int)mouseTilePos, indicatorTile);
                    colorTilemap.SetTileFlags((Vector3Int)mouseTilePos, TileFlags.None);
                    colorTilemap.SetColor((Vector3Int)mouseTilePos, ResourceManager.Instance.Yellow);
                }
                else
                {
                    if (pathFinder == null)
                    {
                        pathFinder = new AStar.AStarPathFinder((Vector2Int)startPoint, (Vector2Int)endPoint, OnNodeUpdate);
                    }
                    else if (!pathFinder.Finished)
                    {
                        pathFinder.NextStep();
                    }
                }
            }
        }
    }

    private void StopDebugging()
    {
        startPoint = null;
        endPoint = null;
        debugging = false;

        foreach (KeyValuePair<Vector2Int, Text> pair in positionToTextObjectMap)
            Destroy(pair.Value.gameObject);

        colorTilemap.ClearAllTiles();
        positionToTextObjectMap.Clear();

        pathFinder = null;
    }

    private void OnNodeUpdate(AStar.AStarNode node, AStar.AStarNodeUpdateType type)
    {
        void UpdateText(Text text)
        {
            text.text = "F: " + node.FCost + "\nG: " + node.GCost + "\nH " + node.HCost;
        }

        Vector2Int pos = (Vector2Int)node.Position;
        colorTilemap.SetTile((Vector3Int)pos, indicatorTile);
        colorTilemap.SetTileFlags((Vector3Int)pos, TileFlags.None);

        switch (type)
        {
            case (AStar.AStarNodeUpdateType.Closed):
                colorTilemap.SetColor((Vector3Int)pos, ResourceManager.Instance.Red);
                UpdateText(positionToTextObjectMap[pos]);
                break;
            case (AStar.AStarNodeUpdateType.NewlyOpen):
                colorTilemap.SetColor((Vector3Int)pos, ResourceManager.Instance.Green);
                Text t = Instantiate(tileText).GetComponent<Text>();
                t.transform.SetParent(devCanvas.transform, false);
                t.transform.position = new Vector2(pos.x, pos.y);
                positionToTextObjectMap.Add(pos, t);
                UpdateText(t);
                break;
            case (AStar.AStarNodeUpdateType.Overwritten):
                colorTilemap.SetColor((Vector3Int)pos, ResourceManager.Instance.Yellow);
                UpdateText(positionToTextObjectMap[pos]);
                break;
            case (AStar.AStarNodeUpdateType.MarkShortestPath):
                colorTilemap.SetColor((Vector3Int)pos, ResourceManager.Instance.Purple);
                break;
            case (AStar.AStarNodeUpdateType.MarkPathKeyPoint):
                colorTilemap.SetColor((Vector3Int)pos, ResourceManager.Instance.Orange);
                break;

            default:
                break;

        }
    }
}
