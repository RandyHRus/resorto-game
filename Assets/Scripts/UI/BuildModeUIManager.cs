using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildModeUIManager : MonoBehaviour
{
    [SerializeField] private Canvas canvas = null;

    SelectionPanel regionPanel;

    private static BuildModeUIManager _instance;
    public static BuildModeUIManager Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            ClosePanel();
        }
    }

    private void Start()
    {
        regionPanel = new SelectionPanel(canvas, new Vector2(120, 260));

        foreach (KeyValuePair<int, RegionInformation> info in RegionInformationManager.Instance.regionInformationMap)
        {
            if (info.Key == RegionInformationManager.DEFAULT_REGION_ID)
                continue;

            Selection selection = new RegionSelection(info.Value, regionPanel.ObjectTransform);
            regionPanel.InsertSelection(selection);
        }

        ClosePanel();
    }

    public void ShowRegionPanel()
    {
        regionPanel.Show(true);
    }

    public void ClosePanel()
    {
        regionPanel.Show(false);
        //Can add more panels here
    }
}
