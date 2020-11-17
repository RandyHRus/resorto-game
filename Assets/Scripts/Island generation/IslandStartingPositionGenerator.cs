using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandStartingPositionGenerator : MonoBehaviour
{

    private static IslandStartingPositionGenerator _instance;
    public static IslandStartingPositionGenerator Instance { get { return _instance; } }
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
    }

    public IslandStartingPosition GetRandomStartingPosition()
    {
        List<IslandStartingPosition> validPositions = FindValidPositions();

        return validPositions[Random.Range(0, validPositions.Count)];
    }

    /* Find valid positions
    /*
     *  - position that is sand
     *  - is connected to ocean
     *  - is next to water
     *  - has enough sand nearby to spawn other things (starter chest, maybe NPC, etc)
    */
    private List<IslandStartingPosition> FindValidPositions()
    {
        List<Vector2Int> waterEdgesConnectedToOcean = new List<Vector2Int>(128);

        // Use flood fill to find all edges connected to ocean
        // Sand next to any of the found water edges is a valid position 

        // Flood fill start at (0,0), (0,0) should be ocean

        HashSet<Vector2Int> visitedPositions = new HashSet<Vector2Int>();

        Stack<Vector2Int> positionsToCheck = new Stack<Vector2Int>();
        positionsToCheck.Push(new Vector2Int(0, 0));

        //Flood fill, find edges
        while (positionsToCheck.Count > 0)
        {
            Vector2Int thisPosition = positionsToCheck.Pop();

            if (visitedPositions.Contains(thisPosition))
            {
                continue;
            }

            visitedPositions.Add(thisPosition);

            if (!TileInformationManager.Instance.TryGetTileInformation(thisPosition, out TileInformation thisTileInfo))
                continue;

            if (!TileLocation.Water.HasFlag(thisTileInfo.tileLocation))
                continue;

            Vector2Int[] positionsToAdd = new Vector2Int[]
            {
                new Vector2Int(thisPosition.x + 1, thisPosition.y   ),
                new Vector2Int(thisPosition.x - 1, thisPosition.y   ),
                new Vector2Int(thisPosition.x,     thisPosition.y + 1),
                new Vector2Int(thisPosition.x,     thisPosition.y - 1)
            };
            foreach (Vector2Int addPosition in positionsToAdd)
            {
                positionsToCheck.Push(addPosition);
            }

            if (thisTileInfo.tileLocation == TileLocation.WaterEdge)
                waterEdgesConnectedToOcean.Add(thisPosition);
        }

        if (waterEdgesConnectedToOcean.Count == 0)
            throw new IslandGenerationException("Could not find any water edge connected to ocean"); //Request new island generation

        HashSet<Vector2Int> sandPositionsNextToOcean = new HashSet<Vector2Int>();

        // Find all edges from the found edges that is connected to sand
        // Could fail if all edges are not next to sand (they may be connected to cliff, land etc)
        foreach (Vector2Int edge in waterEdgesConnectedToOcean)
        {
            Vector2Int[] neighboursToCheckForSand = new Vector2Int[]
            {
                new Vector2Int(edge.x + 1, edge.y),
                new Vector2Int(edge.x - 1, edge.y),
                new Vector2Int(edge.x,     edge.y + 1),
                new Vector2Int(edge.x,     edge.y - 1)
            };

            List<Vector2Int> validSandPositions = new List<Vector2Int>(128);

            foreach (Vector2Int neighbour in neighboursToCheckForSand)
            {
                if (!TileInformationManager.Instance.TryGetTileInformation(neighbour, out TileInformation neighbourInfo))
                    continue;

                if (neighbourInfo.tileLocation == TileLocation.Sand)
                    sandPositionsNextToOcean.Add(neighbour);
            }

        }

        if (sandPositionsNextToOcean.Count == 0)
        {
            throw new IslandGenerationException("None of the water edges were connected to sand!");
        }

        //A set of all valid groups of sand positions
        List<IslandStartingPosition> validStartingPositions = new List<IslandStartingPosition>(128);

        //Find positions with more sand nearby
        foreach (Vector2Int pos in sandPositionsNextToOcean)
        {
            List<Vector2Int> nearbySand = new List<Vector2Int>(128);

            for (int i = - 2; i <= 2; i++)
            {
                for (int j = -2; j <= 2; j++)
                {
                    Vector2Int checkPosition = new Vector2Int(pos.x + i, pos.y + j);

                    if (checkPosition == pos)
                        continue;

                    TileInformationManager.Instance.TryGetTileInformation(checkPosition, out TileInformation checkPositionInfo);

                    if (checkPositionInfo?.tileLocation == TileLocation.Sand)
                    {
                        nearbySand.Add(checkPosition);
                    }
                }
            }

            if (nearbySand.Count >= 1)
            {
                int randomChestPositionIndex = Random.Range(0, nearbySand.Count);
                Vector2Int randomChestPosition = nearbySand[randomChestPositionIndex];
                nearbySand.RemoveAt(randomChestPositionIndex);

                IslandStartingPosition startingPosition = new IslandStartingPosition(pos, randomChestPosition);
                validStartingPositions.Add(startingPosition);
            }
        }

        if (validStartingPositions.Count == 0)
        {
            throw new IslandGenerationException("No sand position had enough nearby sand!");
        }
        else
        {
            return validStartingPositions;
        }
    }
}

public class IslandStartingPosition
{
    public Vector2Int ActualStartingPosition { get; private set; }
    public Vector2Int StartingChestPosition { get; private set; }

    public IslandStartingPosition(Vector2Int actualStartingPosition, Vector2Int startingChestPosition)
    {
        this.ActualStartingPosition = actualStartingPosition;
        this.StartingChestPosition = startingChestPosition;
    }
}