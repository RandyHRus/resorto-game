using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInformationManager : MonoBehaviour
{
    [SerializeField] private ObjectInformation[] objectInformation = null;
    public Dictionary<int, ObjectInformation> objectInformationMap { get; private set; }

    private static ObjectInformationManager _instance;
    public static ObjectInformationManager Instance { get { return _instance; } }

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
            objectInformationMap = new Dictionary<int, ObjectInformation>();
            foreach (ObjectInformation info in objectInformation)
            {
                objectInformationMap.Add(info.id, info);
            }
        }
    }
}