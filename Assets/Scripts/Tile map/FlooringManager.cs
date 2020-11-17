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

    public static bool FlooringPlaceable(FlooringVariantBase flooringVariant, Vector2Int position)
    {
        TileInformationManager.Instance.TryGetTileInformation(position, out TileInformation info);
        TileInformationManager.Instance.TryGetTileInformation(new Vector2Int(position.x, position.y -1), out TileInformation belowInfo);

        if (info == null || belowInfo == null)
            return false;

        if (info.TopMostBuild != null|| belowInfo.TopMostBuild != null)
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

    public static bool TryPlaceFlooring(FlooringVariantBase flooringVariant, HashSet<Vector2Int> positions, FlooringRotation rotation)
    {
        foreach (Vector2Int position in positions)
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

    private static void CreateDock(FlooringVariantBase flooringVariant, HashSet<Vector2Int> positions, FlooringRotation rotation)
    {
        FindEdges(positions, out int minX, out int maxX, out int minY, out int maxY);

        //Create docks
        Dictionary<Vector2Int, FlooringNormalPartOnTile> floorings = new Dictionary<Vector2Int, FlooringNormalPartOnTile>();
        foreach (Vector2Int position in positions)
        {
            FlooringNormalPartOnTile f = CreateSingleDock((DockFlooringVariant)flooringVariant, position);
            floorings.Add(position, f);
        }

        //Create supports
        List<GameObject> supportObjects = new List<GameObject>(128);
        {
            float xSupportsSpacing = GetSpacing(minX, maxX, out int xSupportsCount);
            float ySupportsSpacing = GetSpacing(minY, maxY, out int ySupportsCount);

            for (int i = 0; i < xSupportsCount; i++)
            {
                for (int j = 0; j < ySupportsCount; j++)
                {
                    int xPos = minX + Mathf.RoundToInt(i * xSupportsSpacing);
                    int yPos = minY + Mathf.RoundToInt(j * ySupportsSpacing);
                    Vector2Int position = new Vector2Int(xPos, yPos);
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
        HashSet<Vector2Int> supportPositions = new HashSet<Vector2Int>();
        for (int i = minX; i <= maxX; i++)
        {
            Vector2Int pos = new Vector2Int(i, minY - 1);
            supportPositions.Add(pos);
        }

        //Set tiles through tileInformation script
        TileInformationManager.Instance.TryGetTileInformation(new Vector2Int(minX, minY), out TileInformation mainTileInfo);
        mainTileInfo.CreateFlooringGroup(floorings, supportPositions, new Vector2Int(maxX, maxY), supportObjects, flooringVariant, rotation);

        //The support object
        GameObject CreateDockSupport(DockFlooringVariant dockVariant, Vector2Int position)
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

        FlooringNormalPartOnTile CreateSingleDock(DockFlooringVariant dockVariant, Vector2Int position)
        {
            List<Vector2Int> neighbours = new List<Vector2Int>
            {
                new Vector2Int(position.x,     position.y + 1),
                new Vector2Int(position.x + 1, position.y),
                new Vector2Int(position.x,     position.y - 1),
                new Vector2Int(position.x - 1, position.y)
            };

            //Create this dock
            FlooringNormalPartOnTile newFlooring;
            {
                GameObject obj = new GameObject("Flooring");
                obj.transform.position = new Vector3(position.x, position.y, position.y);

                SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
                renderer.sortingLayerName = "Flooring";
                renderer.sprite = GetSprite(flooringVariant, positions, true, position, rotation);

                TileInformationManager.Instance.TryGetTileInformation(position, out TileInformation tileInfo);

                newFlooring = new FlooringNormalPartOnTile(obj);
            }

            //Set neighbour sprites
            foreach (Vector2Int neighbour in neighbours)
            {
                TileInformationManager.Instance.TryGetTileInformation(neighbour, out TileInformation info);

                if (PositionHasFlooringVariant(flooringVariant, neighbour, out FlooringNormalPartOnTile neighbourFlooring)
                    && info.NormalFlooringGroup.Rotation == rotation)
                {
                    neighbourFlooring.Renderer.sprite = GetSprite(flooringVariant, positions, true, neighbour, info.NormalFlooringGroup.Rotation);
                }
            }

            return newFlooring;
        }
    }

    private static void FindEdges(HashSet<Vector2Int> positions, out int minX, out int maxX, out int minY, out int maxY)
    {
        maxX = int.MinValue; maxY = int.MinValue; minX = int.MaxValue; minY = int.MaxValue;

        foreach (Vector2Int position in positions)
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

    public static Sprite GetSprite(FlooringVariantBase flooringVariant, HashSet<Vector2Int> positionsToBeAddedOrRemoved, bool addingPositions, Vector2Int p, FlooringRotation r)
    {
        int resultCode = (r == FlooringRotation.Horizontal) ? 0b00000 : 0b10000;

        List<Tuple<Vector2Int, int>> codeToAddFromNeighbours = new List<Tuple<Vector2Int, int>>()
        {
            Tuple.Create(new Vector2Int(p.x,     p.y + 1), 0b0001),
            Tuple.Create(new Vector2Int(p.x + 1, p.y),     0b0010),
            Tuple.Create(new Vector2Int(p.x,     p.y - 1), 0b0100),
            Tuple.Create(new Vector2Int(p.x - 1, p.y),     0b1000)
        };

        List<Tuple<Vector2Int, int>> codeToAddFromStairs = new List<Tuple<Vector2Int, int>>()
        {
            Tuple.Create(new Vector2Int(p.x,     p.y + 1), 0b0001),
            Tuple.Create(new Vector2Int(p.x + 1, p.y - 1), 0b0010),
            Tuple.Create(new Vector2Int(p.x,     p.y - 2), 0b0100),
            Tuple.Create(new Vector2Int(p.x - 1, p.y - 1), 0b1000)
        };

        //Add code from neighbours
        foreach (Tuple<Vector2Int, int> t in codeToAddFromNeighbours)
        {
            TileInformationManager.Instance.TryGetTileInformation(t.Item1, out TileInformation info);

            if (addingPositions)
            {
                if (PositionHasFlooringVariant(flooringVariant, t.Item1, out FlooringNormalPartOnTile flooring) && info.NormalFlooringGroup.Rotation == r)
                    resultCode = resultCode | t.Item2;
                else if (positionsToBeAddedOrRemoved != null && positionsToBeAddedOrRemoved.Contains(t.Item1))
                    resultCode = resultCode | t.Item2;
            }
            else
            {
                if (PositionHasFlooringVariant(flooringVariant, t.Item1, out FlooringNormalPartOnTile flooring) && (positionsToBeAddedOrRemoved == null || !positionsToBeAddedOrRemoved.Contains(t.Item1)))
                    resultCode = resultCode | t.Item2;
            }
        }

        //Add code from stairs
        foreach (Tuple<Vector2Int, int> t in codeToAddFromStairs)
        {
            TileInformationManager.Instance.TryGetTileInformation(t.Item1, out TileInformation tileInfo);
            if (tileInfo?.TopMostBuild?.BuildInfo is StairsVariant stairs)
            {
                resultCode = resultCode | t.Item2;
            }
        }

        return flooringVariant.CodeToSprite[resultCode];
    }

    private static bool PositionHasFlooringVariant(FlooringVariantBase flooringVariant, Vector2Int p, out FlooringNormalPartOnTile flooring)
    {
        flooring = null;
        if (!TileInformationManager.Instance.TryGetTileInformation(p, out TileInformation info)) 
            return false;

        if (info.NormalFlooringGroup == null)
            return false;

        if (info.NormalFlooringGroup.FlooringVariant != flooringVariant)
            return false;

        FlooringGroup group = (FlooringGroup)info.NormalFlooringGroup;
        flooring = group.NormalFloorings[p];
        return true;
    }
}