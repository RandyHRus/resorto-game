using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanelsManager : MonoBehaviour
{
    private UIObject currentOpenPanel;
    private bool currentPanelShouldBeDeleted = false;

    private static UIPanelsManager _instance;
    public static UIPanelsManager Instance { get { return _instance; } }
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
            CloseCurrentPanel();
        }      
    }

    public void SetCurrentPanel(UIObject obj, bool shouldBeDeleted)
    {
        CloseCurrentPanel();
        obj.Show(true);

        currentOpenPanel = obj;
        currentPanelShouldBeDeleted = shouldBeDeleted;
    }

    public void CloseCurrentPanel()
    {
        if (currentOpenPanel != null)
        {
            if (currentPanelShouldBeDeleted)
            {
                currentOpenPanel.Destroy();
            }
            else
            {
                currentOpenPanel.Show(false);
            }
        }
        currentOpenPanel = null;
    }
}
