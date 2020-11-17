using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManageRegionSelection : CollapsibleComponentUI
{
    public override int CollapsibleTargetSize => 100;

    private RegionInstance instance;

    private Button removeButton;

    private ComponentsListPanel<TextComponentUI> warningsList;

    public ManageRegionSelection(RegionInstance instance, Transform parent): base(ResourceManager.Instance.ManageRegionComponent, parent)
    {
        this.instance = instance;

        foreach (Transform t in ObjectTransform.GetComponentsInChildren<Transform>())
        {
            if (t.tag == "Name Field")
            {
                OutlinedText text = new OutlinedText(t.gameObject);
                text.SetText(instance.InstanceName);
            }
            else if (t.tag == "Icon Field")
            {
                Image img = t.GetComponent<Image>();
                img.sprite = instance.regionInformation.Icon;
            }
            else if (t.tag == "Button")
            {
                removeButton = t.GetComponent<Button>();
                removeButton.onClick.AddListener(OnRemoveButtonClicked);
            }
            else if (t.tag == "List Field")
            {
                warningsList = new ComponentsListPanel<TextComponentUI>(t.gameObject);
            }
        }

        instance.OnRegionModified += OnRegionModifiedHandler;
        RefreshWarnings();
    }

    private void OnRemoveButtonClicked()
    {
        RegionManager.RemoveRegion(instance);
    }

    public override void OnClick()
    {
        base.OnClick();
        CameraFunctions.LerpToPosition(instance.GetWeightedMiddlePos());
    }

    public override void Destroy()
    {
        base.Destroy();

        removeButton.onClick.RemoveListener(OnRemoveButtonClicked);
        instance.OnRegionModified -= OnRegionModifiedHandler;
    }

    private void OnRegionModifiedHandler(RegionInstance instance, TileInformation modifiedTile)
    {
        RefreshWarnings();
    }

    private void RefreshWarnings()
    {
        List<string> warnings = instance.GetWarnings();

        warningsList.ClearComponents();

        foreach (string w in warnings)
        {
            TextComponentUI warningComp = new TextComponentUI(w, ResourceManager.Instance.Red, 80, warningsList.ObjectTransform);
            warningsList.InsertListComponent(warningComp);
        }
    }

}
