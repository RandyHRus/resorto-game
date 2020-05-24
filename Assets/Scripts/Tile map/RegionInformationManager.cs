using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionInformationManager : MonoBehaviour
{
    public static int DEFAULT_REGION_ID = 0;

    [SerializeField] private RegionInformation[] regions = null;
    public Dictionary<int, RegionInformation> regionInformationMap { get; private set; }

    private static RegionInformationManager _instance;
    public static RegionInformationManager Instance { get { return _instance; } }

    private void Awake()
    {
        //Singleton
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
        {
            //Initialize dictionary
            regionInformationMap = new Dictionary<int, RegionInformation>();
            foreach (RegionInformation info in regions)
            {
                regionInformationMap.Add(info.id, info);
            }
        }
    }
}

[System.Serializable]
public class RegionInformation
{
    public int id;
    public string name;
    public bool unique;
    public RegionsLocations location;
    public bool objectsCanBePlaced;
    public Color32 color;
    public Sprite icon;
}

public enum RegionsLocations
{
    land,
    water,
    sandOnly
}