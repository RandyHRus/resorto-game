using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FlooringGroup: IRemovable
{
    public FlooringRotation Rotation { get; private set; }
    public Dictionary<Vector2Int, FlooringNormalPartOnTile> NormalFloorings { get; private set; }
    public HashSet<Vector2Int> SupportFloorings { get; private set; }
    private List<GameObject> supportObjects;  //Only used for docks for now
    public FlooringVariantBase FlooringVariant { get; private set; }
    public Vector2Int BottomLeft { get; private set; } //This is the bottom left EXCLUDING the supports
    public Vector2Int TopRight { get; private set; }
    private List<BuildOnTile> connectedBuilds; //Stairs if on dock etc...

    public delegate void FlooringRemoved(FlooringGroup sender);
    public event FlooringRemoved OnFlooringRemoved;

    public FlooringGroup(Dictionary<Vector2Int, FlooringNormalPartOnTile> normalFloorings, HashSet<Vector2Int> supportFloorings,
        Vector2Int bottomLeft, Vector2Int topRight, List<GameObject> supportObjects, FlooringVariantBase flooringVariant, FlooringRotation rotation)
    {
        this.NormalFloorings = normalFloorings;
        this.SupportFloorings = supportFloorings;
        this.FlooringVariant = flooringVariant;
        this.Rotation = rotation;
        this.supportObjects = supportObjects;
        this.BottomLeft = bottomLeft;
        this.TopRight = topRight;
        this.connectedBuilds = new List<BuildOnTile>();
    }

    public void AddConnectedBuild(BuildOnTile build)
    {
        connectedBuilds.Add(build);
        build.OnBuildRemoved += OnConnectedBuildRemoved;
    }

    private void OnConnectedBuildRemoved(BuildOnTile sender)
    {
        connectedBuilds.Remove(sender);
        sender.OnBuildRemoved -= OnConnectedBuildRemoved;
    }

    public void Remove(bool removedThroughPlayerInteraction)
    {
        //TODO MOVE INTO ONREMOVED EVENT SOMEWHERE ELSE
        //Update neighbour tiles
        {
            HashSet<Vector2Int> neighbourTiles = new HashSet<Vector2Int>();

            for (int i = BottomLeft.x; i <= TopRight.x; i++)
            {
                neighbourTiles.Add(new Vector2Int(i, BottomLeft.y - 1));
                neighbourTiles.Add(new Vector2Int(i, TopRight.y + 1));
            }
            for (int i = BottomLeft.y; i <= TopRight.y; i++)
            {
                neighbourTiles.Add(new Vector2Int(BottomLeft.x - 1, i));
                neighbourTiles.Add(new Vector2Int(TopRight.x + 1, i));
            }

            HashSet<Vector2Int> toBeRemoved = new HashSet<Vector2Int>();
            foreach (KeyValuePair<Vector2Int, FlooringNormalPartOnTile> p in NormalFloorings)
                toBeRemoved.Add(p.Key);

            foreach (Vector2Int n in neighbourTiles)
            {
                if (!TileInformationManager.Instance.TryGetTileInformation(n, out TileInformation neighbourTileInfo))
                    continue;

                if (neighbourTileInfo.NormalFlooringGroup == null)
                    continue;

                FlooringGroup group = neighbourTileInfo.NormalFlooringGroup;

                //TODO change
                group.NormalFloorings[n].Renderer.sprite = FlooringManager.Instance.GetSprite(group.FlooringVariant, toBeRemoved, false, n, group.Rotation);
            }
        }

        foreach (KeyValuePair<Vector2Int, FlooringNormalPartOnTile> f in NormalFloorings)
        {
            UnityEngine.Object.Destroy(f.Value.GameObjectOnTile);
        }

        foreach (GameObject s in supportObjects)
        {
            UnityEngine.Object.Destroy(s);
        }

        //TODO MOVE INTO ONREMOVED EVENT SOMEWHERE ELSE
        //Remove connected builds
        for (int i = connectedBuilds.Count - 1; i >= 0; i--)
        {
            connectedBuilds[i].Remove(false);
        }

        OnFlooringRemoved?.Invoke(this);
    }
}

public class FlooringNormalPartOnTile
{
    public GameObject GameObjectOnTile { get; private set; }
    public SpriteRenderer Renderer { get; private set; }
    //Should be the support that is set BELOW this tile

    public FlooringNormalPartOnTile(GameObject gameObjectOnTile)
    {
        this.GameObjectOnTile = gameObjectOnTile;
        Renderer = gameObjectOnTile.GetComponent<SpriteRenderer>();
    }
}

public enum FlooringRotation
{
    Horizontal,
    Vertical
}