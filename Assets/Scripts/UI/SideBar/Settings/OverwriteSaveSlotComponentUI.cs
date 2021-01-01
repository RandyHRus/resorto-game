using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverwriteSaveSlotComponentUI : SaveSlotComponentUI
{
    public OverwriteSaveSlotComponentUI(Transform parent, string saveInfoPath) : base(parent, saveInfoPath)
    {

    }

    public override void OnClick()
    {
        base.OnClick();
        SaveManager.SaveGame(rawFileName);
    }
}
