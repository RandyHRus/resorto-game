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


    public GameObject selectionPanel;
    public GameObject regionSelection;
    public GameObject progressbar;

    public Color32 red;
    public Color32 green;
    public Color32 yellow;

    public Color32[] elevationColors;

    [EnumNamedArray(typeof(TileLocation))]
    public Color32[] tileLocationColors = new Color32[Enum.GetNames(typeof(TileLocation)).Length];

    public GameObject character;

    public Material diffuse;
}
