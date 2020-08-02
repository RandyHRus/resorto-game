using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RegionInformation : ScriptableObject
{
    [SerializeField] private string regionName = "";
    public string RegionName => regionName;

    [SerializeField] private TileLocation location = 0;
    public TileLocation Location => location;

    [SerializeField] private Color32 showColor = Color.white;
    public Color32 ShowColor => showColor;

    [SerializeField] private Sprite icon = null;
    public Sprite Icon => icon;
}
