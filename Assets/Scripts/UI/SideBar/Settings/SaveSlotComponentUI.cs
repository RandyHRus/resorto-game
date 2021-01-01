using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;

public abstract class SaveSlotComponentUI : ListComponentUI
{
    protected SaveInfo saveInfo;
    protected string rawFileName;

    public SaveSlotComponentUI(Transform parent, string saveInfoPath): base(ResourceManager.Instance.Prefab_saveSlotComponentUI, parent)
    {
        rawFileName = Path.GetFileNameWithoutExtension(saveInfoPath);     
        saveInfo = JsonConvert.DeserializeObject<SaveInfo>(File.ReadAllText(saveInfoPath));

        foreach (Transform t in ObjectTransform.GetComponentsInChildren<Transform>())
        {
            switch (t.tag)
            {
                case ("Name Field"):
                    OutlinedText nameText = new OutlinedText(t.gameObject);
                    nameText.SetText(saveInfo.PlayerName);
                    break;
                case ("Date Field"):
                    OutlinedText dateText = new OutlinedText(t.gameObject);
                    dateText.SetText(saveInfo.SaveTimeFormatted);
                    break;
                default:
                    break;
            }
        }
    }
}
