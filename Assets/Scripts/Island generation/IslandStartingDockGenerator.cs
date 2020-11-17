using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandStartingDockGenerator: MonoBehaviour
{
    [SerializeField] DockFlooringVariant dockToCreate = null;
    [SerializeField] StairsVariant stairsToCreate = null;
    [SerializeField] RegionInformation unloadingRegionInformation = null;

    private static IslandStartingDockGenerator _instance;
    public static IslandStartingDockGenerator Instance { get { return _instance; } }
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

    public void CreateStartingDock(out Vector2Int unloadingPosition)
    {
        Vector2Int? foundValidStairsPosition = null;

        //First locate a position where we can place stairs
        for (int y = TileInformationManager.mapSize - 1; y >= 0 ; y--)
        {
            List<Vector2Int> foundPositionsInThisRow = new List<Vector2Int>();

            //Offset by (20) so that it generates more towards middle
            for (int x = 20; x < TileInformationManager.mapSize-20; x++)
            {
                Vector2Int dockPos = new Vector2Int(x, y);
                Vector2Int stairsPos = new Vector2Int(x, y - 2);
                Vector2Int belowStairsPos = new Vector2Int(x, y - 3); //Need to check this for SAND

                TileInformationManager.Instance.TryGetTileInformation(belowStairsPos, out TileInformation belowStairsInfo);

                if (belowStairsInfo?.tileLocation == TileLocation.Sand)
                {
                    foundPositionsInThisRow.Add(stairsPos); //If below is sand, can place stairs here
                }
            }

            if (foundPositionsInThisRow.Count > 0)
            {
                foundValidStairsPosition = foundPositionsInThisRow[Random.Range(0, foundPositionsInThisRow.Count)];
                break;
            }
        }

        if (foundValidStairsPosition == null)
            throw new IslandGenerationException("Could not find a place to place stairs");

        //Dock position is offset by some # so it creates a more "natural" looking dock
        int verticalDockX = foundValidStairsPosition.Value.x + 6;

        //Vertical part
        HashSet<Vector2Int> dockPositions1 = new HashSet<Vector2Int>();
        for (int i = TileInformationManager.mapSize - 3; i > foundValidStairsPosition.Value.y + 1; i--)
        {
            dockPositions1.Add(new Vector2Int(verticalDockX, i));
        }

        //Horizontal part connecting to stairs
        HashSet<Vector2Int> dockPositions2 = new HashSet<Vector2Int>(); //Horizontal
        for (int i = foundValidStairsPosition.Value.x; i < verticalDockX; i++)
        {
            dockPositions2.Add(new Vector2Int(i, foundValidStairsPosition.Value.y + 2));
        }

        //The unloading part
        HashSet<Vector2Int> unloadingDockPositions = new HashSet<Vector2Int>(); //Horizontal
        for (int i = verticalDockX; i < verticalDockX + 5; i++)
        {
            for (int j = TileInformationManager.mapSize - 1; j > TileInformationManager.mapSize - 3; j--)
            {
                unloadingDockPositions.Add(new Vector2Int(i, j));
            }
        }

        //Just 1x1
        HashSet<Vector2Int> unloadingRegionPositions = new HashSet<Vector2Int>();
        unloadingRegionPositions.Add(new Vector2Int(verticalDockX+2, TileInformationManager.mapSize - 1));

        unloadingPosition = new Vector2Int(verticalDockX + 2, TileInformationManager.mapSize - 1);

        if (dockPositions1.Count > 0)
        {
            if (!FlooringManager.TryPlaceFlooring(dockToCreate, dockPositions1, FlooringRotation.Vertical))
                throw new IslandGenerationException("Something went wrong generating the dock1!");
        }

        if (!FlooringManager.TryPlaceFlooring(dockToCreate, dockPositions2, FlooringRotation.Horizontal))
            throw new IslandGenerationException("Something went wrong generating the dock2!");

        if (!FlooringManager.TryPlaceFlooring(dockToCreate, unloadingDockPositions, FlooringRotation.Horizontal))
            throw new IslandGenerationException("Something went wrong generating the unloading dock");

        if (!StairsManager.TryCreateStairs(stairsToCreate, foundValidStairsPosition.Value))
            throw new IslandGenerationException("Something went wrong generating the stairs!");

        if (!RegionManager.TryCreateRegion(unloadingRegionInformation, unloadingRegionPositions))
            throw new IslandGenerationException("Something went wrong generating the unloading region!");
    }
}
