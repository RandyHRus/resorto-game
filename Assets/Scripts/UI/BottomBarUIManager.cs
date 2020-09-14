using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class BottomBarUIManager : MonoBehaviour
{
    [SerializeField] private RegionInformation startingRegion = null;
    [SerializeField] private RegionInformation[] regionInformationsToShowInList = null;

    [SerializeField] private StairsStructureInformation stairsStructure = null;
    [SerializeField] private DockStructureInformation dockStructure = null;
    [SerializeField] private FlooringStructureInformation flooringStructure = null;
    [SerializeField] private BuildingStructureInformation buildingStructure = null;

    private List<StructureInformation> structuresInformationToShowInList = null;

    [SerializeField] private Canvas bottomBarCanvas = null;

    private SelectionPanel regionPanel, structuresPanel;

    private void Awake()
    {
        structuresInformationToShowInList = new List<StructureInformation>()
        {
            stairsStructure,
            dockStructure,
            flooringStructure,
            buildingStructure
        };

    }

    private void Start()
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

            foreach (StructureInformation s in structuresInformationToShowInList)
            {
                SelectionPanel variantsPanel = new SelectionPanel(structuresPanel.ObjectTransform, new Vector2(160, 0));

                foreach (StructureVariantInformation variantInfo in s.Variants)
                {
                    Selection variantSelection = new StructureVariantSelection(s, variantInfo, variantsPanel);
                    variantsPanel.InsertSelection(variantSelection);
                }

                Selection structuresSelection = new StructureSelection(s, variantsPanel, structuresPanel);
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
