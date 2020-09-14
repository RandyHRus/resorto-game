using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ResourceManager : MonoBehaviour
{
    [Header("UI")]

    [SerializeField] private GameObject selectionPanel = null;
    public GameObject SelectionPanel => selectionPanel;

    [SerializeField] private GameObject regionSelection = null;
    public GameObject RegionSelection => regionSelection;

    [SerializeField] private GameObject structureSelection = null;
    public GameObject StructureSelection => structureSelection;

    [SerializeField] private GameObject progressbar = null;
    public GameObject Progressbar => progressbar;

    [SerializeField] private GameObject itemSlot = null;
    public GameObject ItemSlot => itemSlot;

    [SerializeField] private GameObject itemInformationDisplay = null;
    public GameObject ItemInformationDisplay => itemInformationDisplay;

    [SerializeField] private GameObject itemInformationDisplayWithCount = null;
    public GameObject ItemInformationDisplayWithCount => itemInformationDisplayWithCount;

    [SerializeField] private GameObject dropItemInventorySlot = null;
    public GameObject DropItemInventorySlot => dropItemInventorySlot;

    [SerializeField] private GameObject resizablePanel = null;
    public GameObject ResizablePanel => resizablePanel;

    [SerializeField] private Canvas inventoryCanvas = null;
    public Canvas InventoryCanvas => inventoryCanvas;

    [SerializeField] private Canvas indicatorsCanvas = null;
    public Canvas IndicatorsCanvas => indicatorsCanvas;

    [SerializeField] private GameObject houseCustomizationMenu = null;
    public GameObject HouseCustomizationMenu => houseCustomizationMenu;


    [Header("Colors")]

    [SerializeField] private Color32 red = Color.white;
    public Color32 Red => red;

    [SerializeField] private Color32 green = Color.white;
    public Color32 Green => green;

    [SerializeField] private Color32 yellow = Color.white;
    public Color32 Yellow => yellow;

    [EnumNamedArray(typeof(ItemTag)), SerializeField]
    private Color32[] itemTagColors = new Color32[Enum.GetNames(typeof(ItemTag)).Length];
    public Color32[] ItemTagColors => itemTagColors;

    [Header("Others")]

    [SerializeField] private GameObject character = null;
    public GameObject Character => character;

    [SerializeField] private Material diffuse = null;
    public Material Diffuse => diffuse;

    [SerializeField] private GameObject droppedItem = null;
    public GameObject DroppedItem => droppedItem;

    [SerializeField] private Sprite boxOutline = null;
    public Sprite BoxOutline => boxOutline;

    private static ResourceManager _instance;
    public static ResourceManager Instance { get { return _instance; } }
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
}
