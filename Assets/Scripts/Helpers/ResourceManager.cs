using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ResourceManager : MonoBehaviour
{
    [Header("UI")]

    [SerializeField] private GameObject componentsListPanel = null;
    public GameObject ComponentsListPanel => componentsListPanel;

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

    [SerializeField] private GameObject taskInstanceUI = null;
    public GameObject TaskInstanceUI => taskInstanceUI;

    [SerializeField] private GameObject statisticComponentUI = null;
    public GameObject StatisticComponentUI => statisticComponentUI;

    [SerializeField] private GameObject structureTypeComponentUI = null;
    public GameObject StructureTypeComponentUI => structureTypeComponentUI;

    [SerializeField] private GameObject structureVariantComponentUI = null;
    public GameObject StructureVariantComponentUI => structureVariantComponentUI;

    [SerializeField] private GameObject regionComponent = null;
    public GameObject RegionComponent => regionComponent;

    [SerializeField] private GameObject subtaskInstanceUI = null;
    public GameObject SubtaskInstanceUI => subtaskInstanceUI;

    [SerializeField] private GameObject terrainOptionComponentUI = null;
    public GameObject TerrainOptionComponentUI => terrainOptionComponentUI;

    [SerializeField] private GameObject touristInformationComponentUI = null;
    public GameObject TouristInformationComponentUI => touristInformationComponentUI;

    [SerializeField] private Sprite subtaskActiveSprite = null;
    public Sprite SubtaskActiveSprite => subtaskActiveSprite;

    [SerializeField] private Sprite subtaskCompleteSprite = null;
    public Sprite SubtaskCompleteSprite => subtaskCompleteSprite;

    [SerializeField] private GameObject dialogueBoxInstance = null;
    public GameObject DialogueBoxInstance => dialogueBoxInstance;

    [SerializeField] private Transform player = null;
    public Transform Player => player;

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

    [SerializeField] private Color32 uiHighlightColor = Color.white;
    public Color32 UIHighlightColor => uiHighlightColor;

    [Header("Others")]

    [SerializeField] private GameObject character = null;
    public GameObject Character => character;

    [SerializeField] private Material diffuse = null;
    public Material Diffuse => diffuse;

    [SerializeField] private GameObject droppedItem = null;
    public GameObject DroppedItem => droppedItem;

    [SerializeField] private Sprite boxOutline = null;
    public Sprite BoxOutline => boxOutline;

    [SerializeField] private GameObject fishingLinePrefab = null;
    public GameObject FishingLinePrefab => fishingLinePrefab;

    [SerializeField] private GameObject caughtFishPrefab = null;
    public GameObject CaughtFishPrefab => caughtFishPrefab;

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
