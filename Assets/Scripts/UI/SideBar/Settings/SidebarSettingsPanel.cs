using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SidebarSettingsPanel : SidebarPanel
{
    private Button button_deepListBack;
    private ComponentsListPanel<ListComponentUI> deepList;
    private ComponentsListPanel<SettingsButtonComponentUI> settingsList;

    private void Start()
    {
        foreach (Transform t in transform)
        {
            if (t.tag == "List Field")
            {
                settingsList = new ComponentsListPanel<SettingsButtonComponentUI>(t.gameObject);
                settingsList.InsertListComponent(new SettingsButtonComponentUI(settingsList.ObjectTransform, "Save", OnSaveButtonClickedHandler)); //Save button
                settingsList.InsertListComponent(new SettingsButtonComponentUI(settingsList.ObjectTransform, "Load", OnLoadButtonClickedHandler)); //Load button
            }
            else if (t.tag == "List Field 2")
            {
                deepList = new ComponentsListPanel<ListComponentUI>(t.gameObject);
                deepList.Show(false);
            }
            else if (t.tag == "Button")
            {
                button_deepListBack = t.GetComponent<Button>();
                button_deepListBack.onClick.AddListener(OnDeepListBackButtonPressedHandler);
            }
        }
    }

    private void OnSaveButtonClickedHandler()
    {
        List<ListComponentUI> saveSlots = new List<ListComponentUI>();

        //Add new save slot
        saveSlots.Add(new NewSaveComponentUI(deepList.ObjectTransform));

        //Add overWrite slots
        foreach (string saveInfoPath in SaveManager.EnumerateSaveFiles(SaveFileType.Info))
        {
            saveSlots.Add(new OverwriteSaveSlotComponentUI(deepList.ObjectTransform, saveInfoPath));
        }

        OpenDeepList(saveSlots);
    }

    public void OnLoadButtonClickedHandler()
    {
        List<ListComponentUI> loadSlots = new List<ListComponentUI>();

        foreach (string saveInfoPath in SaveManager.EnumerateSaveFiles(SaveFileType.Info))
        {
            loadSlots.Add(new LoadSaveSlotComponentUI(deepList.ObjectTransform, saveInfoPath));
        }

        OpenDeepList(loadSlots);
    }

    private void OpenDeepList(List<ListComponentUI> componentsToShow)
    {
        settingsList.Show(false);
        deepList.Show(true);

        if (deepList.ComponentsCount != 0)
        {
            Debug.LogWarning("Deep list already open, clearing...");
            deepList.ClearComponents();
        }

        foreach (ListComponentUI comp in componentsToShow)
        {
            deepList.InsertListComponent(comp);
        }
    }

    private void OnDeepListBackButtonPressedHandler()
    {
        //Close deep list
        deepList.ClearComponents();
        deepList.Show(false);
        settingsList.Show(true);
    }

    private void OnDestroy()
    {
        button_deepListBack.onClick.RemoveListener(OnDeepListBackButtonPressedHandler);
    }
}
