using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomBarUIManager : MonoBehaviour
{
    [SerializeField] private RegionInformation startingRegion = null;
    [SerializeField] private RegionInformation[] regionInformationsToShowInList = null;
    [SerializeField] private StructureInformation[] structuresInformationToShowInList = null;
    [SerializeField] private Canvas bottomBarCanvas = null;

    private SelectionPanel regionPanel, structuresPanel;

    private void Awake()
    {
        //Set up region panel
        {
            regionPanel = new SelectionPanel(bottomBarCanvas.transform, new Vector2(20, 60));

            foreach (RegionInformation info in regionInformationsToShowInList)
            {
                Selection selection = new RegionSelection(info, regionPanel);
                regionPanel.InsertSelection(selection);
            }

            regionPanel.Show(false);
        }

        //Set up structures panel
        {
            structuresPanel = new SelectionPanel(bottomBarCanvas.transform, new Vector2(20, 60));

            foreach (StructureInformation structureInfo in structuresInformationToShowInList)
            {
                SelectionPanel variantsPanel = new SelectionPanel(structuresPanel.ObjectTransform, new Vector2(160, 0));

                foreach (StructureVariantInformation variantInfo in structureInfo.Variants)
                {
                    Selection variantSelection = new StructureVariantSelection(structureInfo, variantInfo, variantsPanel);
                    variantsPanel.InsertSelection(variantSelection);
                }

                Selection structuresSelection = new StructureSelection(structureInfo, variantsPanel, structuresPanel);
                structuresPanel.InsertSelection(structuresSelection);

                variantsPanel.Show(false);
            }

            structuresPanel.Show(false);
        }
    }

    public void BreakModeButtonPressed()
    {
        PlayerStateMachine.Instance.TrySwitchState<RemoveState>();
    }

    public void TerrainModeButtonClicked()
    {
        PlayerStateMachine.Instance.TrySwitchState<TerrainState>();
    }

    public void RegionsModeButtonClicked()
    {
        PlayerStateMachine.Instance.TrySwitchState<CreateRegionState>(new object[] { startingRegion });
        UIPanelsManager.Instance.SetCurrentPanel(regionPanel, false);
    }

    public void StructuresButtonClicked()
    {
        UIPanelsManager.Instance.SetCurrentPanel(structuresPanel, false);
    }
}
