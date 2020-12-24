using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManageRegionSelection : CollapsibleComponentUI
{
    public override int CollapsibleTargetSize => 130;

    private RegionInstance instance;
    private Button removeButton;
    private Button additionalButton;
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
            else if (t.tag == "Button 2")
            {
                additionalButton = t.GetComponent<Button>();
                if (instance.AdditionalButtonText == null)
                {
                    t.gameObject.SetActive(false);
                }
                else
                {
                    OutlinedText buttonText = new OutlinedText(additionalButton.transform.GetChild(0).gameObject);
                    buttonText.SetText(instance.AdditionalButtonText);
                    additionalButton.onClick.AddListener(instance.OnAdditionalButtonClicked);
                }
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
        additionalButton.onClick.RemoveListener(instance.OnAdditionalButtonClicked);

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
