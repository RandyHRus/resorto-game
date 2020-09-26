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

    ComponentsListPanel<StructureTypeComponentUI> structuresTypeList;

    public class StructureTypeComponentUI : ListComponentUI
    {
        private static int collapsibleTargetSize = 250;
        private static float collapseSpeed = 2000;
        private SelectionPanel<StructureVariantComponentUI> dropdownSelection;
        private bool collapsed = true;
        private bool collapseRunning = false;

        public delegate void CollapseStartDelegate(StructureTypeComponentUI caller, float targetY, float speed);
        public event CollapseStartDelegate OnCallapseStart;

        public StructureTypeComponentUI(StructureTypeInformation info, Transform parent, CollapseStartDelegate onCollapse) : base(ResourceManager.Instance.StructureTypeComponentUI, parent)
        {
            this.OnCallapseStart = onCollapse;

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
                else if (t.tag == "List Field")
                {
                    dropdownSelection = new SelectionPanel<StructureVariantComponentUI>(t.gameObject);
                    foreach (StructureInformation var in info.Variants)
                    {
                        dropdownSelection.InsertListComponent(new StructureVariantComponentUI(var, dropdownSelection.ObjectTransform));
                    }
                }
            }
        }

        public override void OnClick()
        {
            void OnCollapseProgress(float value)
            {
                dropdownSelection.RectTransform.sizeDelta = new Vector2(dropdownSelection.RectTransform.sizeDelta.x, value);
            }

            void OnCollapseEnd()
            {
                if (collapsed)
                    dropdownSelection.ObjectInScene.SetActive(false); //To fix weird button raycast collision bug

                collapseRunning = false;
            }

            base.OnClick();

            //We dont want double clicking happening, if collapsed while collapsing, things could mess up
            if (collapseRunning == true)
                return;

            collapseRunning = true;

            collapsed = !collapsed;

            if (!collapsed)
                dropdownSelection.ObjectInScene.SetActive(true);

            int targetYSize = collapsed ? 0 : collapsibleTargetSize;
            Vector2 currentSize = dropdownSelection.RectTransform.sizeDelta;

            float startValue = currentSize.y;

            Coroutines.Instance.StartCoroutine(LerpEffect.LerpSpeed(startValue, targetYSize, collapseSpeed, OnCollapseProgress, OnCollapseEnd));
            OnCallapseStart?.Invoke(this, targetYSize - startValue, collapseSpeed);
        }
    }

    public class StructureVariantComponentUI: ListComponentUI
    {
        public StructureVariantComponentUI(StructureInformation var, Transform parent) : base(ResourceManager.Instance.StructureVariantComponentUI, parent)
        {
            foreach (Transform t in ObjectTransform)
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
    }

    private void Start()
    {
        structuresTypeList = new ComponentsListPanel<StructureTypeComponentUI>(transform.Find("Build list").gameObject);

        structuresTypeList.InsertListComponent(new StructureTypeComponentUI(docks,     structuresTypeList.ObjectTransform, OnStructureTypeCollapseStart));
        structuresTypeList.InsertListComponent(new StructureTypeComponentUI(floorings, structuresTypeList.ObjectTransform, OnStructureTypeCollapseStart));
        structuresTypeList.InsertListComponent(new StructureTypeComponentUI(stairs,    structuresTypeList.ObjectTransform, OnStructureTypeCollapseStart));
        structuresTypeList.InsertListComponent(new StructureTypeComponentUI(buildings, structuresTypeList.ObjectTransform, OnStructureTypeCollapseStart));
        structuresTypeList.InsertListComponent(new StructureTypeComponentUI(objects,   structuresTypeList.ObjectTransform, OnStructureTypeCollapseStart));
    }

    public void OnStructureTypeCollapseStart(StructureTypeComponentUI component, float change, float speed)
    {
        structuresTypeList.SignalContentSizeChange(component, change, speed);
    }
}
