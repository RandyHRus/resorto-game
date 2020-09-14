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
        List<Vector3Int> waterEdgesConnectedToOcean = new List<Vector3Int>();

        // Use flood fill to find all edges connected to ocean
        // Sand next to any of the found water edges is a valid position 

        // Flood fill start at (0,0), (0,0) should be ocean

        HashSet<Vector3Int> visitedPositions = new HashSet<Vector3Int>();

        Stack<Vector3Int> positionsToCheck = new Stack<Vector3Int>();
        positionsToCheck.Push(new Vector3Int(0, 0, 0));

        //Flood fill, find edges
        while (positionsToCheck.Count > 0)
        {
            Vector3Int thisPosition = positionsToCheck.Pop();

            if (visitedPositions.Contains(thisPosition))
            {
                continue;
            }

            visitedPositions.Add(thisPosition);

            TileInformation thisTileInfo = TileInformationManager.Instance.GetTileInformation(thisPosition);
            if (thisTileInfo == null || !TileLocation.Water.HasFlag(thisTileInfo.tileLocation))
            {
                //Not water
                continue;
            }

            Vector3Int[] positionsToAdd = new Vector3Int[]
            {
                new Vector3Int(thisPosition.x + 1, thisPosition.y,     0),
                new Vector3Int(thisPosition.x - 1, thisPosition.y,     0),
                new Vector3Int(thisPosition.x,     thisPosition.y + 1, 0),
                new Vector3Int(thisPosition.x,     thisPosition.y - 1, 0)
            };
            foreach (Vector3Int addPosition in positionsToAdd)
            {
                positionsToCheck.Push(addPosition);
            }

            if (thisTileInfo.tileLocation == TileLocation.WaterEdge)
                waterEdgesConnectedToOcean.Add(thisPosition);
        }

        if (waterEdgesConnectedToOcean.Count == 0)
            throw new IslandGenerationException("Could not find any water edge connected to ocean"); //Request new island generation

        HashSet<Vector3Int> sandPositionsNextToOcean = new HashSet<Vector3Int>();

        // Find all edges from the found edges that is connected to sand
        // Could fail if all edges are not next to sand (they may be connected to cliff, land etc)
        foreach (Vector3Int edge in waterEdgesConnectedToOcean)
        {
            Vector3Int[] neighboursToCheckForSand = new Vector3Int[]
            {
                new Vector3Int(edge.x + 1, edge.y,     0),
                new Vector3Int(edge.x - 1, edge.y,     0),
                new Vector3Int(edge.x,     edge.y + 1, 0),
                new Vector3Int(edge.x,     edge.y - 1, 0)
            };

            List<Vector3Int> validSandPositions = new List<Vector3Int>();

            foreach (Vector3Int neighbour in neighboursToCheckForSand)
            {
                TileInformation neighbourInfo = TileInformationManager.Instance.GetTileInformation(neighbour);
                if (neighbourInfo?.tileLocation == TileLocation.Sand)
                    sandPositionsNextToOcean.Add(neighbour);
            }

        }

        if (sandPositionsNextToOcean.Count == 0)
        {
            throw new IslandGenerationException("None of the water edges were connected to sand!");
        }

        //A set of all valid groups of sand positions
        List<IslandStartingPosition> validStartingPositions = new List<IslandStartingPosition>();

        //Find positions with more sand nearby
        foreach (Vector3Int pos in sandPositionsNextToOcean)
        {
            List<Vector3Int> nearbySand = new List<Vector3Int>();

            for (int i = - 2; i <= 2; i++)
            {
                for (int j = -2; j <= 2; j++)
                {
                    Vector3Int checkPosition = new Vector3Int(pos.x + i, pos.y + j, 0);

                    if (checkPosition == pos)
                        continue;

                    TileInformation checkPositionInfo = TileInformationManager.Instance.GetTileInformation(checkPosition);

                    if (checkPositionInfo?.tileLocation == TileLocation.Sand)
                    {
                        nearbySand.Add(checkPosition);
                    }
                }
            }

            if (nearbySand.Count >= 1)
            {
                int randomChestPositionIndex = Random.Range(0, nearbySand.Count);
                Vector3Int randomChestPosition = nearbySand[randomChestPositionIndex];
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
    public Vector3Int ActualStartingPosition { get; private set; }
    public Vector3Int StartingChestPosition { get; private set; }

    public IslandStartingPosition(Vector3Int actualStartingPosition, Vector3Int startingChestPosition)
    {
        this.ActualStartingPosition = actualStartingPosition;
        this.StartingChestPosition = startingChestPosition;
    }
}