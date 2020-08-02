using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInitialization : MonoBehaviour
{
    private void Awake()
    {
        IslandGenerationPipeline.IslandCompleted += SetInitialLocation;
    }

    //Moves player onto sand
    private void SetInitialLocation()
    {
        Vector3Int pos = FindValidSandPositionConnectedToOcean();
        transform.position = pos;

        GetComponent<PlayerMovement>().InitializeLayerAndDepth();
    }

    private Vector3Int FindValidSandPositionConnectedToOcean()
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
            foreach(Vector3Int addPosition in positionsToAdd)
            {
                positionsToCheck.Push(addPosition);
            }

            if (thisTileInfo.tileLocation == TileLocation.WaterEdge)
                waterEdgesConnectedToOcean.Add(thisPosition);
        }

        if (waterEdgesConnectedToOcean.Count == 0)
            throw new IslandGenerationException("Could not find any water edge connected to ocean"); //Request new island generation

        // Find a random edge from the found edges that is connected to sand
        // Could fail if all edges are not next to sand (they may be connected to cliff, land etc)
        while (waterEdgesConnectedToOcean.Count > 0)
        {
            int randomIndex = Random.Range(0, waterEdgesConnectedToOcean.Count);
            Vector3Int randomEdge = waterEdgesConnectedToOcean[randomIndex];
            waterEdgesConnectedToOcean.RemoveAt(randomIndex);

            Vector3Int[] neighboursToCheckForSand = new Vector3Int[]
            {
                new Vector3Int(randomEdge.x + 1, randomEdge.y,     0),
                new Vector3Int(randomEdge.x - 1, randomEdge.y,     0),
                new Vector3Int(randomEdge.x,     randomEdge.y + 1, 0),
                new Vector3Int(randomEdge.x,     randomEdge.y - 1, 0)
            };

            List<Vector3Int> validSandPositions = new List<Vector3Int>();

            foreach(Vector3Int neighbour in neighboursToCheckForSand)
            {
                TileInformation neighbourInfo = TileInformationManager.Instance.GetTileInformation(neighbour);
                if (neighbourInfo != null && neighbourInfo.tileLocation == TileLocation.Sand)
                    validSandPositions.Add(neighbour);
            }

            if (validSandPositions.Count > 0)
                return validSandPositions[Random.Range(0, validSandPositions.Count)];
        }

        throw new IslandGenerationException("None of the water edges were connected to sand!");
    }
}
