using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
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

    [SerializeField] private GameObject selectionPanel = null;
    public GameObject SelectionPanel => selectionPanel;

    [SerializeField] private GameObject regionSelection = null;
    public GameObject RegionSelection => regionSelection;

    [SerializeField] private GameObject progressbar = null;
    public GameObject Progressbar => progressbar;

    [SerializeField] private GameObject itemSlot = null;
    public GameObject ItemSlot => itemSlot;

    [SerializeField] private Color32 red = Color.white;
    public Color32 Red => red;

    [SerializeField] private Color32 green = Color.white;
    public Color32 Green => green;

    [SerializeField] private Color32 yellow = Color.white;
    public Color32 Yellow => yellow;

    [SerializeField] private Color32[] elevationColors = null;
    public Color32[] ElevationColors => elevationColors;

    [EnumNamedArray(typeof(TileLocation)), SerializeField]
    private Color32[] tileLocationColors = new Color32[Enum.GetNames(typeof(TileLocation)).Length];
    public Color32[] TileLocationColors { get { return tileLocationColors; } private set { tileLocationColors = value; } }

    [SerializeField] private GameObject character = null;
    public GameObject Character => character;

    [SerializeField] private Material diffuse = null;
    public Material Diffuse => diffuse;

    [SerializeField] private GameObject droppedItem = null;
    public GameObject DroppedItem => droppedItem;

    [SerializeField] private Canvas inventoryCanvas = null;
    public Canvas InventoryCanvas => inventoryCanvas;
}
