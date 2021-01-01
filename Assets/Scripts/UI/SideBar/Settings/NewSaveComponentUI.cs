using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSaveComponentUI : ListComponentUI
{
    public NewSaveComponentUI(Transform parent): base(ResourceManager.Instance.Prefab_newSaveSlotComponentUI, parent)
    {

    }

    public override void OnClick()
    {
        base.OnClick();
        SaveManager.SaveGameNewFile();
    }
}
