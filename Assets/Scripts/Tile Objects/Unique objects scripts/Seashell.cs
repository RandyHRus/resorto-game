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
        objectData.RemoveFromTile();
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
