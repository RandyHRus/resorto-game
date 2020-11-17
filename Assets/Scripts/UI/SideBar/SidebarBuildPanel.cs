using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SidebarBuildPanel : SidebarPanel
{
    [System.Serializable] public class DockStructureTypeInformation : StructureTypeInformation<DockFlooringVariant> { }
    [SerializeField] public DockStructureTypeInformation docks = null;

    [System.Serializable] public class FlooringStructureTypeInformation : StructureTypeInformation<FlooringVariantBase> { }
    [SerializeField] public FlooringStructureTypeInformation floorings = null;

    [System.Serializable] public class StairsStructureTypeInformation : StructureTypeInformation<StairsVariant> { }
    [SerializeField] public StairsStructureTypeInformation stairs = null;

    [System.Serializable] public class BuildingsStructureTypeInformation : StructureTypeInformation<BuildingStructureVariant> { }
    [SerializeField] public BuildingsStructureTypeInformation buildings = null;

    [System.Serializable] public class ObjectsStructureTypeInformation : StructureTypeInformation<ObjectInformation> { }
    [SerializeField] public ObjectsStructureTypeInformation objects = null;

    [System.Serializable]
    public abstract class StructureTypeInformation
    {
        [SerializeField] private string structureName = null;
        public string StructureName => structureName;

        [SerializeField] private Sprite icon = null;
        public Sprite Icon => icon;

        [SerializeField] private PlayerState onSelectState = null;
        public PlayerState OnSelectState => onSelectState;

        public abstract StructureInformation[] Variants { get; }
    }

    [System.Serializable]
    public class StructureTypeInformation<T> : StructureTypeInformation where T : StructureInformation
    {
        [SerializeField] T[] variants = null;
        public override StructureInformation[] Variants => variants;
    }

    SelectionPanel<StructureTypeComponentUI> structuresTypeList;

    public class StructureTypeComponentUI: CollapsibleComponentUI
    {
        public override int CollapsibleTargetSize => 250;

        public StructureTypeComponentUI(GameObject prefab, Transform parent) : base (prefab, parent)
        {

        }
    }

    public class StructureTypeComponentUI<T> : StructureTypeComponentUI where T: StructureInformation
    {
        private SelectionPanel<StructureVariantComponentUI> dropdownSelection;

        public StructureTypeComponentUI(StructureTypeInformation<T> info, Transform parent) : base(ResourceManager.Instance.StructureTypeComponentUI, parent)
        {
            Transform[] allChildren = ObjectTransform.GetComponentsInChildren<Transform>();

            foreach (Transform t in allChildren)
            {
                if (t.tag == "Name Field")
                {
                    OutlinedText text = new OutlinedText(t.gameObject);
                    text.SetText(info.StructureName);
                }
                else if (t.tag == "Icon Field")
                {
                    Image itemIcon = t.GetComponent<Image>();
                    itemIcon.sprite = info.Icon;
                }
                else if (t.tag == "Collapsible Field")
                {
                    dropdownSelection = new SelectionPanel<StructureVariantComponentUI>(t.gameObject);
                    foreach (T var in info.Variants)
                    {
                        dropdownSelection.InsertListComponent(new StructureVariantComponentUI<T>(info, var, dropdownSelection.ObjectTransform));
                    }
                }
            }
        }
    }

    //So we can make lists
    public class StructureVariantComponentUI: ListComponentUI
    {
        public StructureVariantComponentUI(GameObject prefab, Transform parent): base(prefab, parent)
        {

        }
    }

    public class StructureVariantComponentUI<T>: StructureVariantComponentUI where T: StructureInformation
    {
        StructureTypeInformation type;
        StructureInformation structureInfo;

        public StructureVariantComponentUI(StructureTypeInformation<T> type, T var, Transform parent) : base(ResourceManager.Instance.StructureVariantComponentUI, parent)
        {
            this.type = type;
            structureInfo = var;

            foreach (Transform t in ObjectTransform.GetComponentsInChildren<Transform>())
            {
                if (t.tag == "Name Field")
                {
                    OutlinedText text = new OutlinedText(t.gameObject);
                    text.SetText(var.Name);
                }
                else if (t.tag == "Icon Field")
                {
                    Image itemIcon = t.GetComponent<Image>();
                    itemIcon.sprite = var.Icon;
                }
            }
        }

        public override void OnClick()
        {
            base.OnClick();
            PlayerStateMachineManager.Instance.SwitchState(type.OnSelectState.GetType(), new object[] { structureInfo });
        }
    }

    private void Start()
    {
        structuresTypeList = new SelectionPanel<StructureTypeComponentUI>(transform.Find("Build list").gameObject);

        structuresTypeList.InsertListComponent(new StructureTypeComponentUI<DockFlooringVariant>(docks,     structuresTypeList.ObjectTransform));
        structuresTypeList.InsertListComponent(new StructureTypeComponentUI<FlooringVariantBase>(floorings, structuresTypeList.ObjectTransform));
        structuresTypeList.InsertListComponent(new StructureTypeComponentUI<StairsVariant>(stairs,    structuresTypeList.ObjectTransform));
        structuresTypeList.InsertListComponent(new StructureTypeComponentUI<BuildingStructureVariant>(buildings, structuresTypeList.ObjectTransform));
        structuresTypeList.InsertListComponent(new StructureTypeComponentUI<ObjectInformation>(objects,   structuresTypeList.ObjectTransform));
    }
}
