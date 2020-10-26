using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemoveRightClickOptionUI : RightClickOptionUI
{
    IRemovable toRemove;

    public RemoveRightClickOptionUI(IRemovable toRemove, Transform parent): base("Remove", parent)
    {
        this.toRemove = toRemove;
    }

    public void RemoveBuild()
    {
        toRemove.Remove(true);
    }

    public override void OnClick()
    {
        base.OnClick();
        RemoveBuild();
    }
}
