using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seashell : MonoBehaviour, ITileObjectFunctions
{
    ObjectOnTile objectData;

    public void Initialize(ObjectOnTile objectData)
    {
        this.objectData = objectData;
    }

    public void ClickInteract()
    {
        if (TileObjectsManager.TryRemoveObject(objectData.OccupiedTiles[0], out ObjectInformation removedObjectInfo))
        {
            InventoryManager.Instance.AddItem(removedObjectInfo.DropItem, 1);
        }
    }

    public void StepOff()
    {
        //throw new System.NotImplementedException();
    }

    public void StepOn()
    {
        //throw new System.NotImplementedException();
    }
}
