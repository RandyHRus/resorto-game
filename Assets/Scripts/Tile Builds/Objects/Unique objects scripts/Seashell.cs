using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seashell : MonoBehaviour, ITileObjectFunctions
{
    BuildOnTile buildData;

    public void Initialize(BuildOnTile buildData)
    {
        this.buildData = buildData;
    }

    public void ClickInteract()
    {
        if (RemoveManager.TryRemoveBuild(buildData))
        {
            buildData.BuildInfo.OnRemove(buildData);
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
