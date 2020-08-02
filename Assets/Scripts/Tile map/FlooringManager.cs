using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlooringManager
{
    /*
     * Flooring code
     * ##### -> 43210
     * 
     * *****4*****
     * if 0, horizontal
     * if 1, vertical
     * 
     * *****0-4*****
     * if 0, neighbour empty
     * if 1, neighbour NOT empty
     * _______
     * |  0   |
     * |3    1|
     * |__2___|
     */

    private static int MAX_SUPPORTS_SPACING = 4;

    public static bool FlooringPlaceable(FlooringVariantBase flooringVariant, Vector3Int position)
    {
        TileInformation info = TileInformationManager.Instance.GetTileInformation(position);
        TileInformation belowInfo = TileInformationManager.Instance.GetTileInformation(new Vector3Int(position.x, position.y -1, 0));

        if (info == null || belowInfo == null)
            return false;

        if (!info.TileIsEmpty() || !belowInfo.TileIsEmpty())
            return false;

        if (info.NormalFlooringGroup != null)
            return false;

        if (flooringVariant.GetType() == typeof(NormalFlooringVariant))
        {
            if (!TileLocation.Land.HasFlag(info.tileLocation))
                return false;
        }
        else if (flooringVariant.GetType() == typeof(DockFlooringVariant))
        {
            if (!TileLocation.Water.HasFlag(info.tileLocation) || !TileLocation.Water.HasFlag(belowInfo.tileLocation))
                return false;
        }

        return true;
    }

    public static bool FlooringRemoveable(Vector3Int position)
    {
        TileInformation info = TileInformationManager.Instance.GetTileInformation(position);
        if (info == null)
            return false;

        if (!info.TileIsEmpty())
            return false;

        if (info.GetTopFlooringGroup() == null)
            return false;

        return true;
    }

    public static bool TryPlaceFlooring(FlooringVariantBase flooringVariant, HashSet<Vector3Int> positions, FlooringRotation rotation)
    {
        foreach (Vector3Int position in positions)
        {
            //Exit if can't place flooring
            if (!FlooringPlaceable(flooringVariant, position))
                return false;
        }

        if (flooringVariant.GetType() == typeof(NormalFlooringVariant))
        {
            //TODO
        }
        else if (flooringVariant.GetType() == typeof(DockFlooringVariant))
        {
            CreateDock(flooringVariant, positions, rotation);
        }

        return true;
    }

    private static void CreateDock(FlooringVariantBase flooringVariant, HashSet<Vector3Int> positions, FlooringRotation rotation)
    {
        FindEdges(positions, out int minX, out int maxX, out int minY, out int maxY);

        //Create docks
        Dictionary<Vector3Int, FlooringNormalPartOnTile> floorings = new Dictionary<Vector3Int, FlooringNormalPartOnTile>();
        foreach (Vector3Int position in positions)
        {
            FlooringNormalPartOnTile f = CreateSingleDock((DockFlooringVariant)flooringVariant, position);
            floorings.Add(position, f);
        }

        //Create supports
        List<GameObject> supportObjects = new List<GameObject>();
        {
            float xSupportsSpacing = GetSpacing(minX, maxX, out int xSupportsCount);
            float ySupportsSpacing = GetSpacing(minY, maxY, out int ySupportsCount);

            for (int i = 0; i < xSupportsCount; i++)
            {
                for (int j = 0; j < ySupportsCount; j++)
                {
                    int xPos = minX + Mathf.RoundToInt(i * xSupportsSpacing);
                    int yPos = minY + Mathf.RoundToInt(j * ySupportsSpacing);
                    Vector3Int position = new Vector3Int(xPos, yPos, 0);
                    GameObject supportObject = CreateDockSupport((DockFlooringVariant)flooringVariant, position);
                    supportObjects.Add(supportObject);
                }
            }

            float GetSpacing(int min, int max, out int supportsCount)
            {
                if (min == max)
                {
                    supportsCount = 1;
                    return 0;
                }

                int tryDivision = 2;
                float proposedSpacing = Mathf.Abs(max - min);
                while (proposedSpacing > MAX_SUPPORTS_SPACING)
                {
                    proposedSpacing = Mathf.Abs(max - min);
                    proposedSpacing = proposedSpacing / tryDivision;
                    tryDivision++;
                }

                supportsCount = tryDivision;
                return proposedSpacing;
            }
        }

        //Get support positions
        HashSet<Vector3Int> supportPositions = new HashSet<Vector3Int>();
        for (int i = minX; i <= maxX; i++)
        {
            Vector3Int pos = new Vector3Int(i, minY - 1, 0);
            supportPositions.Add(pos);
        }

        //Set tiles
        FlooringGroup group = new FlooringGroup(floorings, supportPositions, new Vector2Int(minX, minY), new Vector2Int(maxX, maxY), supportObjects, flooringVariant, rotation);
        foreach (KeyValuePair<Vector3Int, FlooringNormalPartOnTile> pair in floorings)
        {
            TileInformation tileInfo = TileInformationManager.Instance.GetTileInformation(pair.Key);
            tileInfo.SetNormalFlooringGroup(group);
        }
        foreach (Vector3Int p in supportPositions)
        {
            TileInformation tileInfo = TileInformationManager.Instance.GetTileInformation(p);
            tileInfo.SetSupportFlooringGroup(group);
        }

        //The support object
        GameObject CreateDockSupport(DockFlooringVariant dockVariant, Vector3Int position)
        {
            GameObject supportTop = new GameObject("SupportTop");
            SpriteRenderer topRenderer = supportTop.AddComponent<SpriteRenderer>();
            topRenderer.sprite = dockVariant.SupportTop;
            topRenderer.sortingLayerName = "Flooring";
            supportTop.transform.position = new Vector3(position.x, position.y, position.y - 0.1f);

            GameObject supportBottom = new GameObject("SupportBottom");
            SpriteRenderer bottomRenderer = supportBottom.AddComponent<SpriteRenderer>();
            bottomRenderer.sprite = dockVariant.SupportBottom;
            bottomRenderer.sortingLayerName = "Flooring";
            supportBottom.transform.position = new Vector3(position.x, position.y, position.y + 0.1f);

            GameObject parent = new GameObject("Support");
            Transform t = parent.transform;
            supportTop.transform.SetParent(t);
            supportBottom.transform.SetParent(t);

            return parent;
        }

        FlooringNormalPartOnTile CreateSingleDock(DockFlooringVariant dockVariant, Vector3Int position)
        {
            List<Vector3Int> neighbours = new List<Vector3Int>()
            {
                new Vector3Int(position.x,     position.y + 1, 0),
                new Vector3Int(position.x + 1, position.y,     0),
                new Vector3Int(position.x,     position.y - 1, 0),
                new Vector3Int(position.x - 1, position.y,     0)
            };

            //Create this dock
            FlooringNormalPartOnTile newFlooring;
            {
                GameObject obj = new GameObject("Flooring");
                obj.transform.position = new Vector3(position.x, position.y, position.y);

                SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
                renderer.sortingLayerName = "Flooring";
                renderer.sprite = GetSprite(flooringVariant, positions, true, position, rotation);

                TileInformation tileInfo = TileInformationManager.Instance.GetTileInformation(position);

                newFlooring = new FlooringNormalPartOnTile(obj);
            }

            //Set neighbour sprites
            foreach (Vector3Int neighbour in neighbours)
            {
                TileInformation info = TileInformationManager.Instance.GetTileInformation(neighbour);

                if (PositionHasFlooringVariant(flooringVariant, neighbour, out FlooringNormalPartOnTile neighbourFlooring)
                    && info.NormalFlooringGroup.Rotation == rotation)
                {
                    neighbourFlooring.Renderer.sprite = GetSprite(flooringVariant, positions, true, neighbour, info.NormalFlooringGroup.Rotation);
                }
            }

            return newFlooring;
        }
    }

    private static void FindEdges(HashSet<Vector3Int> positions, out int minX, out int maxX, out int minY, out int maxY)
    {
        maxX = int.MinValue; maxY = int.MinValue; minX = int.MaxValue; minY = int.MaxValue;

        foreach (Vector3Int position in positions)
        {
            if (position.x > maxX)
                maxX = position.x;

            if (position.y > maxY)
                maxY = position.y;

            if (position.x < minX)
                minX = position.x;

            if (position.y < minY)
                minY = position.y;
        }
    }

    public static Sprite GetSprite(FlooringVariantBase flooringVariant, HashSet<Vector3Int> positionsToBeAddedOrRemoved, bool addingPositions, Vector3Int p, FlooringRotation r)
    {
        int resultCode = (r == FlooringRotation.Horizontal) ? 0b00000 : 0b10000;

        List<Tuple<Vector3Int, int>> codeToAddFromNeighbours = new List<Tuple<Vector3Int, int>>()
        {
            Tuple.Create(new Vector3Int(p.x,     p.y + 1, 0), 0b0001),
            Tuple.Create(new Vector3Int(p.x + 1, p.y,     0), 0b0010),
            Tuple.Create(new Vector3Int(p.x,     p.y - 1, 0), 0b0100),
            Tuple.Create(new Vector3Int(p.x - 1, p.y,     0), 0b1000)
        };

        foreach (Tuple<Vector3Int, int> t in codeToAddFromNeighbours)
        {
            TileInformation info = TileInformationManager.Instance.GetTileInformation(t.Item1);

            if (addingPositions)
            {
                if (PositionHasFlooringVariant(flooringVariant, t.Item1, out FlooringNormalPartOnTile flooring) && info.NormalFlooringGroup.Rotation == r)
                    resultCode = resultCode | t.Item2;
                else if (positionsToBeAddedOrRemoved.Contains(t.Item1))
                    resultCode = resultCode | t.Item2;
            }
            else
            {
                if (PositionHasFlooringVariant(flooringVariant, t.Item1, out FlooringNormalPartOnTile flooring) && !positionsToBeAddedOrRemoved.Contains(t.Item1))
                    resultCode = resultCode | t.Item2;
            }
        }

        return flooringVariant.CodeToSprite[resultCode];
    }

    private static bool PositionHasFlooringVariant(FlooringVariantBase flooringVariant, Vector3Int p, out FlooringNormalPartOnTile flooring)
    {
        flooring = null;
        TileInformation info = TileInformationManager.Instance.GetTileInformation(p);

        if (info == null)
            return false;

        if (info.NormalFlooringGroup == null)
            return false;

        if (info.NormalFlooringGroup.FlooringVariant != flooringVariant)
            return false;

        FlooringGroup group = (FlooringGroup)info.NormalFlooringGroup;
        flooring = group.NormalFloorings[p];
        return true;
    }

    public static bool TryRemoveFlooring(Vector3Int pos)
    {
        if (!FlooringRemoveable(pos))
            return false;

        TileInformation tileInfo = TileInformationManager.Instance.GetTileInformation(pos);

        //Update neighbour tiles
        {
            HashSet<Vector3Int> neighbourTiles = new HashSet<Vector3Int>();
            FlooringGroup thisGroup = tileInfo.GetTopFlooringGroup();
            for (int i = thisGroup.BottomLeft.x; i <= thisGroup.TopRight.x; i++)
            {
                neighbourTiles.Add(new Vector3Int(i, thisGroup.BottomLeft.y - 1, 0));
                neighbourTiles.Add(new Vector3Int(i, thisGroup.TopRight.y + 1, 0));
            }
            for (int i = thisGroup.BottomLeft.y; i <= thisGroup.TopRight.y; i++)
            {
                neighbourTiles.Add(new Vector3Int(thisGroup.BottomLeft.x - 1, i, 0));
                neighbourTiles.Add(new Vector3Int(thisGroup.TopRight.x + 1, i, 0));
            }

            HashSet<Vector3Int> toBeRemoved = new HashSet<Vector3Int>();
            foreach (KeyValuePair<Vector3Int, FlooringNormalPartOnTile> p in thisGroup.NormalFloorings)
                toBeRemoved.Add(p.Key);

            foreach (Vector3Int n in neighbourTiles)
            {
                TileInformation neighbourTileInfo = TileInformationManager.Instance.GetTileInformation(n);

                if (neighbourTileInfo == null || neighbourTileInfo.NormalFlooringGroup == null)
                    continue;

                FlooringGroup group = neighbourTileInfo.NormalFlooringGroup;

                group.NormalFloorings[n].Renderer.sprite = GetSprite(group.FlooringVariant, toBeRemoved, false, n, group.Rotation);
            }
        }

        //Actual removal
        tileInfo.RemoveFlooring();

        return true;
    }
}

public enum FlooringRotation
{
    Horizontal,
    Vertical
}