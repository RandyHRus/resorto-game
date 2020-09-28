using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SidebarTerrainPanel : SidebarPanel
{
    [SerializeField] private TerrainOptionInformation[] terrainOptions = null;

    [System.Serializable]
    public class TerrainOptionInformation
    {
        [SerializeField] private string optionName = "";
        public string OptionName => optionName;

        [SerializeField] private Sprite icon = null;
        public Sprite Icon => icon;

        [SerializeField] private PlayerState onSelectState = null;
        public PlayerState OnSelectState => onSelectState;
    }

    public class TerrainOptionComponent: ListComponentUI
    {
        TerrainOptionInformation info;

        public TerrainOptionComponent(TerrainOptionInformation info, Transform parent): base(ResourceManager.Instance.TerrainOptionComponentUI, parent)
        {
            this.info = info;

            foreach (Transform tr in ObjectTransform.GetComponentsInChildren<Transform>())
            {
                if (tr.tag == "Name Field")
                {
                    OutlinedText nameText = new OutlinedText(tr.gameObject);
                    nameText.SetText(info.OptionName);
                }
                else if (tr.tag == "Icon Field")
                {
                    Image icon = tr.GetComponent<Image>();
                    icon.sprite = info.Icon;

                }
            }
        }

        public override void OnClick()
        {
            base.OnClick();
            PlayerStateMachine.Instance.SwitchState(info.OnSelectState.GetType(), null);
        }
    }

    private SelectionPanel<TerrainOptionComponent> terrainOptionsPanel;

    private void Start()
    {
        GameObject listObject = null;
        foreach (Transform tr in transform.GetComponentsInChildren<Transform>())
        {
            if (tr.tag == "List Field")
            {
                listObject = tr.gameObject;
            }
        }

        terrainOptionsPanel = new SelectionPanel<TerrainOptionComponent>(listObject);

        foreach (TerrainOptionInformation info in terrainOptions)
        {
            terrainOptionsPanel.InsertListComponent(new TerrainOptionComponent(info, terrainOptionsPanel.ObjectTransform));
        }
    }
}
